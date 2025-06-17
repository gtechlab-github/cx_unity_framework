using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class cxCachedWebContentLoader : cxSingleton<cxCachedWebContentLoader> {

    enum LoadingState {
        Unloaded = 0,
        Loading = 1,
        Loaded = 2,
        Failed = 3
    }

    class CachedContent {
        public string url;
        public UnityEngine.Object content;
        public System.Type contentType;

        internal BehaviorSubject<LoadingState> state = new BehaviorSubject<LoadingState> (LoadingState.Unloaded);

        public async Task WaitForLoad () {
            await new WaitUntil (() => state.Value == LoadingState.Loaded || state.Value == LoadingState.Failed);
        }

        public T To<T> () where T : UnityEngine.Object {
            if (typeof (T) != contentType) {
                throw new Exception ($"CachedContent To Exception contentType:{typeof(T)}, GetType:{typeof(T) }");
            }

            return content as T;
        }
    }

    private Dictionary<string, CachedContent> cachedContents = new Dictionary<string, CachedContent> ();

    private CachedContent Find (string url) {
        lock (cachedContents) {
            if (cachedContents.TryGetValue (url.ToLower (), out CachedContent image)) {
                return image;
            } else {
                return null;
            }
        }
    }

    public void ClearCache () {
        cachedContents.Clear ();
    }

    public bool HasCache (string url) {
        var cached = Find (url);
        return cached != null && cached.state.Value == LoadingState.Loaded;
    }

    public async Task<T> LoadAsset<T> (string url) where T : UnityEngine.Object {
        bool supported = cxResourceNaming.IsHttp (url) || cxResourceNaming.IsStreaming (url, out string path);
        if (!supported)
            throw new Exception ("cxCachedWebContentLoader unsupported url schema:" + url);

        var cached = await LoadImpl<T> (url);
        return (cached != null) ? cached.To<T> () : null;
    }

    // public async Task<Sprite> LoadSprite (string url) {
    //     var cached = await LoadImplEx<Sprite> (url);
    //     return (cached !=null) ? cached.To<Sprite>() : null;
    // }

    // public async Task<Texture2D> LoadTexture (string url) {
    //     var cached = await LoadImplEx<Texture2D> (url);
    //     return (cached !=null) ? cached.To<Texture2D>() : null;
    // }

    /*
        private async Task<CachedContent> LoadImpl (string url, bool sprite) {
            if (string.IsNullOrEmpty (url))
                return null;

            var cached = Find (url);

            if (cached == null) {

                lock (cachedContents) {
                cached = new CachedContent () {
                path = url.ToLower (),
                texture = null,
                sprite = null,
                    };
                    cached.state.OnNext (LoadingState.Loading);
                    cachedContents.Add (cached.path, cached);
                }

                bool isNet = cxResourceNaming.IsHttp (url);

                if (cxResourceNaming.IsStreaming (url, out string path)) {
                    isNet = true;
                    url = Path.Combine (Application.streamingAssetsPath, path);
                }

                if (isNet) {
                    cached.texture = await DownloadImage (url);
                    cached.state.OnNext (cached.texture != null ? LoadingState.Loaded : LoadingState.Failed);
                } else {
                    if (sprite) {
                        cached.sprite = LoadBuiltinResource<Sprite> (url);
                        cached.state.OnNext (cached.sprite != null ? LoadingState.Loaded : LoadingState.Failed);
                    } else {
                        cached.texture = LoadBuiltinResource<Texture2D> (url);
                        cached.state.OnNext (cached.sprite != null ? LoadingState.Loaded : LoadingState.Failed);
                    }
                }
            }

            await cached.WaitForLoad ();

            return cached;
        }
    */

    private async Task<CachedContent> LoadImpl<T> (string url) where T : UnityEngine.Object {
        if (string.IsNullOrEmpty (url))
            return null;

        var cached = Find (url);

        Type contentType = typeof (T);

        if (cached == null) {

            lock (cachedContents) {
            cached = new CachedContent () {
            url = url.ToLower (),
            // texture = null,
            // sprite = null,
            content = null,
            contentType = contentType
                };
                cached.state.OnNext (LoadingState.Loading);
                cachedContents.Add (cached.url, cached);
            }

            bool isNet = cxResourceNaming.IsHttp (url);

            if (cxResourceNaming.IsStreaming (url, out string path)) {
                isNet = true;
                url = cxResourceNaming.ToAppStreamingPath(path);

//                 url = Path.Combine (Application.streamingAssetsPath, path);

// #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
//                 url ="file://"+ url;
// #endif
            }

            UnityEngine.Object content = null;
            if (contentType == typeof (Sprite)) {
                var texture = await DownloadImage (url);
                if (texture != null)
                    content = ToSprite (texture);
            } else if (contentType == typeof (Texture2D)) {
                content = await DownloadImage (url);
            }  else if (contentType == typeof (AudioClip)) {
                content = await DownloadAudio (url);
            } else if(contentType == typeof(VideoClip)) {
                throw new Exception("cxCachedWebContent not support videoClip");
            } else if(contentType == typeof(TextAsset)) {
                content = await DownloadTextAsset (url);
            } else {
                throw new Exception("cxCachedWebContent unsupported type:"+nameof(contentType));
            }

            /*
            if (isNet) {
                if (contentType == typeof (Sprite)) {
                    var texture = await DownloadImage (url);
                    if (texture != null)
                        content = ToSprite (texture);
                } else if (contentType == typeof (Texture2D)) {
                    content = await DownloadImage (url);
                }
            } else {
                content = LoadBuiltinResource<T> (url);
            }
            */

            cached.content = content;
            cached.state.OnNext (content != null ? LoadingState.Loaded : LoadingState.Failed);
        }

        await cached.WaitForLoad ();
        return cached;
    }

    Sprite ToSprite (Texture2D texture) {
        return Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f), 100);
    }
    async Task<TextAsset> DownloadTextAsset (string url) {

        var op = UnityWebRequest.Get (url).SendWebRequest ();
        await new WaitUntil (() => op.isDone);

        TextAsset myTextAsset = null;

        if (op.webRequest.result == UnityWebRequest.Result.Success)
            myTextAsset = new TextAsset (op.webRequest.downloadHandler.text);
        else {
            Debug.LogWarning ($"cxCachedContentLoader DownloadTextAsset failed: {url}");
        }

        return myTextAsset;
    }
    async Task<Texture2D> DownloadImage (string url) {

        var op = UnityWebRequestTexture.GetTexture (url).SendWebRequest ();
        await new WaitUntil (() => op.isDone);

        Texture2D myTexture = null;

        if (op.webRequest.result == UnityWebRequest.Result.Success)
            myTexture = ((DownloadHandlerTexture) op.webRequest.downloadHandler).texture;
        else {
            Debug.LogWarning ($"cxCachedContentLoader DownloadImage failed: {url}");
        }

        return myTexture;
    }

     async Task<AudioClip> DownloadAudio (string url) {

        var req = UnityWebRequest.Get (url);
        req.downloadHandler = new DownloadHandlerAudioClip(string.Empty, AudioType.MPEG);

       var op= req.SendWebRequest ();
        await new WaitUntil (() => op.isDone);

        AudioClip myTexture = null;

        if (op.webRequest.result == UnityWebRequest.Result.Success)
            myTexture = ((DownloadHandlerAudioClip) op.webRequest.downloadHandler).audioClip;
        else {
            Debug.LogWarning ($"cxCachedContentLoader DownloadAudio failed: {url}");
        }

        return myTexture;
    }

    // T LoadBuiltinResource<T> (string path) where T : UnityEngine.Object {
    //     var myTexture = Resources.Load<T> (path);
    //     if (myTexture == null) {
    //         Debug.LogWarning ($"cxCachedContentLoader LoadBuiltinResource failed: {path}");

    //     }
    //     return myTexture;
    // }

    // CachedImage Save (string url, Texture2D image, Sprite sprite) {
    //     if (cachedImages.TryGetValue (url.ToLower (), out CachedImage c)) {
    //         Debug.Log ($"cxCachedImageLoader Save warning {url} already cached");
    //         return c;
    //     }

    //     CachedImage cachedImage = new CachedImage () {
    //         path = url.ToLower (),
    //         texture = image,
    //         sprite = sprite
    //     };

    //     cachedImages.Add (cachedImage.path, cachedImage);
    //     return cachedImage;
    // }

}