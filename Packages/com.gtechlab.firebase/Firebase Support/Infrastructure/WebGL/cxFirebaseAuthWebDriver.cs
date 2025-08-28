using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using Newtonsoft.Json;
using UnityEngine;


/// JSON Format
// {"displayName":"Grayson L",
// "email":"e.jongok@gmail.com",
// "isAnonymous":false,
// "isEmailVerified":false,
// "metadata":{"lastSignInTimestamp":0,"creationTimestamp":0},
// "phoneNumber":null,
// "providerData":[
// {"displayName":"Grayson L","email":"e.jongok@gmail.com","photoUrl":null,"providerId":"google.com","userId":null}
// ]
// "providerId":null,
// "uid":"SHHU7J1i6xONvhzEtNierP7s7B63"}
/// 

public class cxFirebaseAuthWebGLDriver : cxIFirebaseAuthDriver {

    public override async Task<TSocialUserModel> HasAuthSession () {

        var user = await FirebaseAuth.CheckCurrentAuthUser ();
        if (user == null) {
            return null;
        } else {
            //Debug
           // Debug.Log ("Debug HasAuthSession: " + JsonConvert.SerializeObject (user));

            string providerId = user.providerId;
            if(providerId == null && user.providerData.Length > 0) {
                providerId = user.providerData[0].providerId;
            }

            Debug.Log ("Debug providerId: " + providerId);
            
            //TODO
            TSocialType socialType = FirebaseUtils.ProviderIdToSocialType (providerId);

            return new TSocialUserModel {
                nickname = user.displayName,
                    email = user.email,
                    socialKey = user.uid,
                    socialType = socialType,
                    photoURL = user.providerData.Length > 0 ? user.providerData[0].photoUrl : string.Empty
            };
        }
    }

    public override async Task<TSocialUserModel> SignInAnonymously () {
        try {
            var user = await FirebaseAuth.SignInAnonymously ();
            return new TSocialUserModel {
                nickname = user.displayName,
                    email = user.email,
                    socialKey = user.uid,
                    socialType = TSocialType.Guest,
                    photoURL = user.providerData.Length > 0 ? user.providerData[0].photoUrl : string.Empty
            };
        } catch (Exception e) {
            throw HandleException (e);
        }
    }

    public override async Task<TSocialUserModel> CreateUserWithEmailAndPassword (string email, string password) {
        try {
            var user = await FirebaseAuth.CreateUserWithEmailAndPassword (email, password);
            return new TSocialUserModel {
                nickname = user.displayName,
                    email = user.email,
                    socialKey = user.uid,
                    socialType = TSocialType.EMail,
                    photoURL = user.providerData.Length > 0 ? user.providerData[0].photoUrl : string.Empty
            };
        } catch (Exception e) {
            throw HandleException (e);
        }
    }

    public override async Task<TSocialUserModel> SignInWithEmailAndPassword (string email, string password) {
        try {
            var user = await FirebaseAuth.SignInWithEmailAndPassword (email, password);
            return new TSocialUserModel {
                nickname = user.displayName,
                    email = user.email,
                    socialKey = user.uid,
                    socialType = TSocialType.EMail,
                    photoURL = user.providerData.Length > 0 ? user.providerData[0].photoUrl : string.Empty
            };
        } catch (Exception e) {
            throw HandleException (e);
        }
    }


    public  override async Task SendPasswordResetEmail (string email) {
        try {
            await FirebaseAuth.SendPasswordResetEmail(email);
            //throw new Exception("Not implemented");

        }  catch (Exception e) {
            throw HandleException (e);
        }
    }

    public override async Task<TSocialUserModel> SignInWithGoogle () {
        try {
            var user = await FirebaseAuth.SignInWithGoogle ();
            return new TSocialUserModel {
                nickname = user.displayName,
                    email = user.email,
                    socialKey = user.uid,
                    socialType = TSocialType.Google,
                    photoURL = user.providerData.Length > 0 ? user.providerData[0].photoUrl : string.Empty
            };
        } catch (Exception e) {
            throw HandleException (e);
        }
    }

    public override async Task SignOut () {
        try {
            FirebaseAuth.SignOut ();
        } catch (Exception e) {
            throw HandleException (e);
        }
    }


    private Exception HandleException (Exception e) {
        try {
            var firebaseError = Newtonsoft.Json.JsonConvert.DeserializeObject<FirebaseError> (e.Message);
            return new cxBlocException (0, firebaseError.message);
        } catch (Exception) {
            return e;
        }
    }
}