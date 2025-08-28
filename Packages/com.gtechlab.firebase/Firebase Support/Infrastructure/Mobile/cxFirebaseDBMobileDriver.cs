#if UNITY_EDITOR || !UNITY_WEBGL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Google.MiniJSON;
using Newtonsoft.Json;
using UnityEngine;

public class cxFirebaseDBMobileDriver : cxIFirebaseDBDriver {

    private class EventHandler {
        public string path;
        public Action<cxFirebaseDBSnapshot> callback;

        public EventHandler (string path, Action<cxFirebaseDBSnapshot> callback) {
            this.path = path;
            this.callback = callback;
        }

        public void OnValueChanged (object sender, ValueChangedEventArgs args) {
            var path = GetPath (args.Snapshot.Reference);
            //Debug.Log ("OnValueChanged: " + path + " " + args.Snapshot.GetRawJsonValue ());
            callback (new cxFirebaseDBSnapshot (path, args.Snapshot.GetRawJsonValue ()));
        }

        public void OnChildAdded (object sender, ChildChangedEventArgs args) {
            var path = GetPath (args.Snapshot.Reference, includeKey : false);

            Debug.Log ("OnChildAdded: " + path + " " + args.Snapshot.GetRawJsonValue ());
            callback (new cxFirebaseDBSnapshot (string.Join ("/", path, args.Snapshot.Key), args.Snapshot.GetRawJsonValue ()));
        }

        public void OnChildRemoved (object sender, ChildChangedEventArgs args) {
            var path = GetPath (args.Snapshot.Reference, includeKey : false);
            Debug.Log ("OnChildRemoved: " + path + " " + args.Snapshot.GetRawJsonValue ());

            callback (new cxFirebaseDBSnapshot (string.Join ("/", path, args.Snapshot.Key), args.Snapshot.GetRawJsonValue ()));
        }
    }

    private List<EventHandler> valueChangedHandlers = new List<EventHandler> ();
    private List<EventHandler> childAddedHandlers = new List<EventHandler> ();
    private List<EventHandler> childRemovedHandlers = new List<EventHandler> ();

    public cxFirebaseDBMobileDriver () {
    }

    static string GetPath (DatabaseReference reference, bool includeKey = true) {
        List<string> path = new List<string> ();

        if (includeKey) {
            path.Add (reference.Key);
        }

        while (reference.Parent != null) {
            reference = reference.Parent;
            path.Add (reference.Key);
        }

        path.Reverse ();
        return string.Join ("/", path);
    }

    public override cxFirebaseDBRef GetDataReference (string path) {
        path = !path.StartsWith ("/") ? "/" + path : path;

        return new cxFirebaseDBRef (path);
    }

    public override async Task<cxFirebaseDBSnapshot> GetValueAsync (string path) {

        var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);
        var snapshot = await dbRef.GetValueAsync ();
        return new cxFirebaseDBSnapshot (path, snapshot.GetRawJsonValue ());
    }

    public override async Task SetValueAsync (string path, string value) {
        var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);
        await dbRef.SetRawJsonValueAsync (value);
    }

    public override async Task SetValueAsync(string path, object value)
    {
        var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);
        await dbRef.SetRawJsonValueAsync (Newtonsoft.Json.JsonConvert.SerializeObject (value));
    }

    public override async Task UpdateValueAsync (string path, Dictionary<string, object> value) {
        var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);
        await dbRef.UpdateChildrenAsync (value);
    }

    // public override async Task UpdateValueAsync (string path, object value) {
    //     var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);
    //     var valueDict = FirebaseUtils.ToDictionary (value);
    //     await dbRef.UpdateChildrenAsync (valueDict);
    // }

    /*
        public override async Task<cxFirebaseDBSnapshot> PushAsync (string path, string value) {
            var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);
            var newRef = dbRef.Push ();
            await newRef.SetRawJsonValueAsync (value);
            return new cxFirebaseDBSnapshot (GetPath (newRef), value);
        }
    */

    public override Task<cxFirebaseDBSnapshot> PushAsync (string path) {
        var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);
        var newRef = dbRef.Push ();

        return Task.FromResult (new cxFirebaseDBSnapshot (GetPath (newRef), ""));
    }

    public override async Task RemoveAsync (string path) {
        var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);
        await dbRef.RemoveValueAsync ();
    }

    public override void AddValueChanged (string path, Action<cxFirebaseDBSnapshot> handler) {
        var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);
        var eventHandler = new EventHandler (path, handler);

        valueChangedHandlers.Add (eventHandler);
        dbRef.ValueChanged += eventHandler.OnValueChanged;
    }

    public override void RemoveValueChanged (string path, Action<cxFirebaseDBSnapshot> handler) {

        var eventHandler = valueChangedHandlers.Find (x => x.path == path && x.callback == handler);
        if (eventHandler != null) {
            valueChangedHandlers.Remove (eventHandler);
            var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);
            dbRef.ValueChanged -= eventHandler.OnValueChanged;
        }

    }

    public override void AddChildAdded (string path, Action<cxFirebaseDBSnapshot> handler) {
        var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);
        var eventHandler = new EventHandler (path, handler);

        childAddedHandlers.Add (eventHandler);
        dbRef.ChildAdded += eventHandler.OnChildAdded;
    }

    public override void RemoveChildAdded (string path, Action<cxFirebaseDBSnapshot> handler) {
        var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);

        var callback = childAddedHandlers.Find (x => x.path == path && x.callback == handler);
        if (callback != null) {
            childAddedHandlers.Remove (callback);
            dbRef.ChildAdded -= callback.OnChildAdded;
        }
    }

    public override void AddChildRemoved (string path, Action<cxFirebaseDBSnapshot> handler) {

        var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);
        var eventHandler = new EventHandler (path, handler);

        childRemovedHandlers.Add (eventHandler);
        dbRef.ChildRemoved += eventHandler.OnChildRemoved;
    }

    public override void RemoveChildRemoved (string path, Action<cxFirebaseDBSnapshot> handler) {
        var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);

        var callback = childRemovedHandlers.Find (x => x.path == path && x.callback == handler);
        if (callback != null) {
            childRemovedHandlers.Remove (callback);
            dbRef.ChildRemoved -= callback.OnChildRemoved;
        }
    }

    public override async Task<cxFirebaseDBSnapshot> QueryEqual(string path, string key, string value)
    {
        var dbRef = FirebaseDatabase.DefaultInstance.GetReference (path);
        var snapshot = await dbRef.OrderByChild(key).EqualTo(value).GetValueAsync();
        //var snapshot = await dbRef.OrderByChild(key).GetValueAsync();

        return new cxFirebaseDBSnapshot (path, snapshot.GetRawJsonValue());
    }
}

#endif