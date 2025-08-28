using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseDemoApp : MonoBehaviour
{
    public Button signInButton;
    public Button signOutButton;

    // Start is called before the first frame update
    void Start()
    {
        signInButton.onClick.AddListener(()=>{
            cxWebFirebaseInstance.Instance.SignInWithGoogle();
        });

         signOutButton.onClick.AddListener(()=>{
            cxWebFirebaseInstance.Instance.SignOut();
        });
    }

}
