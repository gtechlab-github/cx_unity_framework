using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

public class cxUIVideoPlayFrame : cxUIParameterFrame<cxUIVideoPlayFrame.FrameArgs> {

    public class FrameArgs {
        public VideoClip videoClip;
        public string videoURL;
        public string tipMessage;
    }

    public Button closeButton;
    public cxUIVideoController contoller;

    public GameObject tipMessagePanel;
    public Text tipMessageText;
    public float tipMessageTime = 20.0f;


    private float playStartTime = 0;

    //public VideoClip demoVideoClip;

    protected override void OnInit () {
        closeButton.onClick.AddListener (() => {
            int playTime = playStartTime > 0 ? Mathf.CeilToInt (Time.realtimeSinceStartup - playStartTime) : 0;
            PopResult (playTime);
        });

        contoller.OnVideoReady.AddListener (() => {
            playStartTime = Time.realtimeSinceStartup;
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
}
