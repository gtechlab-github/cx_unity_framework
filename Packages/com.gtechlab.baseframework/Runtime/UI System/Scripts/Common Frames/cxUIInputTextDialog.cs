using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class cxUIInputTextDialog : cxUIParameterFrame<cxUIInputTextDialog.FrameArgs> {
    public class FrameArgs {
        public string title = "Input Text";
        public string confirmText = "OK";
        public string placeholderText = "Enter text";
        public string initText = string.Empty;
    }

    public TMP_Text titleText;
    public Button closeButton;
    public TMP_InputField inputField;
    public Button okButton;

    protected override void OnInit () {
        closeButton.onClick.AddListener (Pop);
        okButton.onClick.AddListener (() => {
            PopResult (inputField.text);
        });
    }

    protected override void OnActivated (FrameArgs frameArgs) {
        //  string initText =showParam !=null? showParam as string : string.Empty;
        inputField.text = frameArgs.initText;
        inputField.placeholder.GetComponent<TMP_Text> ().text = frameArgs.placeholderText;
        titleText.text = frameArgs.title;
        okButton.SetLabel (frameArgs.confirmText);
    }

    protected override void OnDeactivated () {

    }

}