using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FirebaseWebGL.Scripts.Objects;
using UnityEngine;

namespace FirebaseWebGL.Scripts.FirebaseBridge {
    public static class FirebaseAuth {

        [DllImport ("__Internal", EntryPoint = "CheckCurrentAuthUser")]
        public static extern void _CheckCurrentAuthUser (int callbackId);

        /// <summary>
        /// Creates and signs in a user anonymous
        /// </summary>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "SignInAnonymously")]
        public static extern void _SignInAnonymously (int callbackId);

        /// <summary>
        /// Creates a user with email and password
        /// </summary>
        /// <param name="email"> User email </param>
        /// <param name="password"> User password </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "CreateUserWithEmailAndPassword")]
        public static extern void _CreateUserWithEmailAndPassword (string email, string password, int callbackId);

        /// <summary>
        /// Signs in a user with email and password
        /// </summary>
        /// <param name="email"> User email </param>
        /// <param name="password"> User password </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "SignInWithEmailAndPassword")]
        public static extern void _SignInWithEmailAndPassword (string email, string password, int callbackId);

        /// <summary>
        /// Signs in a user with Google
        /// </summary>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "SignInWithGoogle")]
        public static extern void _SignInWithGoogle (int callbackId);

        /// <summary>
        /// Signs in a user with Facebook
        /// </summary>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "SignInWithFacebook")]
        public static extern void _SignInWithFacebook (int callbackId);

        /// <summary>
        /// Listens for changes of the auth state (sign in/sign out)
        /// </summary>
        /// <param name="objectName"> Name of the gameobject to call the onUserSignedIn/onUserSignedOut of </param>
        /// <param name="onUserSignedIn"> Name of the method to call when the user signs in. Method must have signature: void Method(string output). Will return a serialized FirebaseUser object </param>
        /// <param name="onUserSignedOut"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output) </param>
        [DllImport ("__Internal", EntryPoint = "OnAuthStateChanged")]
        public static extern void _OnAuthStateChanged (int callbackId);

        [DllImport ("__Internal", EntryPoint = "SendPasswordResetEmail")]
        public static extern void _SendPasswordResetEmail (string email, int callbackId);

        [DllImport ("__Internal", EntryPoint = "SignOut")]
        public static extern void _SignOut ();

        public static async Task<FirebaseUser> CheckCurrentAuthUser () {
            var context = cxWebCallack.AddCallback ();
            _CheckCurrentAuthUser (context.callbackId);

            string json = await context.tcs.Task;
            var user = JsonUtility.FromJson<FirebaseUser> (json);
            return user;
        }

        public static async Task<FirebaseUser> SignInAnonymously () {
            var context = cxWebCallack.AddCallback ();
            _SignInAnonymously (context.callbackId);

            string json = await context.tcs.Task;
            var user = JsonUtility.FromJson<FirebaseUser> (json);
            return user;

        }
        public static async Task<FirebaseUser> CreateUserWithEmailAndPassword (string email, string password) {

            var context = cxWebCallack.AddCallback ();

            _CreateUserWithEmailAndPassword (email, password, context.callbackId);
            string json = await context.tcs.Task;
            var user = JsonUtility.FromJson<FirebaseUser> (json);
            return user;
        }

        public static async Task<FirebaseUser> SignInWithEmailAndPassword (string email, string password) {
            var context = cxWebCallack.AddCallback ();

            _SignInWithEmailAndPassword (email, password, context.callbackId);
            string json = await context.tcs.Task;
            var user = JsonUtility.FromJson<FirebaseUser> (json);
            return user;
        }

        public static async Task SendPasswordResetEmail (string email) {
            var context = cxWebCallack.AddCallback ();
            _SendPasswordResetEmail (email, context.callbackId);
            string json = await context.tcs.Task;
            // var user = JsonUtility.FromJson<FirebaseUser> (json);
            // return user;
        }

        public static async Task<FirebaseUser> SignInWithGoogle () {
            var context = cxWebCallack.AddCallback ();

            _SignInWithGoogle (context.callbackId);
            string json = await context.tcs.Task;
            var user = JsonUtility.FromJson<FirebaseUser> (json);
            return user;
        }

        public static async Task<FirebaseUser> SignInWithFacebook () {
            var context = cxWebCallack.AddCallback ();

            _SignInWithFacebook (context.callbackId);
            string json = await context.tcs.Task;
            var user = JsonUtility.FromJson<FirebaseUser> (json);
            return user;
        }

        public static void OnAuthStateChanged (Action<string> onChanged) {
            var context = cxWebCallack.AddCallback (on: onChanged, playload: onChanged, stream: true);
            _OnAuthStateChanged (context.callbackId);
        }

        public static void OffAuthStateChanged (Action<string> onChanged) {
            var context = cxWebCallack.FindCallbackByPayload (onChanged);
            if (context != null) {
                cxWebCallack.RemoveCallback (context.callbackId);
            }
        }

        public static void SignOut () {
            _SignOut ();
        }
    }
}