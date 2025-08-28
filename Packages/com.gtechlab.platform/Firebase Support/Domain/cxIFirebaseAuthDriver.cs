using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


public abstract class cxIFirebaseAuthDriver {

    public abstract Task<TSocialUserModel> HasAuthSession ();
    public abstract Task<TSocialUserModel> SignInAnonymously();
    public abstract Task<TSocialUserModel> CreateUserWithEmailAndPassword (string email, string password);
    public abstract Task<TSocialUserModel> SignInWithEmailAndPassword (string email, string password);
    public abstract Task<TSocialUserModel> SignInWithGoogle ();
    public abstract Task SignOut ();
    public abstract Task SendPasswordResetEmail (string email);
}