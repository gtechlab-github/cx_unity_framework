using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Obsolete("Use cxUINetImage")]
[RequireComponent (typeof (Mask))]
public class cxUIProfileImage : MonoBehaviour {
    static Dictionary<string, Texture> Caching = new Dictionary<string, Texture> ();

    [SerializeField]
    private RawImage photoImage;
    [SerializeField]
    private string url;

    public static void ClearCaching() {
        Caching.Clear();
    }

    void Awake () {

    }

    private void Start () {
        if (!string.IsNullOrEmpty (url))
            Load (url);
        else
            SetEmpty ();
    }

    public void Load (string url) {
        this.url = url;
        SetEmpty ();

        if (!string.IsNullOrEmpty (url)) {

            if (Caching.TryGetValue (url, out Texture cachedTexture)) {
                photoImage.color = Color.white;
                photoImage.texture = cachedTexture;

            } else {

                UnityWebRequestTexture.GetTexture (url)
                    .SendWebRequest ()
                    .AsObservable ()
                    .Subscribe (op => {
                        UnityWebRequestAsyncOperation op2 = (UnityWebRequestAsyncOperation) op;

                        if (op2.isDone && op2.webRequest.result == UnityWebRequest.Result.Success) {
                            Texture myTexture = ((DownloadHandlerTexture) op2.webRequest.downloadHandler).texture;
                            photoImage.color = Color.white;
                            photoImage.texture = myTexture;

                           SaveCache (url, myTexture);
                        }

                    }).AddTo (this);
            }
        }
    }

    void SaveCache(string url, Texture myTexture){
        if(!Caching.ContainsKey(url))
            Caching.Add (url, myTexture);
    }

    void SetEmpty () {
        photoImage.color = Color.gray;
        photoImage.texture = null;
    }
}