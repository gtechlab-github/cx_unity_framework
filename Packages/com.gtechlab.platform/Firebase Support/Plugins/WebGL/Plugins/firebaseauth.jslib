mergeInto(LibraryManager.library, {

    CheckCurrentAuthUser: function(callbackId) {
        var user = firebase.auth().currentUser;

        if (user) {
            // 사용자가 로그인되어 있음
            Module.CallManagedCallback(callbackId,  JSON.stringify(user));
        } else {
            // 로그인된 사용자가 없음
            Module.CallManagedCallback(callbackId, "");
        }
    },

    SignInAnonymously: function (callbackId) {

        try {
            firebase.auth().signInAnonymously().then(function (result) {
                Module.CallManagedCallback(callbackId, JSON.stringify(result.user));
                //window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "Success: signed up for " + result);
            }).catch(function (error) {
                Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                //window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });

        } catch (error) {
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },
    
    CreateUserWithEmailAndPassword: function (email, password, callbackId) {
        var parsedEmail = UTF8ToString(email);
        var parsedPassword = UTF8ToString(password);

        try {

            firebase.auth().createUserWithEmailAndPassword(parsedEmail, parsedPassword).then(function (result) {
                Module.CallManagedCallback(callbackId, JSON.stringify(result.user));
            }).catch(function (error) {
                Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });

        } catch (error) {
           // window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    SignInWithEmailAndPassword: function (email, password, callbackId) {
        var parsedEmail = UTF8ToString(email);
        var parsedPassword = UTF8ToString(password);

        try {

            firebase.auth().signInWithEmailAndPassword(parsedEmail, parsedPassword).then(function (result) {
                //window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "Success: signed in for " + parsedEmail);
                Module.CallManagedCallback(callbackId,JSON.stringify(result.user));
            }).catch(function (error) {
                //window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });

        } catch (error) {
           // window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    SignInWithGoogle: function (callbackId) {

        try {
            var provider = new firebase.auth.GoogleAuthProvider();
            firebase.auth().signInWithPopup(provider).then(function (result) {
                //window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "Success: signed in with Google!");
                Module.CallManagedCallback(callbackId,JSON.stringify(result.user));
            }).catch(function (error) {
                // window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });

        } catch (error) {
            //window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    SendPasswordResetEmail: function (email, callbackId) {
        var parsedEmail = UTF8ToString(email);
        firebase.auth().sendPasswordResetEmail(parsedEmail).then(function (result) {
            Module.CallManagedCallback(callbackId, JSON.stringify(result));
        }).catch(function (error) {
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        });
    },

//  firebase.auth().signInWithPopup(provider)
//       .then((result) => {
//         var jstr = JSON.stringify(result);
//         console.log("firebase.auth.signIn done:" + jstr);

//         cxGameInstance.SendMessage('cxWebFirebaseInstance', 'OnSignInCompleted', jstr);
//       }).catch((error) => {
//         var errorStr = JSON.stringify(error);
//         console.log("firebase.auth.signIn error:" + errorStr);

//         cxGameInstance.SendMessage('cxWebFirebaseInstance', 'OnSignInError', errorStr);
//       });

    SignInWithFacebook: function (callbackId) {

        try {
            var provider = new firebase.auth.FacebookAuthProvider();
            firebase.auth().signInWithRedirect(provider).then(function (result) {
               // window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "Success: signed in with Facebook!");
                Module.CallManagedCallback(callbackId,JSON.stringify(result.user));
            }).catch(function (error) {
                //window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
                Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            });

        } catch (error) {
            //window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
            Module.CallManagedFallback(callbackId, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
    },

    OnAuthStateChanged: function (callbackId) {

        firebase.auth().onAuthStateChanged(function(user) {
            if (user) {
                //window.unityInstance.SendMessage(parsedObjectName, parsedOnUserSignedIn, JSON.stringify(user));
                Module.CallManagedCallback(callbackId, JSON.stringify(user));
            } else {
                //window.unityInstance.SendMessage(parsedObjectName, parsedOnUserSignedOut, "User signed out");
                Module.CallManagedCallback(callbackId, "");
            }
        });
    },

    SignOut: function () {
        firebase.auth().signOut();
    }
});
