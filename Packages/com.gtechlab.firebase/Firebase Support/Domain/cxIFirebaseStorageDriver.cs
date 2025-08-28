using System;
using System.Threading.Tasks;
using UnityEngine;

public interface cxIFirebaseStorageDriver {
    Task<string> UploadImage (string path, string key, byte[] imageBytes, string contentType);
}
