using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class cxUIToastMessageFrame : cxUIFrame {
    
    public enum MsgType {
        Normal,
        Warning,
        Congratulation
    }

    [SerializeField]
    private cxUITweenPanel panel;
    [SerializeField]
    private Image background;

    [SerializeField]
    private float hideTime = 3.0f;

    [SerializeField]
    private Text message;

    [SerializeField]
    private TMP_Text messageTmp;

    [SerializeField]
    private Color normalMessageBg = Color.blue;
    [SerializeField]
    private Color normalMessageColor = Color.white;

    [SerializeField]
    private Color warningMessageBg = Color.yellow;
    [SerializeField]
    private Color warningMessageColor = Color.white;

     [SerializeField]
    private Color congratulationMessageBg = Color.yellow;
    [SerializeField]
    private Color congratulationMessageColor = Color.white;

    protected override void OnInit () {
        //cxUINavigator.Instance.PushFrame<cxUIToastMessageFrame>();
    }

    protected override void OnActivated (object showParam) {
        panel.Hide (true);
    }

    protected override void OnDeactivated () {

    }

    public void ShowToast (string message, MsgType msgType = MsgType.Normal) {

        Color messageColor = msgType == MsgType.Normal ? normalMessageColor : msgType == MsgType.Warning ? warningMessageColor : congratulationMessageColor;
        Color backgroundColor = msgType == MsgType.Normal ? normalMessageBg : msgType == MsgType.Warning ? warningMessageBg : congratulationMessageBg;
        if (this.message) {
            this.message.text = message;
             this.message.color = messageColor;
        }
        if (this.messageTmp){
            this.messageTmp.text = message;
            this.messageTmp.color = messageColor;
        }

        this.background.color = backgroundColor;

        panel.Play ();
        panel.ScheduledHide (hideTime);
    }

    public static void Toast (string message, MsgType msgType = MsgType.Normal) {
        var toastFrame = cxUINavigator.Instance.FindFrame<cxUIToastMessageFrame> ();
        if (toastFrame == null) {
            Debug.LogWarning ("cxUIToastMessageFrame Toast, Active Frame Not Found");
            return;
        }

        toastFrame.ShowToast (message, msgType);
    }

    [ContextMenu ("SetWarningMode")]
    void SetWarningMode () {
        this.background.color = warningMessageBg;
        if (this.message)
            this.message.color = warningMessageColor;
        if (this.messageTmp)
            this.messageTmp.color = warningMessageColor;
    }

    [ContextMenu ("SetNormalMode")]
    void SetNormalMode () {
        this.background.color = normalMessageBg;
        if (this.message)
            this.message.color = normalMessageColor;
        if (this.messageTmp)
            this.messageTmp.color = normalMessageColor;
    }

    [ContextMenu ("SetCongratulationMode")]
    void SetCongratulationMode () {
        this.background.color = congratulationMessageBg;
        if (this.message)
            this.message.color = congratulationMessageColor;
        if (this.messageTmp)
            this.messageTmp.color = congratulationMessageColor;
    }
}