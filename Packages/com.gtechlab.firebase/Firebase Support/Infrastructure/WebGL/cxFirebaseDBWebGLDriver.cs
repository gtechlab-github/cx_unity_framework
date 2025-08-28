using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseWebGL.Scripts.FirebaseBridge;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class cxFirebaseDBWebGLDriver : cxIFirebaseDBDriver {

    [Serializable]
    public class Snapshot {
        public string key;
        public object value;
        public string jsonValue;

        public static Snapshot ToSnapshot (string json) {
            var snapshot = Newtonsoft.Json.JsonConvert.DeserializeObject<Snapshot> (json);
            snapshot.jsonValue = JsonConvert.SerializeObject (snapshot.value);
            return snapshot;
        }
    }

    private class EventHandler {
        public string path;
        public Action<cxFirebaseDBSnapshot> callback;

        public EventHandler (string path, Action<cxFirebaseDBSnapshot> callback) {
            this.path = path;
            this.callback = callback;
        }

        public void OnChildAdded (string json) {
            var snapshot = Snapshot.ToSnapshot (json);
            var n = new cxFirebaseDBSnapshot (string.Join ("/", path, snapshot.key), snapshot.jsonValue);
            callback (n);
        }

        public void OnChildRemoved (string json) {
            var snapshot = Snapshot.ToSnapshot (json);
            callback (new cxFirebaseDBSnapshot (string.Join ("/", path, snapshot.key), snapshot.jsonValue));
        }

        public void OnValueChanged (string json) {
            var snapshot = Snapshot.ToSnapshot (json);
            callback (new cxFirebaseDBSnapshot (path, snapshot.jsonValue));
        }
    }

    private List<EventHandler> valueChangedHandlers = new List<EventHandler> ();
    private List<EventHandler> childAddedHandlers = new List<EventHandler> ();
    private List<EventHandler> childRemovedHandlers = new List<EventHandler> ();

    public override cxFirebaseDBRef GetDataReference (string path) {
        path = !path.StartsWith ("/") ? "/" + path : path;
        return new cxFirebaseDBRef (path);
    }

    public override async Task<cxFirebaseDBSnapshot> GetValueAsync (string path) {
        var json = await FirebaseDatabase.GetJSON (path);
        var snapshot = Snapshot.ToSnapshot (json);
        var n = new cxFirebaseDBSnapshot (path, snapshot.jsonValue);
        return n;
    }


    public override async Task<cxFirebaseDBSnapshot> QueryEqual(string path, string key, string value)
    {
        var json = await FirebaseDatabase.QueryEqual(path, key, value);
        var snapshot = Snapshot.ToSnapshot (json);
        var n = new cxFirebaseDBSnapshot (path, snapshot.jsonValue);
        return n;
    }

    public override async Task<cxFirebaseDBSnapshot> PushAsync (string path) {
        var newKey = await FirebaseDatabase.Push (path);
        return new cxFirebaseDBSnapshot (path + "/" + newKey, "");
    }

    public override Task RemoveAsync (string path) {
        return FirebaseDatabase.DeleteJSON (path);
    }

    public override Task SetValueAsync (string path, string value) {
        return FirebaseDatabase.PostJSON (path, value);
    }
    public override Task SetValueAsync(string path, object value)
    {
        return FirebaseDatabase.PostJSON (path, JsonConvert.SerializeObject (value));
    }

    public override Task UpdateValueAsync (string path, Dictionary<string, object> value) {
        return FirebaseDatabase.UpdateJSON (path, JsonConvert.SerializeObject (value));
    }

    // public override Task UpdateValueAsync (string path, object value) {
    //     return FirebaseDatabase.UpdateJSON (path, JsonConvert.SerializeObject (value));
    // }

    public override void AddChildAdded (string path, Action<cxFirebaseDBSnapshot> handler) {

        var eventHandler = new EventHandler (path, handler);
        childAddedHandlers.Add (eventHandler);
        FirebaseDatabase.ListenForChildAdded (path, eventHandler.OnChildAdded);
    }

    public override void RemoveChildAdded (string path, Action<cxFirebaseDBSnapshot> handler) {

        var eventHandler = childAddedHandlers.Find (x => x.path == path && x.callback == handler);
        if (eventHandler != null) {
            childAddedHandlers.Remove (eventHandler);
            FirebaseDatabase.StopListeningForChildAdded (path, eventHandler.OnChildAdded);
        }
    }

    public override void AddValueChanged (string path, Action<cxFirebaseDBSnapshot> handler) {

        var eventHandler = new EventHandler (path, handler);
        valueChangedHandlers.Add (eventHandler);
        FirebaseDatabase.ListenForValueChanged (path, eventHandler.OnValueChanged);
    }

    public override void RemoveValueChanged (string path, Action<cxFirebaseDBSnapshot> handler) {
        var eventHandler = valueChangedHandlers.Find (x => x.path == path && x.callback == handler);
        if (eventHandler != null) {
            valueChangedHandlers.Remove (eventHandler);
            FirebaseDatabase.StopListeningForValueChanged (path, eventHandler.OnValueChanged);
        }
    }

    public override void AddChildRemoved (string path, Action<cxFirebaseDBSnapshot> handler) {
        var eventHandler = new EventHandler (path, handler);
        childRemovedHandlers.Add (eventHandler);
        FirebaseDatabase.ListenForChildRemoved (path, eventHandler.OnChildRemoved);
    }

    public override void RemoveChildRemoved (string path, Action<cxFirebaseDBSnapshot> handler) {
        var eventHandler = childRemovedHandlers.Find (x => x.path == path && x.callback == handler);
        if (eventHandler != null) {
            childRemovedHandlers.Remove (eventHandler);
            FirebaseDatabase.StopListeningForChildRemoved (path, eventHandler.OnChildRemoved);
        }
    }

}