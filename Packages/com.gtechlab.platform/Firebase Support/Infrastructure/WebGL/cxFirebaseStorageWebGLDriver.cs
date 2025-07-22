using System;
using System.Threading.Tasks;
using FirebaseWebGL.Scripts.FirebaseBridge;
using Newtonsoft.Json;
using UnityEngine;

public class cxFirebaseStorageWebGLDriver : cxIFirebaseStorageDriver {

    private string storageUrl;

    [Serializable]
    public class StorageMeta {
        /*
         "metadata": {
            "type": "file",
            "bucket": "historicsite-bmi.appspot.com",
            "generation": "1730210156432480",
            "metageneration": "1",
            "fullPath": "profileImages/rBtoBsdH7Oa8c4dnkyjXxT3yJ272.png",
            "name": "rBtoBsdH7Oa8c4dnkyjXxT3yJ272.png",
            "size": 259518,
            "timeCreated": "2024-10-29T13:55:56.441Z",
            "updated": "2024-10-29T13:55:56.441Z",
            "md5Hash": "2mnySr+okfTPZ+glHj/2IA==",
            "contentDisposition": "inline; filename*=utf-8''rBtoBsdH7Oa8c4dnkyjXxT3yJ272.png",
            "contentEncoding": "identity",
            "contentType": "application/octet-stream"
        }
        */
        public string type;
        public string bucket;
        public string fullPath;
        public string name;
        public int size;
    }

    [Serializable]
    public class UploadResponse {
        public int bytesTransferred;
        public int totalBytes;
        public string state;
        public StorageMeta metadata;
    }

    public cxFirebaseStorageWebGLDriver (string storageUrl) {
        this.storageUrl = storageUrl;
    }

    public async Task<string> UploadImage (string path, string key, byte[] imageBytes, string contentType) {
        try {
            path = string.Join("/", path, key);
            var jsonResponse = await FirebaseStorage.UploadFile(path, imageBytes, contentType);
            //Debug.Log("UploadImage Response:" + jsonResponse.ToString());
            var res = JsonConvert.DeserializeObject<UploadResponse>(jsonResponse);
            if(res.state != "success") {
                throw new Exception("UploadFile failed:"+jsonResponse);
            }

            var url = await FirebaseStorage.DownloadFile(path);
            Debug.Log($"UploadImage URL: {path} : {url}");
            return url;
        } catch (Exception ex) {
            Debug.LogException (ex);
            return null;
        }
    }
}