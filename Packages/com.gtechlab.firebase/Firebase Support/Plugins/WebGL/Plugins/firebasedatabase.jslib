mergeInto(LibraryManager.library, {

    GetJSON: function(path, callbackId) {
        var parsedPath = UTF8ToString(path);

        try {

            firebase.database().ref(parsedPath).once('value').then(function(snapshot) {
                var snapshot = {
                    key: snapshot.key,
                    value: snapshot.val()
                }

                console.log("GetJSON: "+parsedPath + " snapshot:" + JSON.stringify(snapshot));
                Module.CallManagedCallback(callbackId, JSON.stringify(snapshot));
            });

        } catch (error) {
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

     QueryEqual: function(path, key, value, callbackId) {
        var parsedPath = UTF8ToString(path);
        var parsedKey =  UTF8ToString(key);
        var parsedValue = UTF8ToString(value);
        try {

            console.log("QueryEqual parsedPath:"+parsedPath +", parsedKey:"+parsedKey+", parsedValue:"+parsedValue)

            firebase.database().ref(parsedPath).orderByChild(parsedKey).equalTo(parsedValue).once('value').then(function(snapshot) {
                 console.log("QueryEqual: "+parsedPath + " snapshot:" + JSON.stringify(snapshot));

                //  if(snapshot) {
                //         // var list = []
                //         // snapshot.forEach((childSnapshot) => {
                //         //   list.push(childSnapshot)
                //         //     });

                //         Module.CallManagedCallback(callbackId, JSON.stringify(snapshot));
                //     }

                var snapshot = {
                    key: snapshot.key,
                    value: snapshot.val()
                }
                // console.log("QueryEqual: "+parsedPath + " snapshot:" + JSON.stringify(snapshot));
                Module.CallManagedCallback(callbackId, JSON.stringify(snapshot));
            });

        } catch (error) {
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    

    PostJSON: function(path, value,callbackId) {
        var parsedPath = UTF8ToString(path);
        var parsedValue = UTF8ToString(value);

        try {

            firebase.database().ref(parsedPath).set(JSON.parse(parsedValue)).then(function(unused) {

                console.log("PostJSON: "+parsedPath + " snapshot:" + JSON.stringify(unused));

                Module.CallManagedCallback(callbackId,unused);
            });

        } catch (error) {
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

/*
    PushJSON: function(path, value, callbackId) {
        var parsedPath = UTF8ToString(path);
        var parsedValue = UTF8ToString(value);

        try {

            firebase.database().ref(parsedPath).push().set(JSON.parse(parsedValue)).then(function(unused) {
                console.log("PushJSON: "+parsedPath + " snapshot:" + JSON.stringify(unused));

                Module.CallManagedCallback(callbackId,unused);
            });

        } catch (error) {
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },
*/

    Push: function(path, callbackId) {
        var parsedPath = UTF8ToString(path);

        try {
            var Ref = firebase.database().ref(parsedPath).push();
            Module.CallManagedCallback(callbackId,Ref.Key);
            
        } catch (error) {
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },


    UpdateJSON: function(path, value, callbackId) {
        var parsedPath = UTF8ToString(path);
        var parsedValue = UTF8ToString(value);

        try {

            firebase.database().ref(parsedPath).update(JSON.parse(parsedValue)).then(function(unused) {
                console.log("UpdateJSON: "+parsedPath + " snapshot:" + JSON.stringify(unused));

                Module.CallManagedCallback(callbackId,unused);
            });

        } catch (error) {
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    DeleteJSON: function(path, callbackId) {
        var parsedPath = UTF8ToString(path);

        try {

            firebase.database().ref(parsedPath).remove().then(function(unused) {
                console.log("DeleteJSON: "+parsedPath + " snapshot:" + JSON.stringify(unused));

                Module.CallManagedCallback(callbackId,unused);
            });

        } catch (error) {
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    ListenForValueChanged: function(path, callbackId) {
        var parsedPath = UTF8ToString(path);

        try {

            firebase.database().ref(parsedPath).on('value', function(snapshot) {
                 var snapshot = {
                    key: snapshot.key,
                    value: snapshot.val()
                }

                console.log("ListenForValueChanged: "+parsedPath + " value:" + JSON.stringify(snapshot));
                Module.CallManagedCallback(callbackId, JSON.stringify(snapshot));
            });

        } catch (error) {
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    StopListeningForValueChanged: function(path, callbackId) {
        var parsedPath = UTF8ToString(path);

        try {
            firebase.database().ref(parsedPath).off('value');
            Module.CallManagedCallback(callbackId,"");
        } catch (error) {
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    ListenForChildAdded: function(path, callbackId) {
        var parsedPath = UTF8ToString(path);

        try {

            firebase.database().ref(parsedPath).on('child_added', function(snapshot) {

                 var snapshot = {
                    key: snapshot.key,
                    value: snapshot.val()
                }

                console.log("ListenForChildAdded: "+parsedPath + " value:" + JSON.stringify(snapshot));
                Module.CallManagedCallback(callbackId, JSON.stringify(snapshot));
            });

        } catch (error) {
            //window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    StopListeningForChildAdded: function(path, callbackId) {
        var parsedPath = UTF8ToString(path);
        // var parsedObjectName = UTF8ToString(objectName);
        // var parsedCallback = UTF8ToString(callback);
        // var parsedFallback = UTF8ToString(fallback);

        try {
            firebase.database().ref(parsedPath).off('child_added');
            Module.CallManagedCallback(callbackId,"");
        } catch (error) {
           // window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    ListenForChildChanged: function(path, callbackId) {
        var parsedPath = UTF8ToString(path);
        // var parsedObjectName = UTF8ToString(objectName);
        // var parsedCallback = UTF8ToString(callback);
        // var parsedFallback = UTF8ToString(fallback);

        try {

            firebase.database().ref(parsedPath).on('child_changed', function(snapshot) {
                 var snapshot = {
                    key: snapshot.key,
                    value: snapshot.val()
                }

                console.log("ListenForChildChanged: "+parsedPath + " value:" + JSON.stringify(snapshot));
                Module.CallManagedCallback(callbackId, JSON.stringify(snapshot));
            });

        } catch (error) {
            //window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    StopListeningForChildChanged: function(path, callbackId) {
        var parsedPath = UTF8ToString(path);
        // var parsedObjectName = UTF8ToString(objectName);
        // var parsedCallback = UTF8ToString(callback);
        // var parsedFallback = UTF8ToString(fallback);

        try {
            firebase.database().ref(parsedPath).off('child_changed');
            //window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "Success: listener removed");
            Module.CallManagedCallback(callbackId,"");
        } catch (error) {
            //window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    ListenForChildRemoved: function(path,callbackId) {
        var parsedPath = UTF8ToString(path);
        // var parsedObjectName = UTF8ToString(objectName);
        // var parsedCallback = UTF8ToString(callback);
        // var parsedFallback = UTF8ToString(fallback);

        try {

            firebase.database().ref(parsedPath).on('child_removed', function(snapshot) {
                 var snapshot = {
                    key: snapshot.key,
                    value: snapshot.val()
                }

                console.log("ListenForChildRemoved: "+parsedPath + " value:" + JSON.stringify(snapshot));
                Module.CallManagedCallback(callbackId, JSON.stringify(snapshot));
            });

        } catch (error) {
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    StopListeningForChildRemoved: function(path, callbackId) {
        var parsedPath = UTF8ToString(path);
        // var parsedObjectName = UTF8ToString(objectName);
        // var parsedCallback = UTF8ToString(callback);
        // var parsedFallback = UTF8ToString(fallback);

        try {
            firebase.database().ref(parsedPath).off('child_removed');
            Module.CallManagedCallback(callbackId,"");
        } catch (error) {
           Module. CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    ModifyNumberWithTransaction: function(path, amount,callbackId) {
        var parsedPath = UTF8ToString(path);
        // var parsedObjectName = UTF8ToString(objectName);
        // var parsedCallback = UTF8ToString(callback);
        // var parsedFallback = UTF8ToString(fallback);

        try {

            firebase.database().ref(parsedPath).transaction(function(currentValue) {
                if (!isNaN(currentValue)) {
                    return currentValue + amount;
                } else {
                    return amount;
                }
            }).then(function(unused) {
              //  window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "Success: transaction run in " + parsedPath);
                Module.CallManagedCallback(callbackId,unused);
            });

        } catch (error) {
           // window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    ToggleBooleanWithTransaction: function(path,callbackId) {
        var parsedPath = UTF8ToString(path);
        // var parsedObjectName = UTF8ToString(objectName);
        // var parsedCallback = UTF8ToString(callback);
        // var parsedFallback = UTF8ToString(fallback);

        try {

            firebase.database().ref(parsedPath).transaction(function(currentValue) {
                if (typeof currentValue === "boolean") {
                    return !currentValue;
                } else {
                    return true;
                }
            }).then(function(unused) {
                //window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "Success: transaction run in " + parsedPath);
                Module.CallManagedCallback(callbackId,unused);
            });

        } catch (error) {
           // window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    // QueryEqual : function(path, key, value , callbackId) {
    //     var parsedPath = UTF8ToString(path);
    //     var parsedKey =  UTF8ToString(key);
    //     var parsedValue =  UTF8ToString(value);


    //     try {
    //             var query = firebase.database().ref(parsedPath).orderByKey(parsedKey).equalTo(parsedValue);
    //             query.get().then(snapshot => {
    //                 if(snapshot) {
    //                     var list = []
    //                     snapshot.forEach((childSnapshot) => {
    //                       list.pus(childSnapshot)
    //                         });

    //                     Module.CallManagedCallback(callbackId, JSON.stringify(list));
    //                 }
    //             });

    //     } catch (error) {
    //        // window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
    //         Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
    //     }

    // }

});