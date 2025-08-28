using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Button))]
public class cxUIOnOffButton : MonoBehaviour {
    [SerializeField]
    private GameObject on;
    [SerializeField]
    private GameObject off;
    [SerializeField]
    private bool autoToggle = false;

    [SerializeField]
    private bool isOn = false;

    public bool IsOn => isOn;

    public UnityEngine.Events.UnityEvent<bool> onClick {get;private set;}= new UnityEngine.Events.UnityEvent<bool> ();


    private void Awake () {
        
        GetComponent<Button> ().onClick.AddListener (() => {
            if (autoToggle) {
                Toggle ();
            }

            onClick.Invoke (IsOn);
        });
    }

    private void OnValidate()
    {
        SetOn(isOn);
    }
    
    public void SetOn (bool isOn, bool notify = false) {
        this.isOn = isOn;
        on.gameObject.SetActive (isOn);
        off.gameObject.SetActive (!isOn);

        if(notify){
            onClick.Invoke (isOn);
        }
    }

    public void SetOnWithoutNotify (bool isOn) {
        SetOn(isOn, false);
    }

    public void Toggle () {
        SetOn (!IsOn);
    }

    public void SetLabel (string label) {
        var text = on.GetComponentInChildren<TMPro.TMP_Text> ();
        if(text !=null) text.text =label;

        text = off.GetComponentInChildren<TMPro.TMP_Text> ();
        if(text !=null) text.text =label;
    }
}