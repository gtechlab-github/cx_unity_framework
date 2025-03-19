#if UNITY_EDITOR || !UNITY_WEBGL

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

public class cxFirebaseAuthMobileDriver : cxIFirebaseAuthDriver {

    public bool isFirebaseInitialized { get; private set; } = false;
    public FirebaseUser currentUser { get; private set; }

    public cxFirebaseAuthMobileDriver (bool forEditor=false) {
        if(forEditor) {
            InitFirebaseForEditor();
        } else {
            InitFirebase ();
        }
    }

   async void InitFirebaseForEditor(){
        FirebaseApp firebaseApp = FirebaseApp.Create (
                FirebaseApp.DefaultInstance.Options,
                "FIREBASE_EDITOR");
   }

    async void InitFirebase () {
        // FirebaseApp firebaseApp = FirebaseApp.Create (
        //     FirebaseApp.DefaultInstance.Options,
        //     "FIREBASE_EDITOR");

        try {
            var result = await FirebaseApp.CheckAndFixDependenciesAsync ();
            if (result == DependencyStatus.Available) {
                Debug.Log ("Firebase dependencies are resolved.");
                var app = Firebase.FirebaseApp.DefaultInstance;
                isFirebaseInitialized = true;
            } else {
                Debug.LogError (
                    "Could not resolve all Firebase dependencies: " + result);
            }

        } catch (System.Exception e) {
            Debug.LogError (e);
        }
    }

    public override async Task<TSocialUserModel> HasAuthSession () {
        var auth = FirebaseAuth.GetAuth (FirebaseApp.DefaultInstance);
        currentUser = auth.CurrentUser;
        if (currentUser != null) {
            Debug.Log ("User session exist: " + currentUser.UserId);
            TSocialType socialType = FirebaseUtils.ProviderIdToSocialType (currentUser.ProviderId);

            return new TSocialUserModel {
                nickname = currentUser.DisplayName,
                    email = currentUser.Email,
                    socialKey = currentUser.UserId,
                    socialType = socialType,
                    photoURL = currentUser.PhotoUrl?.ToString ()
            };
        } else {
            Debug.Log ("No user signed in.");
            return null;
        }
    }

    public override async Task<TSocialUserModel> SignInWithEmailAndPassword (string email, string password) {
        try {
            var auth = FirebaseAuth.GetAuth (FirebaseApp.DefaultInstance);
            var result = await auth.SignInWithEmailAndPasswordAsync (email, password);

            currentUser = result.User;
            Debug.Log ("User signed in successfully: " + currentUser.UserId);

            return new TSocialUserModel {
                nickname = currentUser.DisplayName,
                    email = currentUser.Email,
                    socialKey = currentUser.UserId,
                    socialType = TSocialType.EMail,
                    photoURL = currentUser.PhotoUrl?.ToString ()
            };
        } catch (TaskCanceledException e) {
            throw new cxBlocException (0, "SignIn has been cancelled.");
        } catch (FirebaseException e) {
            throw HandleFirebaseAuthError (e);
        } catch (System.Exception e) {
            Debug.LogException (e);
            throw e;
        }
    }


    public  override async Task ResetEmailPassword () {
        try {
            
            var auth = FirebaseAuth.GetAuth (FirebaseApp.DefaultInstance);
            if(currentUser == null || currentUser.Email == null) {
                throw new cxBlocException (0, "No user signed in.");
            }

            var email = currentUser.Email;
            await auth.SendPasswordResetEmailAsync(email);

        } catch (TaskCanceledException e) {
            throw new cxBlocException (0, "SignIn has been cancelled.");
        } catch (FirebaseException e) {
            throw HandleFirebaseAuthError (e);
        } catch (System.Exception e) {
            Debug.LogException (e);
            throw e;
        }
    }

    // Firebase Authentication 오류 처리
    private cxBlocException HandleFirebaseAuthError (FirebaseException exception) {
        AuthError errorCode = (AuthError) exception.ErrorCode;

        switch (errorCode) {
            case AuthError.InvalidEmail:
                return new cxBlocException (0, "Invalid email format.");
            case AuthError.WrongPassword:
                return new cxBlocException (0, "Incorrect password.");
            case AuthError.UserNotFound:
                return new cxBlocException (0, "No user found with this email.");
            case AuthError.NetworkRequestFailed:
                return new cxBlocException (0, "Network error. Please check your connection.");
            case AuthError.EmailAlreadyInUse:
                return new cxBlocException (0, "The email is already in use by another account.");
            case AuthError.WeakPassword:
                return new cxBlocException (0, "The password is too weak.");
            default:
                return new cxBlocException (0, "Unknown error occurred: " + exception.Message);
        }
    }

    public override async Task<TSocialUserModel> CreateUserWithEmailAndPassword (string email, string password) {
        try {
            var auth = FirebaseAuth.GetAuth (FirebaseApp.DefaultInstance);
            var result = await auth.CreateUserWithEmailAndPasswordAsync (email, password);

            currentUser = result.User;
            Debug.Log ("User created in successfully: " + currentUser.UserId);

            return new TSocialUserModel {
                nickname = currentUser.DisplayName,
                    email = currentUser.Email,
                    socialKey = currentUser.UserId,
                    socialType = TSocialType.EMail,
                    photoURL = currentUser.PhotoUrl?.ToString ()
            };
        } catch (TaskCanceledException e) {
            throw new cxBlocException (0, "CreateUser has been cancelled.");
        } catch (FirebaseException e) {
            throw HandleFirebaseAuthError (e);
        } catch (System.Exception e) {
            Debug.LogException (e);
            throw e;
        }
    }

    public override async Task<TSocialUserModel> SignInWithGoogle () {
        throw new System.NotImplementedException ();
        /*
        try {
            var auth = FirebaseAuth.GetAuth (FirebaseApp.DefaultInstance);

            var result = await auth.SignInWithCredentialAsync ();
            currentUser = result.User;

            Debug.Log ("User signed in successfully: " + currentUser.UserId);

            return new cxFirebaseUser {
                displayName = currentUser.DisplayName,
                    email = currentUser.Email,
                    uid = currentUser.UserId,
                    isAnonymous = currentUser.IsAnonymous,
                    providerId = currentUser.ProviderId,
                    photoUrl = currentUser.PhotoUrl.ToString ()
            };
        } catch (TaskCanceledException ex) {
            throw new cxBlocException (0, "SignIn has been cancelled.");
        } catch (SignInException ex) {
            Debug.LogException (ex);
            Debug.LogWarningFormat ("SignInException statusCode: {0} {1} {2}", ex.Status, ex.Message, ex.StackTrace);

            throw new cxBlocException (0, ex.Message);
        } catch (Exception ex) {
            Debug.LogException (ex);
            throw new cxBlocException (0, ex.Message);
        }
        */
    }

    public override async Task<TSocialUserModel> SignInAnonymously () {
        try {
            var auth = FirebaseAuth.GetAuth (FirebaseApp.DefaultInstance);
            var result = await auth.SignInAnonymouslyAsync ();

            currentUser = result.User;
            Debug.Log ("User created in successfully: " + currentUser.UserId);

            return new TSocialUserModel {
                nickname = currentUser.DisplayName,
                    email = currentUser.Email,
                    socialKey = currentUser.UserId,
                    socialType = TSocialType.Guest,
                    photoURL = currentUser.PhotoUrl.ToString ()
            };
        } catch (TaskCanceledException e) {
            throw new cxBlocException (0, "SignIn has been cancelled.");
        } catch (FirebaseException e) {
            throw HandleFirebaseAuthError (e);
        } catch (System.Exception e) {
            Debug.LogException (e);
            throw e;
        }
    }

    public override async Task SignOut () {
        var auth = FirebaseAuth.GetAuth (FirebaseApp.DefaultInstance);
         auth.SignOut();
    }
}

#endif