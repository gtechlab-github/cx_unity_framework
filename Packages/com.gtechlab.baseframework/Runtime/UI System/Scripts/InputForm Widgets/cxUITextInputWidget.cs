using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using WebGLSupport;

public class cxUITextInputEvent : UnityEvent<string> { }

[RequireComponent (typeof (cxUITouchDelegator))]
[Tooltip ("Editable Text Widget")]
public class cxUITextInputWidget : cxUIInputWidget {
    [SerializeField]
    [Tooltip ("레이블(옵션)")]
    private TMPro.TMP_Text labelText;

    [SerializeField]
    private TMPro.TMP_InputField textInput;
    [SerializeField]
    private TMPro.TMP_Text valueText;
    [SerializeField]
    private cxUITouchDelegator touchDelegator;
    [SerializeField]
    public Button sumitButton;
    [SerializeField] private string text;
   // [SerializeField] private string hintText;

    public cxUITextInputEvent OnInputEvent { get; private set; } = new cxUITextInputEvent ();

    //Note. WebGL Input Sync를 위한 임시 장치!
    public Action<TMPro.TMP_InputField> OnInputEditHook { get; set; }

    public string Value => text;

    private bool inputMode = false;

    private void Awake () {

        textInput.onEndEdit.AddListener (OnSubmit);
        textInput.onSubmit.AddListener (OnSubmit);
        textInput.onValueChanged.AddListener (OnTextChanged);
        touchDelegator.onPointerDoubleClick.AddListener (OnChangeInputMode);
        sumitButton?.onClick.AddListener (OnSubmitButton);
    }

    [Obsolete ("Use SetText instead")]
    public void SetValue (string text, bool inputMode = false) {
        SetText (text, inputMode);
    }

    public void SetText (string text, bool inputMode = false) {
        this.text = text;
        valueText.text = text;
        textInput.SetTextWithoutNotify (text);

        textInput.gameObject.SetActive (inputMode);
        sumitButton?.gameObject.SetActive (inputMode);
        valueText.gameObject.SetActive (!inputMode);

        cxUISystemUtil.RefreshLayout (transform);

        this.inputMode = inputMode;
    }

    public void SetLabel (string title) {
        if (labelText != null)
            labelText.text = title;

        cxUISystemUtil.RefreshLayout (transform);
    }

    public void SetInputHint (string hint) {
       /// hintText = hint;
        var text = textInput.placeholder.GetComponent<TMPro.TMP_Text> ();
        if (text != null)
            text.text = hint;
    }

    public void SetFontSize (int fontSize) {
        textInput.textComponent.fontSize = fontSize;
        textInput.placeholder.GetComponent<TMPro.TMP_Text> ().fontSize = fontSize;
        valueText.fontSize = fontSize;
    }

    public void SetTextAlignment (TMPro.TextAlignmentOptions textAlignment) {
        valueText.alignment = (TMPro.TextAlignmentOptions) textAlignment;
        valueText.SetAllDirty();
        textInput.textComponent.alignment = (TMPro.TextAlignmentOptions) textAlignment;
        textInput.textComponent.SetAllDirty();
        textInput.placeholder.GetComponent<TMPro.TMP_Text> ().alignment = (TMPro.TextAlignmentOptions) textAlignment;
    }

    public void SetTextColor (Color textColor) {
        textInput.textComponent.color = textColor;
        textInput.placeholder.GetComponent<TMPro.TMP_Text> ().color = textColor;
        valueText.color = textColor;
    }

    private void OnTextChanged (string text) {
        this.text = text;
        cxUISystemUtil.RefreshLayout (transform);
    }

    private void OnChangeInputMode () {
        textInput.gameObject.SetActive (true);
        sumitButton?.gameObject.SetActive (true);
        valueText.gameObject.SetActive (false);
        textInput.ActivateInputField ();
        textInput.Select ();
        cxUISystemUtil.RefreshLayout (transform);

        OnInputEditHook?.Invoke (textInput);
        this.inputMode = true;
    }

    private void OnSubmitButton () {
        text = textInput.text;
        this.inputMode = false;
        textInput.DeactivateInputField ();
        
    }

    private void OnSubmit (string text) {
        if(!inputMode) return;

        this.text = text;

        valueText.text = text;
        OnInputEvent.Invoke (text);
        textInput.gameObject.SetActive (false);
        sumitButton?.gameObject.SetActive (false);
        valueText.gameObject.SetActive (true);

        cxUISystemUtil.RefreshLayout (transform);
        this.inputMode = false;
    }

    [ContextMenu ("SetInputMode State")]
    public void SetInputMode () {
        textInput.gameObject.SetActive (true);
        sumitButton?.gameObject.SetActive (true);
        valueText.gameObject.SetActive (false);
        textInput.Select ();
    }

    [ContextMenu ("SetValue Mode State")]
    public void SetValueModeState () {
        textInput.gameObject.SetActive (false);
        sumitButton?.gameObject.SetActive (false);
        valueText.gameObject.SetActive (true);
    }

    private void OnValidate() {
        if(textInput != null) {
            textInput.text = text;
            // if(!textInput.placeholder.GetComponent<TMPro.TMP_Text>().text.Equals(hintText)) {
            //     textInput.placeholder.GetComponent<TMPro.TMP_Text>().text = hintText;
            // }
        }
        if(valueText != null) {
            valueText.text = text;
        }


    }
}