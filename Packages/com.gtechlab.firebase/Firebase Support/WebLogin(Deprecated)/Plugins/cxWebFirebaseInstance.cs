using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class cxWebFirebaseInstance : MonoSingleton<cxWebFirebaseInstance> {

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport ("__Internal")]
    public static extern void signInWithGoogle ();
    [DllImport ("__Internal")]
    public static extern void signOutGoogle ();
    [DllImport ("__Internal")]
    public static extern string hasAuthSessionGoogle ();
#endif

    public class FirebaseUser {
        public string uid;
        public string displayName;
        public string photoURL;
        public string email;
    }

    public class AuthCredential {
        public string providerId;
        public string signInMethod;
        public string oauthIdToken;
        public string oauthAccessToken;

    }
    public class FirebaseAuthResult {
        public FirebaseUser user;
        public AuthCredential credential;
    }

    public class FirebaseError {
        public string code;
        public string message;
    }

    TaskCompletionSource<FirebaseAuthResult> tcsSignIn; //= new TaskCompletionSource<object>();
    TaskCompletionSource<FirebaseUser> tcsHasAuthSession; //= new TaskCompletionSource<object>();

    public async Task<FirebaseAuthResult> SignInWithGoogle () {
        tcsSignIn = new TaskCompletionSource<FirebaseAuthResult> ();
#if UNITY_WEBGL && !UNITY_EDITOR
        signInWithGoogle ();
#else 
        tcsSignIn.SetException (new NotImplementedException ("SignInWithGoogle in Editor"));
#endif
        return await tcsSignIn.Task;
    }

    public void SignOut () {
#if UNITY_WEBGL && !UNITY_EDITOR
        signOutGoogle ();
#else 

#endif
    }

    public async Task<FirebaseUser> HasAuthSession () {

        tcsHasAuthSession = new TaskCompletionSource<FirebaseUser> ();

#if UNITY_WEBGL && !UNITY_EDITOR
        hasAuthSessionGoogle ();
#else 
        tcsHasAuthSession.SetException (new NotImplementedException ("HasAuthSession in Editor"));

#endif
        return await tcsHasAuthSession.Task;
    }

    void OnCurrentUserInfo (string user) {
        Debug.Log ("cxWebFirebaseInstance.OnCurrentUserInfo:" + user);

        if (!string.IsNullOrEmpty (user)) {
            var userInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<FirebaseUser> (user);
            tcsHasAuthSession?.SetResult (userInfo);
        } else {
            tcsHasAuthSession?.SetResult (null);
        }
    }

    void OnSignInCompleted (string result) {
        Debug.Log ("cxWebFirebaseInstance.OnSignInCompleted:" + result);
        //  cxLog.Verbose ("OnSignInCompleted:" + result);
        var authResult = Newtonsoft.Json.JsonConvert.DeserializeObject<FirebaseAuthResult> (result);
        tcsSignIn?.SetResult (authResult);
    }

    void OnSignInError (string error) {
        Debug.Log ("cxWebFirebaseInstance.OnSignInError:" + error);

        var errorResult = Newtonsoft.Json.JsonConvert.DeserializeObject<FirebaseError> (error);
        tcsSignIn?.SetException (new Exception (errorResult.message));
    }

    void OnSignOutCompleted () {
        cxLog.Verbose ("cxWebFirebaseInstance.OnSignOutCompleted");
    }

    void OnSignOutError (string error) {
        cxLog.Verbose ("cxWebFirebaseInstance.OnSignOutError : " + error);
    }
}