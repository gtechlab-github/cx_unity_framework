using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class cxWebKakaoInstance : MonoSingleton<cxWebKakaoInstance> {

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport ("__Internal")]
    public static extern void signInWithKakao ();
    [DllImport ("__Internal")]
    public static extern void signOutKakao ();
    [DllImport ("__Internal")]
    public static extern string hasAuthSessionKakao ();
#endif

    public class KakaoUser {
        public string uid;
        public string displayName;
        public string photoURL;
        public string email;
    }

    public class KakaoError {
        public string error;
        public string error_description;
    }

    TaskCompletionSource<KakaoUser> tcsSignIn; //= new TaskCompletionSource<object>();
    TaskCompletionSource<KakaoUser> tcsHasAuthSession; //= new TaskCompletionSource<object>();

    public async Task<KakaoUser> SignInWithKakao () {
        tcsSignIn = new TaskCompletionSource<KakaoUser> ();
#if UNITY_WEBGL && !UNITY_EDITOR
        signInWithKakao ();
#else 
        tcsSignIn.SetException (new NotImplementedException ("SignInWithKakao in Editor"));
#endif
        return await tcsSignIn.Task;
    }

    public void SignOut () {
#if UNITY_WEBGL && !UNITY_EDITOR
        signOutKakao ();
#else 

#endif
    }

    public async Task<KakaoUser> HasAuthSession () {

        tcsHasAuthSession = new TaskCompletionSource<KakaoUser> ();

#if UNITY_WEBGL && !UNITY_EDITOR
        hasAuthSessionKakao ();
#else 
        tcsHasAuthSession.SetException (new NotImplementedException ("HasAuthSession in Editor"));
#endif
        return await tcsHasAuthSession.Task;
    }

    void OnCurrentUserInfo (string user) {
        Debug.Log ("cxWebKakaoInstance.OnCurrentUserInfo:" + user);

        if (!string.IsNullOrEmpty (user)) {
            var userInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<KakaoUser> (user);
            tcsHasAuthSession?.SetResult (userInfo);
        } else {
            tcsHasAuthSession?.SetResult (null);
        }
    }

    void OnSignInCompleted (string result) {
        Debug.Log ("cxWebKakaoInstance.OnSignInCompleted:" + result);
        var authResult = Newtonsoft.Json.JsonConvert.DeserializeObject<KakaoUser> (result);
        tcsSignIn?.SetResult (authResult);
    }

    void OnSignInError (string error) {
        Debug.Log ("cxWebKakaoInstance.OnSignInError:" + error);

        var errorResult = Newtonsoft.Json.JsonConvert.DeserializeObject<KakaoError> (error);
        tcsSignIn?.SetException (new Exception (errorResult.error));
    }

    void OnSignOutCompleted () {
        cxLog.Verbose ("cxWebKakaoInstance.OnSignOutCompleted");
    }

    void OnSignOutError (string error) {
        cxLog.Verbose ("cxWebKakaoInstance.OnSignOutError : " + error);
    }
}