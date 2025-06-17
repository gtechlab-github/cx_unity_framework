using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;

public class cxWebEventCallbackObject {
    public string eventName;
    public Dictionary<string, object> arguments;

    public T GetArgumentObject<T> () where T : class {
        if (arguments != null)
            return JObject.FromObject (arguments).ToObject<T> ();
        else
            return null;
    }
}

public class cxWebBasicInstance : MonoBehaviour {

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport ("__Internal")]
    private static extern void hello ();

    [DllImport ("__Internal")]
    private static extern void setFullScreen (bool on);

    [DllImport ("__Internal")]
    private static extern void closeWindow ();

    [DllImport ("__Internal")]
    private static extern string getCookie (string cname);

    [DllImport ("__Internal")]
    private static extern string setCookie (string cname, string cvalue, int expireDays);

    [DllImport ("__Internal")]
    private static extern bool isMobileDevice ();

    [DllImport ("__Internal")]
    public static extern bool isIPhoneDevice ();

    [DllImport ("__Internal")]
    private static extern void installEventCallback ();

#endif

    private Subject<cxWebEventCallbackObject> webEventCall = new Subject<cxWebEventCallbackObject> ();
    public IObservable<cxWebEventCallbackObject> webEventCallAsObservable => webEventCall.AsObservable ();

    private void Awake () {
        if (!gameObject.name.Equals ("cxWebBasicInstance")) {
            Debug.LogError ("cxWebBasicInstance must named with cxWebBasicInstance but " + gameObject.name);
        }

        InstallEventCallback ();
    }

    private void InstallEventCallback () {
#if UNITY_WEBGL && !UNITY_EDITOR
        installEventCallback ();
#endif
    }

    public static void Hello () {
#if UNITY_WEBGL && !UNITY_EDITOR
        hello ();
#endif
    }

    public static void SetFullScreen (bool on) {
#if UNITY_WEBGL && !UNITY_EDITOR
        setFullScreen (on);
#endif
    }

    public static void CloseWindow () {
#if UNITY_WEBGL && !UNITY_EDITOR
        closeWindow ();
#endif
    }


    public static bool IsMobileDevice(){
        #if UNITY_WEBGL && !UNITY_EDITOR
            return isMobileDevice();
        #else
            return false;
        #endif
    }

    public static bool IsIPhoneDevice(){
          #if UNITY_WEBGL && !UNITY_EDITOR
            return isIPhoneDevice();
        #else
            return false;
        #endif
    }

    public static string GetCookie (string cname) {

#if UNITY_WEBGL && !UNITY_EDITOR
        return getCookie (cname);
#else
        return string.Empty;
#endif
    }

    public static void SetCookie (string cname, string cvalue, int expireDays) {
#if UNITY_WEBGL && !UNITY_EDITOR
        setCookie (cname, cvalue, expireDays);
#else

#endif
    }

    /* Callback Call
    {
        eventName : "eventName",
        arguments : {

        } //json object
    }
    */

    void OnEventCallback (string eventName, string arguments) {
        cxLog.Log("cxWebBasicInstance OnEventCallback eventName:"+eventName+", arguments:"+arguments);
        var argObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>> (arguments);

        // if (string.IsNullOrEmpty (webObj.eventName)) {
        //     Debug.LogWarning ("cxWebBasicInstance Callback Format Error, eventName not found :" + arguments);
        //     return;
        // }

        cxWebEventCallbackObject obj = new cxWebEventCallbackObject(){
            eventName = eventName,
            arguments = argObj
        };

        webEventCall.OnNext (obj);
    }
}