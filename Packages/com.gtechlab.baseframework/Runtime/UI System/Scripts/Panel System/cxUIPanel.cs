using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (cxUITweenPanel))]
public abstract class cxUIPanel : MonoBehaviour {

    [SerializeField]
    private cxUITweenPanel panel;

    private bool initialized = false;
    private bool activating = false;

    public bool IsOpened => activating;

    void Awake () {
        if (!initialized) {
            initialized = true;
            panel.Hide (true);
            OnInit ();
        }
    }

    public void OpenPanel (object args = null) {
        if (!initialized) {
            initialized = true;
            OnInit ();
        }

        if (activating)
            OnDeactivated ();
            
        panel.Play ();
        OnActivated (args);
        activating = true;
    }

    public void ClosePanel () {
        if (initialized)
            OnDeactivated ();
        
        panel.Hide ();
        activating = false;
    }

    protected abstract void OnInit ();
    protected abstract void OnActivated (object args);
    protected abstract void OnDeactivated ();
}
