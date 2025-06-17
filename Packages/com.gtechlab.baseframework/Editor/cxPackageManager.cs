using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public static class cxPackageManager {
    /*
    Local
    "com.gtechlab.baseframework": "file:/Users/CRAZYBOX_SVN_ROOT/G-TechLab Frameworks/unity_framework_project/Packages/com.gtechlab.baseframework",
    "com.gtechlab.game": "file:/Users/CRAZYBOX_SVN_ROOT/G-TechLab Frameworks/unity_framework_project/Packages/com.gtechlab.game",
    "com.gtechlab.platform": "file:/Users/CRAZYBOX_SVN_ROOT/G-TechLab Frameworks/unity_framework_project/Packages/com.gtechlab.platform",

    Remote
    "com.gtechlab.baseframework" : "https://github.com/whilin/crazybox_unity_baseframework.git?path=/Packages/com.gtechlab.baseframework#v2.0.6",
    "com.gtechlab.game" : "https://github.com/whilin/crazybox_unity_baseframework.git?path=/Packages/com.gtechlab.game#v2.0.6",
    "com.gtechlab.platform" : "https://github.com/whilin/crazybox_unity_baseframework.git?path=/Packages/com.gtechlab.platform#v2.0.6",
    */

    public const string PackageLocalPath = "/Users/CRAZYBOX_SVN_ROOT/G-TechLab Frameworks/unity_framework_project/Packages/";
    // public const string PackageGitPath = "https://github.com/whilin/crazybox_unity_baseframework.git?path=/Packages/";
    public const string PackageGitPath = "https://github.com/gtechlab-github/cx_unity_framework.git?path=/Packages/";

    public class PackageFile {
        public Dictionary<string, string> dependencies;
    }

    [MenuItem ("Tools/G-Tech Lab/CX Package/To Local CX Package")]
    public static void ToLocalUPM () {
        var packageInfo = LoadPackage ();
        packageInfo.dependencies["com.gtechlab.baseframework"] = "file:" + PackageLocalPath + "com.gtechlab.baseframework";
        packageInfo.dependencies["com.gtechlab.game"] = "file:" + PackageLocalPath + "com.gtechlab.game";
        packageInfo.dependencies["com.gtechlab.platform"] = "file:" + PackageLocalPath + "com.gtechlab.platform";

        SavePackage (packageInfo);
    }

    [MenuItem ("Tools/G-Tech Lab/CX Package/To Git CX Package(Auto)")]
    public static void ToGitUPMAuto () {
        string filepath = Path.Combine (PackageLocalPath, "version.txt");
        using (var file = File.OpenText (filepath)) {
            string version = file.ReadLine ();
            Debug.Log ("Package Local Version:" + version);
            ToGitUPM (version);
        }
    }

    public static void ToGitUPM (string tag) {

        //string tag = "v2.0.7";

        var packageInfo = LoadPackage ();
        packageInfo.dependencies["com.gtechlab.baseframework"] = PackageGitPath + "com.gtechlab.baseframework#" + tag;
        packageInfo.dependencies["com.gtechlab.game"] = PackageGitPath + "com.gtechlab.game#" + tag;
        packageInfo.dependencies["com.gtechlab.platform"] = PackageGitPath + "com.gtechlab.platform#" + tag;

        SavePackage (packageInfo);
    }

    public static PackageFile LoadPackage () {
        string filepath = Path.Combine (Application.dataPath, "..", "Packages", "manifest.json");
        using (var file = File.OpenText (filepath)) {
            var packageInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<PackageFile> (file.ReadToEnd ());

            int k = 0;
            return packageInfo;
        }

        throw new System.Exception ("package file open failed:" + filepath);
    }

    public static void SavePackage (PackageFile packageInfo) {
        string filepath = Path.Combine (Application.dataPath, "..", "Packages", "manifest.json");
        using (var file = File.CreateText (filepath)) {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject (packageInfo);
            // var bytes = Encoding.UTF8.GetBytes (json);
            file.Write (JsonPrettify (json));
            file.Flush ();
            return;
        }

        throw new System.Exception ("package file open failed:" + filepath);
    }

    public static string JsonPrettify (string json) {
        using (var stringReader = new StringReader (json))
        using (var stringWriter = new StringWriter ()) {
            var jsonReader = new JsonTextReader (stringReader);
            var jsonWriter = new JsonTextWriter (stringWriter) { Formatting = Formatting.Indented };
            jsonWriter.WriteToken (jsonReader);
            return stringWriter.ToString ();
        }
    }
}