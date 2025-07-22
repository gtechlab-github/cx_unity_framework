#if UNITY_EDITOR || !UNITY_WEBGL

using System;
using System.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;

public class cxFirebaseStorageMobileDriver : cxIFirebaseStorageDriver {

    private string storageUrl;

    public cxFirebaseStorageMobileDriver (string storageUrl) {
        this.storageUrl = storageUrl;
    }

    public async Task<string> UploadImage (string path, string key, byte[] imageBytes, string contentType) {
        try {
            var storage = FirebaseStorage.DefaultInstance;
            string fileName = key;
            StorageReference storageRef = storage.GetReferenceFromUrl (storageUrl).Child (path + "/" + key);

            // 메타데이터 생성 및 Content-Type 설정
            var metadata = new MetadataChange {
                ContentType = contentType
            };

            // 기본 업로드 (contentType은 Firebase Storage가 자동으로 감지)
            var result = await storageRef.PutBytesAsync (imageBytes, metadata);

            // // 업로드 후 메타데이터 업데이트 (선택적)
            // if (!string.IsNullOrEmpty(contentType)) {
            //     try {
            //         var metadata = new StorageMetadata();
            //         // Unity Firebase SDK에서는 직접적인 contentType 설정이 제한적일 수 있음
            //         // 대신 파일 확장자를 통해 자동 감지되도록 함
            //         Debug.Log($"Uploaded with contentType: {contentType}");
            //     } catch (Exception metaEx) {
            //         Debug.LogWarning($"Failed to set metadata: {metaEx.Message}");
            //     }
            // }

            var url = await storageRef.GetDownloadUrlAsync ();
            return url.ToString ();
        } catch (Exception ex) {
            Debug.LogException (ex);
            return null;
        }
    }
}

#endif