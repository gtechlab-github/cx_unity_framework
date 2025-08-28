using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class cxFirebaseDBSnapshot {

    public string Key { get; private set; }
    public string Path { get; private set; }
    public string jsonValue { get; private set; }
    public Dictionary<string, object> ObjectValue { get; private set; }

    public bool Exists => !string.IsNullOrEmpty (jsonValue);

    public cxFirebaseDBRef Ref {
        get {
            return new cxFirebaseDBRef (Path);
        }
    }

    internal cxFirebaseDBSnapshot (string path, string jsonValue) {

        if(jsonValue == "null")
            jsonValue = string.Empty;
            
        this.Key = path.Split ('/').Last ();
        this.Path = path;
        this.jsonValue = jsonValue;
        try {
            this.ObjectValue = !string.IsNullOrEmpty (jsonValue) ? JsonConvert.DeserializeObject<Dictionary<string, object>> (jsonValue) : null;
        } catch (Exception e) {
            this.ObjectValue = null;
        }
    }

    internal cxFirebaseDBSnapshot (string path, Dictionary<string, object> snapshot) {
        this.Key = path.Split ('/').Last ();
        this.Path = path;
        this.jsonValue = snapshot != null ? JsonConvert.SerializeObject (snapshot) : null;
        this.ObjectValue = snapshot;
    }

    public string GetRawJsonValue () {
        return jsonValue;
    }

    public T GetValue<T>() {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T> (jsonValue);
    }

    public cxFirebaseDBSnapshot Child (string key) {

        if (ObjectValue.TryGetValue (key, out object value)) {
            //TODO type check!
            //  var dic = value as Dictionary<string, object>;
            var dic = ValueToDictionary (value);
            return new cxFirebaseDBSnapshot (Path + "/" + key, dic);
        } else {
            return new cxFirebaseDBSnapshot (Path + "/" + key, "");
        }
    }

    public IEnumerable<cxFirebaseDBSnapshot> Children {
        get {
            return ObjectValue != null ?
                ObjectValue.Select (x => new cxFirebaseDBSnapshot (Path + "/" + x.Key, ValueToDictionary (x.Value))) :
                new List<cxFirebaseDBSnapshot> ();
        }
    }

    Dictionary<string, object> ValueToDictionary (object value) {
        if (value == null) return null;
        else return JsonConvert.DeserializeObject<Dictionary<string, object>> (JsonConvert.SerializeObject (value));
    }
}

public class cxFirebaseDBRef {
    public string Key { get; private set; }
    public string Path { get; private set; }

    internal cxFirebaseDBRef (string path) {
        Path = path;
        Key = path.Split ('/').Last ();
    }

    public cxFirebaseDBRef Child (string childName) {
        return new cxFirebaseDBRef (string.Join ("/", Path, childName));
    }

    public cxFirebaseDBRef Push () {
        return new cxFirebaseDBRef (string.Join ("/", Path, FirebaseUtils.GenerateKey ()));
    }

    public static Dictionary<string, object> ConvertDictionary (object inputObj) {
        string jsonString = JsonConvert.SerializeObject (inputObj);
        Dictionary<string, object> inputDict = JsonConvert.DeserializeObject<Dictionary<string, object>> (jsonString);
        Dictionary<string, object> resultDict = new Dictionary<string, object> ();

        foreach (var item in inputDict) {
            if (item.Value == null)
                continue;

            if (item.Value is System.DateTime dateTimeValue) {
                // DateTime을 ISO 8601 문자열로 변환
                string isoDateString = dateTimeValue.ToString ("o"); // "o"는 ISO 8601 형식
                resultDict.Add (item.Key, isoDateString);
            } else {
                resultDict.Add (item.Key, item.Value);
            }
        }

        return resultDict;
    }
}


public abstract class cxIFirebaseDBDriver {
    public abstract cxFirebaseDBRef GetDataReference (string path);

    public abstract Task<cxFirebaseDBSnapshot> GetValueAsync (string path);
    
    public abstract Task SetValueAsync (string path, string jsonValue);
    public abstract Task SetValueAsync (string path, object value);
    public abstract Task UpdateValueAsync (string path, Dictionary<string, object> value);

    //public abstract Task UpdateValueAsync (string path, object value);
   // public abstract Task<cxFirebaseDBSnapshot> PushAsync (string path, string value);
    public abstract Task<cxFirebaseDBSnapshot> PushAsync (string path);

    public abstract Task RemoveAsync (string path);

    public abstract void AddValueChanged (string path, Action<cxFirebaseDBSnapshot> handler);
    public abstract void RemoveValueChanged (string path, Action<cxFirebaseDBSnapshot> handler);

    public abstract void AddChildAdded (string path, Action<cxFirebaseDBSnapshot> handler);
    public abstract void RemoveChildAdded (string path, Action<cxFirebaseDBSnapshot> handler);

    public abstract void AddChildRemoved (string path, Action<cxFirebaseDBSnapshot> handler);
    public abstract void RemoveChildRemoved (string path, Action<cxFirebaseDBSnapshot> handler);

    public abstract Task<cxFirebaseDBSnapshot> QueryEqual(string path, string key, string value);
}