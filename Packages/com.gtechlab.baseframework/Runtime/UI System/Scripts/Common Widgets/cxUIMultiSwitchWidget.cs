using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class cxUIMultiSwitchWidget : MonoBehaviour {
    [SerializeField]
    private List<cxUIOnOffButton> buttons;

    // [SerializeField]
    // private bool allowAllOff = false;

    [SerializeField]
    private int selectedNo = 0;

    public UnityEvent<int> OnSelected { get; private set; } = new UnityEvent<int> ();

    void Awake () {
        for (int i = 0; i < buttons.Count; i++) {
            int buttonNo = i;
            buttons[buttonNo].SetOn (selectedNo == buttonNo);
            buttons[buttonNo].onClick.AddListener ((on) => {
               OnSelect (buttonNo);
            });
        }
    }

    public void SwitchButton (int buttonNo) {
        selectedNo = buttonNo;
        for (int i = 0; i < buttons.Count; i++) {
            buttons[i].SetOn (selectedNo == i);
        }
    }

    void OnSelect (int index) {
        bool changed = selectedNo !=index;

        selectedNo = index;
        for (int i = 0; i < buttons.Count; i++) {
            buttons[i].SetOn (selectedNo == i);
        }

        if(changed)
            OnSelected.Invoke (index);
    }

    void OnValidate () {
        for (int i = 0; i < buttons.Count; i++) {
            int buttonNo = i;
            buttons[buttonNo].SetOn (selectedNo == buttonNo);
        }
    }

}