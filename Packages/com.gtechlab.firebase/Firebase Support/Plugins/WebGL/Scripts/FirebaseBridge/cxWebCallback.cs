using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AOT;
using UnityEngine;

public class cxWebCallack : MonoBehaviour {

    public class WebCallbackContext {
        private static int lastCallbackId = 10;

        public readonly int callbackId;
        public object payload;
        public bool stream = false;

        public Action<string> on;
        public TaskCompletionSource<string> tcs;

        public WebCallbackContext (object payload = null) {
            callbackId = lastCallbackId++;
            this.payload = payload;
            tcs = new TaskCompletionSource<string> ();
        }

        public WebCallbackContext (Action<string> on, object payload = null) {
            callbackId = lastCallbackId++;
            this.payload = payload;
            this.on = on;
        }
    }

    static readonly List<WebCallbackContext> WebCallbackChunks = new List<WebCallbackContext> ();

    void Awake () {
        DontDestroyOnLoad (gameObject);
        gameObject.name = "cxWebCallbackInstance";
    }
    /*

    [MonoPInvokeCallback (typeof (Action<int, string>))]
    public static void JSCallbackWithString (int callbackId, string json) {
        if(callbackId == 0)
            return;

        var chunk = WebCallbackChunks.Find (x => x.callbackId == callbackId);
        if (chunk == null) {
            Debug.LogWarning ("Web Callback not found for callId: " + callbackId);
            return;
        }

        if (!chunk.stream)
            WebCallbackChunks.Remove (chunk);

        if (chunk.on != null)
            chunk.on (json);
        else
            chunk.tcs.SetResult (json);
    }

    [MonoPInvokeCallback (typeof (Action<int, string>))]
    public static void JSFallbackWithString (int callbackId, string error) {
         if(callbackId == 0)
            return;

        var chunk = WebCallbackChunks.Find (x => x.callbackId == callbackId);
        if (chunk == null) {
            Debug.LogWarning ("Web Callback not found for callId: " + callbackId);
            return;
        }

        if (!chunk.stream)
            WebCallbackChunks.Remove (chunk);

        if (chunk.tcs != null)
            chunk.tcs.SetException(new Exception (error));
    }
    */
    
    public class CallbackParam {
        public int callbackId;
        public string jsonString;
    }

    public void JSCallbackWithString (string json) {
        try {
            var param = Newtonsoft.Json.JsonConvert.DeserializeObject<CallbackParam> (json);
            if (param.callbackId == 0)
                return;

            var chunk = WebCallbackChunks.Find (x => x.callbackId == param.callbackId);
            if (chunk == null) {
                Debug.LogWarning ("(JSCallbackWithString) Web Callback not found for callId: " + param.callbackId);
                return;
            }

            if (!chunk.stream)
                WebCallbackChunks.Remove (chunk);

            if (chunk.on != null)
                chunk.on (param.jsonString);
            else
                chunk.tcs.SetResult (param.jsonString);

        } catch (Exception ex) {
            Debug.LogException (ex);
            Debug.Log ("JSCallbackWithString Error: " + json);
        }
    }

    public void JSFallbackWithString (string json) {
       
        try {
            var param = Newtonsoft.Json.JsonConvert.DeserializeObject<CallbackParam> (json);

            var chunk = WebCallbackChunks.Find (x => x.callbackId == param.callbackId);
            if (chunk == null) {
                Debug.LogWarning ("(JSFallbackWithString) Web Callback not found for callId: " + param.callbackId);
                return;
            }

            if (!chunk.stream)
                WebCallbackChunks.Remove (chunk);

            if (chunk.tcs != null)
                chunk.tcs.SetException (new Exception (param.jsonString));

        } catch (Exception ex) {
            Debug.LogException (ex);
            Debug.Log ("JSFallbackWithString Error: " + json);
        }
    }

    public static WebCallbackContext AddCallback (Action<string> on, object playload = null, bool stream = false) {
        var context = new WebCallbackContext (on);
        context.stream = stream;
        WebCallbackChunks.Add (context);

        return context;
    }
    public static WebCallbackContext AddCallback (object playload = null, bool stream = false) {
        var context = new WebCallbackContext ();
        context.stream = stream;

        WebCallbackChunks.Add (context);
        return context;
    }

    public static void RemoveCallback (int callbackId) {
        var context = WebCallbackChunks.Find (x => x.callbackId == callbackId);
        if (context == null) {
            Debug.LogWarning ("(RemoveCallback) Web Callback not found for callId: " + callbackId);
            return;
        }

        WebCallbackChunks.Remove (context);
    }

    public static WebCallbackContext FindCallbackByPayload (object payload) {
        return WebCallbackChunks.Find (x => x.payload == payload);
    }
}