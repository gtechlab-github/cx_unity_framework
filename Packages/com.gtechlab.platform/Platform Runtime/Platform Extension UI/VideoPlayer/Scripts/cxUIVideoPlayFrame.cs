using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using TMPro;

public class cxUIVideoPlayFrame : cxUIParameterFrame<cxUIVideoPlayFrame.FrameArgs> {

    public class FrameArgs {
        public VideoClip videoClip;
        public string videoURL;
        public string tipMessage;
        public int requiredPlayTime;
        public float startTime = 0;
    }

    public Button closeButton;
    public cxUIVideoController contoller;

    public GameObject tipMessagePanel;
    public TMP_Text tipMessageText;
    public float tipMessageTime = 20.0f;


    private float playStartTime = 0;

    //public VideoClip demoVideoClip;

    protected override void OnInit () {
        closeButton.onClick.AddListener (() => {
            // int playTime = playStartTime > 0 ? Mathf.CeilToInt (Time.realtimeSinceStartup - playStartTime) : 0;
            // PopResult (playTime);
            CloseByUser();
        });

        contoller.OnVideoReady.AddListener (() => {
            playStartTime = Time.realtimeSinceStartup;

            if(frameArgs.startTime > 0) {
                contoller.SeekPlayTime(frameArgs.startTime);
            }
        });

        contoller.OnVideoFinished.AddListener (() => {
            int playTime = Mathf.CeilToInt (Time.realtimeSinceStartup - playStartTime);
            PopResult (playTime);
            // ytPlayer.Stop ();
        });

        contoller.ShowPlayerControlAsObservable.Subscribe(show => {
            
        });
    }

    protected override void OnActivated (FrameArgs frameArgs) {

        playStartTime = 0;
        
#if UNITY_WEBGL
        //for WebGL Video Player를 위해..
        contoller.volumeOn = false;
#endif

        contoller.InitPlayer ();
        
        if(frameArgs.videoClip !=null) {
             contoller.Play(frameArgs.videoClip);
        } else if(!string.IsNullOrEmpty(frameArgs.videoURL)) {
             contoller.Play(frameArgs.videoURL);
        } else {
             throw new Exception("Unknown video url"+frameArgs.videoURL);
        }

        if (!string.IsNullOrEmpty(frameArgs.tipMessage)) {
            StartCoroutine (StartNotice (frameArgs.tipMessage));
        } else {
            tipMessagePanel.SetActive (false);
        }
    }

    protected override void OnDeactivated () {
        //contoller.Dispose();
    }

    IEnumerator StartNotice (string tipMessage) {
        tipMessagePanel.SetActive (true);
        tipMessageText.text = tipMessage;

        yield return new WaitForSeconds (tipMessageTime);
        tipMessagePanel.SetActive (false);
    }

    void CloseByUser () {
        int playTime = playStartTime > 0 ? Mathf.CeilToInt (Time.realtimeSinceStartup - playStartTime) : 0;
        if(frameArgs.requiredPlayTime > 0 && playTime < frameArgs.requiredPlayTime) {
            ShowCaution(playTime);
        } else {
            PopResult (playTime);
        }
    }
     void ShowCaution (int playTime) {
           cxUIConfirmDialog.ShowParam param = new cxUIConfirmDialog.ShowParam () {
                title = "Caution",
                message = "You have not watched the video for the required time. Do you want to continue?",
                confirmText = "Continue",
                cancelText = "Quit"
            };

            contoller.Pause();

            cxUINavigator.Instance.ShowDialog<cxUIConfirmDialog> (param).OnCloseOnceAsObservale.Subscribe (
                (result) => {
                    if (result != null && (bool) result) {
                        contoller.Play();
                    } else {
                         PopResult (playTime);
                    }
                }
            );
    }
}
