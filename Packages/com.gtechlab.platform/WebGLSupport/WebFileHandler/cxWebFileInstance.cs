using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;


public class cxWebFileInstance : MonoBehaviour {

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport ("__Internal")]
    private static extern void downloadFile (byte[] array, int size, string fileNamePtr);
#endif

    private void Awake () {
        if (!gameObject.name.Equals ("cxWebFileInstance")) {
            Debug.LogError ("cxWebFileInstance must named with cxWebFileInstance but " + gameObject.name);
        }
    }

    public static void DownloadFile (byte[] array, int size, string fileNamePtr) {
#if UNITY_WEBGL && !UNITY_EDITOR
        downloadFile (array, size, fileNamePtr);
#else
        var file = Path.Combine (Application.dataPath, "..", fileNamePtr);
        File.WriteAllBytes (file, array);
#endif
    }
}