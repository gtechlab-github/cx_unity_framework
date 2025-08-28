using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Serializers;

namespace FirebaseWebGL.Scripts.FirebaseBridge
{
    public static class FirebaseStorage
    {
        /// <summary>
        /// Uploads a byte array to storage
        /// </summary>
        /// <param name="path"> Storage path </param>
        /// <param name="data"> Bytes to upload encoded in a base 64 string </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal", EntryPoint = "UploadFile")]
        public static extern void _UploadFile(string path, string data, string contentType, int callbackId);

        /// <summary>
        /// Downloads a byte array from storage
        /// </summary>
        /// <param name="path"> Storage path </param>
        /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
        /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output). Will return a base 64 encoded string </param>
        /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
        [DllImport("__Internal", EntryPoint ="DownloadFile")]
        public static extern void _DownloadFile(string path,  int callbackId);

         public static async Task<string> UploadFile (string path, byte [] imageBytes, string contentType) {
            var context = cxWebCallack.AddCallback ();
            var base64 = Convert.ToBase64String(imageBytes);
            _UploadFile (path,base64 , contentType, context.callbackId);
            return await context.tcs.Task;
        }

        public static async Task<string> DownloadFile (string path){
            var context = cxWebCallack.AddCallback ();

            _DownloadFile(path , context.callbackId);
            return await context.tcs.Task;
        }
    }
}