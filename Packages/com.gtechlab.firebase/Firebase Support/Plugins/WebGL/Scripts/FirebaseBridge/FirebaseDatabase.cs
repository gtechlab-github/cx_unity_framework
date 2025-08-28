using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FirebaseWebGL.Scripts.FirebaseBridge {
    public static class FirebaseDatabase {
        /// <summary>
        /// Gets JSON from a specified path
        /// Will return a snapshot of the JSON in the callback output
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "GetJSON")]
        public static extern void _GetJSON (string path, int callbackId);

        [DllImport ("__Internal", EntryPoint = "QueryEqual")]
        public static extern void _QueryEqual (string path, string key, string value, int callbackId);

        /// <summary>
        /// Posts JSON to a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="value"> JSON string to post to the specified path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "PostJSON")]
        public static extern void _PostJSON (string path, string value, int callbackId);

        /// <summary>
        /// Pushes JSON to a specified path with a Firebase generated unique key
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="value"> JSON string to push to the specified path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        // [DllImport ("__Internal", EntryPoint = "PushJSON")]
        // public static extern void _PushJSON (string path, string value, int callbackId);

        [DllImport ("__Internal", EntryPoint = "Push")]
        public static extern void _Push (string path, int callbackId);

        /// <summary>
        /// Updates JSON in a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="value"> JSON string to update in the specified path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "UpdateJSON")]
        public static extern void _UpdateJSON (string path, string value, int callbackId);

        /// <summary>
        /// Deletes JSON in a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "DeleteJSON")]
        public static extern void _DeleteJSON (string path, int callbackId);

        /// <summary>
        /// Listens for value changes in a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="objectName"> Name of the gameobject to call the onValueChanged/fallback of </param>
        /// <param name="onValueChanged"> Name of the method to call when the listener is triggered. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "ListenForValueChanged")]
        public static extern void _ListenForValueChanged (string path, int callbackId);

        /// <summary>
        /// Stops listening for value changed on a specific path
        /// </summary>
        /// <param name="path"> Database Path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "StopListeningForValueChanged")]
        public static extern void _StopListeningForValueChanged (string path, int callbackId);

        /// <summary>
        /// Listens for value changes in a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="objectName"> Name of the gameobject to call the onChildAdded/fallback of </param>
        /// <param name="onChildAdded"> Name of the method to call when the listener is triggered. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "ListenForChildAdded")]
        public static extern void _ListenForChildAdded (string path, int callbackId);

        /// <summary>
        /// Stops listening for child added on a specific path
        /// </summary>
        /// <param name="path"> Database Path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "StopListeningForChildAdded")]
        public static extern void _StopListeningForChildAdded (string path, int callbackId);

        /// <summary>
        /// Listens for value changes in a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="objectName"> Name of the gameobject to call the onChildChanged/fallback of </param>
        /// <param name="onChildChanged"> Name of the method to call when the listener is triggered. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "ListenForChildChanged")]
        public static extern void _ListenForChildChanged (string path, int callbackId);

        /// <summary>
        /// Stops listening for child changed on a specific path
        /// </summary>
        /// <param name="path"> Database Path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "StopListeningForChildChanged")]
        public static extern void _StopListeningForChildChanged (string path, int callbackId);

        /// <summary>
        /// Listens for value changes in a specified path
        /// </summary>
        /// <param name="path"> Database path </param>
        /// <param name="objectName"> Name of the gameobject to call the onChildRemoved/fallback of </param>
        /// <param name="onChildRemoved"> Name of the method to call when the listener is triggered. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "ListenForChildRemoved")]
        public static extern void _ListenForChildRemoved (string path, int callbackId);

        /// <summary>
        /// Stops listening for child removed on a specific path
        /// </summary>
        /// <param name="path"> Database Path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "StopListeningForChildRemoved")]
        public static extern void _StopListeningForChildRemoved (string path, int callbackId);

        /// <summary>
        /// Adds a specified number to a numeric value in a specified path using race conditions safe transactions
        /// If the value is not numeric or doesn't exist it will be treated as 0
        /// </summary>
        /// <param name="path"> Database Path </param>
        /// <param name="amount"> Number to add </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "ModifyNumberWithTransaction")]
        public static extern void _ModifyNumberWithTransaction (string path, float amount, int callbackId);

        /// <summary>
        /// Toggles a boolean flag in a specified path using race conditions safe transactions
        /// If the value is not a boolean or doesn't exist it will be treated as false
        /// </summary>
        /// <param name="path"> Database Path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport ("__Internal", EntryPoint = "ToggleBooleanWithTransaction")]
        public static extern void _ToggleBooleanWithTransaction (string path, int callbackId);

        public static async Task<string> GetJSON (string path) {
            var context = cxWebCallack.AddCallback ();
            _GetJSON (path, context.callbackId);
            return await context.tcs.Task;
        }

        public static async Task<string>  QueryEqual(string path, string key, string value) {
            var context = cxWebCallack.AddCallback ();
            _QueryEqual (path, key, value, context.callbackId);
            return await context.tcs.Task;
        }

        public static async Task<string> PostJSON (string path, string value) {
            var context = cxWebCallack.AddCallback ();
            _PostJSON (path, value, context.callbackId);
            return await context.tcs.Task;
        }

        // public static async Task<string> PushJSON (string path, string value) {
        //     var context = cxWebCallack.AddCallback ();
        //     _PushJSON (path, value, context.callbackId);
        //     return await context.tcs.Task;
        // }

        public static async Task<string> Push (string path) {
            var context = cxWebCallack.AddCallback ();
            _Push (path, context.callbackId);
            return await context.tcs.Task;
        }
        
        public static async Task<string> UpdateJSON (string path, string value) {
            var context = cxWebCallack.AddCallback ();
            _UpdateJSON (path, value, context.callbackId);
            return await context.tcs.Task;
        }

        public static async Task<string> DeleteJSON (string path) {
            var context = cxWebCallack.AddCallback ();
            _DeleteJSON (path, context.callbackId);
            return await context.tcs.Task;
        }

        public static void ListenForValueChanged (string path, Action<string> on) {
            var context = cxWebCallack.AddCallback (on, playload: on, stream: true);
            _ListenForValueChanged (path, context.callbackId);
        }

        public static void StopListeningForValueChanged (string path, Action<string> on) {
            var onCallback = cxWebCallack.FindCallbackByPayload (on);
            if (onCallback != null) {
                cxWebCallack.RemoveCallback (onCallback.callbackId);
            }

            var context = cxWebCallack.AddCallback ();
            _StopListeningForValueChanged (path, context.callbackId);
        }

        public static void ListenForChildAdded (string path, Action<string> on) {
            var context = cxWebCallack.AddCallback (on, playload: on, stream: true);
            _ListenForChildAdded (path, context.callbackId);
        }

        public static void StopListeningForChildAdded (string path, Action<string> on) {
            var onCallback = cxWebCallack.FindCallbackByPayload (on);
            if (onCallback != null) {
                cxWebCallack.RemoveCallback (onCallback.callbackId);
            }

            var context = cxWebCallack.AddCallback ();
            _StopListeningForChildAdded (path, context.callbackId);
        }

        public static void ListenForChildChanged (string path, Action<string> on) {
            var context = cxWebCallack.AddCallback (on, playload: on, stream: true);
            _ListenForChildChanged (path, context.callbackId);
        }

        public static void StopListeningForChildChanged (string path, Action<string> on) {
            var onCallback = cxWebCallack.FindCallbackByPayload (on);
            if (onCallback != null) {
                cxWebCallack.RemoveCallback (onCallback.callbackId);
            }

            var context = cxWebCallack.AddCallback ();
            _StopListeningForChildChanged (path, context.callbackId);
        }

        public static void ListenForChildRemoved (string path, Action<string> on) {
            var context = cxWebCallack.AddCallback (on, playload: on, stream: true);
            _ListenForChildRemoved (path, context.callbackId);
        }

        public static void StopListeningForChildRemoved (string path, Action<string> on) {
            var onCallback = cxWebCallack.FindCallbackByPayload (on);
            if (onCallback != null) {
                cxWebCallack.RemoveCallback (onCallback.callbackId);
            }

            var context = cxWebCallack.AddCallback ();
            _StopListeningForChildRemoved (path, context.callbackId);
        }

        public static async Task<string> ModifyNumberWithTransaction (string path, float amount) {
            var context = cxWebCallack.AddCallback ();
            _ModifyNumberWithTransaction (path, amount, context.callbackId);
            return await context.tcs.Task;
        }

        public static async Task<string> ToggleBooleanWithTransaction (string path) {
            var context = cxWebCallack.AddCallback ();
            _ToggleBooleanWithTransaction (path, context.callbackId);
            return await context.tcs.Task;
        }

    }
}