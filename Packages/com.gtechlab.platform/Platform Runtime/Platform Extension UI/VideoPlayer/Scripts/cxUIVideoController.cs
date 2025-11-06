using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class cxUIVideoController : MonoBehaviour {
    public enum VideoState {
        Init,
        Loading,
        Error,
        Ready,
        Play,
        Pause,
        Seeking,
    }

    [Header ("Play Default Property")]
    public bool hideControlPanelAtPlay = false;
    public bool disableControlPanel = false;
    public bool volumeOn = false; //Note. 시작시 볼륨 on/off 컨트롤 가능


    [Header ("video Panel")]
    public VideoPlayer _player;
    public RawImage _playerRenderer;
    public AspectRatioFitter aspectRatio;

    public cxUITouchDelegator touchDelegator;

    [Header ("On Play Time Panel")]
    public Slider onPlayTimeSlider;

    [Header ("On Control Time Panel")]
    public cxUITweenPanel controlPanel;
    public int hideScreenControlTime = 0;
    public Slider playbackSlider;
    public TMP_Text currentTime;
    public TMP_Text totalTime;
    public Button playButton;
    public Button pauseButton;
    public Button volumeOnButton;
    public Button volumeOffButton;

    [Header ("Loading State")]
    public GameObject loading;

    [Header ("On Error Panel")]
    public GameObject errorPanel;
    public TMP_Text errorText;

    
    //private bool showPlayerControl = false;
    ReactiveProperty<bool> showPlayerControl = new ReactiveProperty<bool> (false);
    public IObservable<bool> ShowPlayerControlAsObservable => showPlayerControl.AsObservable ();

    ReactiveProperty<VideoState> videoState = new ReactiveProperty<VideoState> (VideoState.Init);

    private bool seeking = false;

    public UnityEvent OnVideoReady = new UnityEvent ();
    public UnityEvent OnVideoFinished = new UnityEvent ();

    RenderTexture videoRenderTexture;

    private void Awake () {

        _player.loopPointReached += _onVideoEnded;
        _player.prepareCompleted += _onVideoReady;
        _player.seekCompleted += _onVideoSeekCompleted;
        _player.errorReceived += _onVideoError;

        // volumeSlider.minValue = 0.0f;
        // volumeSlider.maxValue = 1.0f;
        // volumeSlider.onValueChanged.AddListener ((value) => {
        //     ChangeVolume (value);
        // });

        playbackSlider.maxValue = 1.0f;
        playbackSlider.minValue = 0.0f;
        playbackSlider.onValueChanged.AddListener ((value) => {
            ChangeVideoTime (value);
        });

        playButton.onClick.AddListener (() => {
            Play ();
        });

        pauseButton.onClick.AddListener (() => {
            Pause ();
        });

        touchDelegator.onPointerClick.AddListener (() => {
            if(!disableControlPanel) {
                if (showPlayerControl.Value)
                    HideControlPanel ();
                else
                    ShowControlPanel (true);
            }
        });

        volumeOnButton.onClick.AddListener (() => {
            ToggleVolumeControl (true);
        });

        volumeOffButton.onClick.AddListener (() => {
            ToggleVolumeControl (false);
        });
    }

    private void OnDisable () {
        _player.Stop ();
    }

    private void OnDestroy () {
        _player.Stop ();
    }

    void _onVideoReady (VideoPlayer vp) {
        var totalTime = _player.length;

        _player.targetTexture = new RenderTexture ((int) _player.width, (int) _player.height, 0);
        _playerRenderer.texture = _player.targetTexture;
        _playerRenderer.color = Color.white;

       // var canvas = _playerRenderer.transform.parent as RectTransform;
       // var cRatio = canvas.rect.width / canvas.rect.height;
        aspectRatio.aspectRatio = (float)  _player.width / (float) _player.height;

        OnVideoReady.Invoke ();

        //Note. WebGL Player 테스트 때문에 강제로!
        ToggleVolumeControl (volumeOn);

        _player.Play ();
        loading.SetActive (false);

        if(hideControlPanelAtPlay)
            HideControlPanel();
        else
            ShowControlPanel (true);
    }

    void _onVideoEnded (VideoPlayer vp) {
        if(!vp.isLooping) {
            _player.Stop ();
            OnVideoFinished.Invoke ();
        }
    }

    void _onVideoSeekCompleted (VideoPlayer vp) {
        seeking = false;
        _player.Play ();
        loading.SetActive (false);
        ShowControlPanel (true);
    }

    void _onVideoError (VideoPlayer vp, string message) {
        loading.SetActive (false);
        ShowControlPanel (false);

        errorPanel.SetActive (true);
        errorText.text = message;
    }

    public void InitPlayer () {
        playbackSlider.value = 0;
        currentTime.text = string.Empty;
        totalTime.text = string.Empty;
        _playerRenderer.texture = null;
        _playerRenderer.color = Color.black;

        seeking = false;
        loading.SetActive (true);
        errorPanel.SetActive (false);
        HideControlPanel ();
    }

    public void Play (string url, bool loop=false) {
        if (cxResourceNaming.IsHttp (url)) {
            _player.url = url;
            _player.isLooping = loop;
            _player.Play ();
            
        } else if (cxResourceNaming.IsStreaming (url, out string path)) {
            //Note. cannot read file error on ios file:///...

// #if UNITY_EDITOR
//              _player.url = cxResourceNaming.ToAppStreamingPath (path);
// #else
//             _player.url =  Path.Combine (Application.streamingAssetsPath, path);
// #endif
            _player.url =  Path.Combine (Application.streamingAssetsPath, path);

            _player.isLooping = loop;
            _player.Play ();

            Debug.Log ("Play Streaming Video:" + _player.url);
        } else {
            LoadVideoClipPlay (url);
        }
    }

    async void LoadVideoClipPlay (string resourceURL,bool loop=false) {

        try {
            var videoClip = await cxUniversalResourceLoader.Instance.LoadAsset<VideoClip> (resourceURL);

            if (videoClip == null)
                throw new Exception ("Video resource not found");

            if (videoClip)
                Play (videoClip, loop);

        } catch (Exception ex) {
            Debug.LogException (ex);
            errorPanel.SetActive (true);
            errorText.text = ex.Message;
        }
    }

    public void Play (VideoClip videoClip,bool loop=false) {
        _player.clip = videoClip;
        _player.isLooping = loop;
        _player.Play ();
    }

    public void Play () {
        _player.Play ();
        HideControlPanel ();
    }

    public void Pause () {
        _player.Pause ();
        ShowControlPanel (false);
    }

    public void ChangeVolume (float volume) {
        switch (_player.audioOutputMode) {
            case VideoAudioOutputMode.Direct:
                _player.SetDirectAudioVolume (0, volume);
                _player.SetDirectAudioMute(0, volume == 0 ? true: false);

                break;
            case VideoAudioOutputMode.AudioSource:
                _player.GetComponent<AudioSource> ().volume = volume;
                _player.SetDirectAudioVolume (0, volume);
                _player.SetDirectAudioMute(0, volume == 0 ? true: false);

                break;
            default:
                _player.GetComponent<AudioSource> ().volume = volume;
                _player.SetDirectAudioVolume (0, volume);
                _player.SetDirectAudioMute(0, volume == 0 ? true: false);

                break;
        }
    }

    public void ChangePlaybackSpeed (float speed) {
        if (!_player.canSetPlaybackSpeed) return;
        if (speed <= 0) {
            _player.playbackSpeed = .5f;
        } else {
            _player.playbackSpeed = speed;
        }
    }

    public void ChangeVideoTime (float value) {
        float time = value * (float) _player.length;

        _player.time = time;
        _player.Pause ();
        loading.SetActive (true);
        seeking = true;
    }

    public void SeekPlayTime (float time) {
        _player.time = time;
        _player.Pause ();
        loading.SetActive (true);
        seeking = true;
    }

    Coroutine coAutoHideControlPanel;

    public void ToggleVolumeControl (bool on) {

        volumeOn = on;
        volumeOffButton.gameObject.SetActive (on);
        volumeOnButton.gameObject.SetActive (!on);
        ChangeVolume (on ? 1 : 0);

        //Note. Hide Time 확장
        ShowControlPanel (true);
    }

    public void ShowControlPanel (bool autoHide = false) {
        if(disableControlPanel) {
            return;
        }
        
        if (!showPlayerControl.Value) {
            controlPanel.Play ();
            showPlayerControl.Value = true;
        }

        onPlayTimeSlider.gameObject.SetActive (false);

        if (coAutoHideControlPanel != null)
            StopCoroutine (coAutoHideControlPanel);

        if (autoHide)
            coAutoHideControlPanel = StartCoroutine (AutoHideControlPanel ());
    }

    IEnumerator AutoHideControlPanel () {
        yield return new WaitForSecondsRealtime (hideScreenControlTime);
        HideControlPanel ();
    }

    public void HideControlPanel () {
        if (coAutoHideControlPanel != null)
            StopCoroutine (coAutoHideControlPanel);

        controlPanel.Hide ();
        onPlayTimeSlider.gameObject.SetActive (true);

        showPlayerControl.Value = false;
    }

    private void Update () {
        if (_player.isPrepared) {

            playButton.gameObject.SetActive (!_player.isPlaying);
            pauseButton.gameObject.SetActive (_player.isPlaying);

            totalTime.text = FormatTime (Mathf.RoundToInt ((float) _player.length));

            if (seeking) {
                float seekTime = playbackSlider.value * (float) _player.length;
                currentTime.text = FormatTime (Mathf.RoundToInt ((float) seekTime));
            } else {

                currentTime.text = FormatTime (Mathf.RoundToInt ((float) _player.time));

                float t = (float) _player.time / (float) _player.length;
                playbackSlider.SetValueWithoutNotify (t);
                onPlayTimeSlider.SetValueWithoutNotify (t);
            }

        } else {
            playButton.gameObject.SetActive (!_player.isPlaying);
            pauseButton.gameObject.SetActive (_player.isPlaying);

            totalTime.text = FormatTime (0);
            currentTime.text = FormatTime (0);
            playbackSlider.SetValueWithoutNotify (0);
            onPlayTimeSlider.SetValueWithoutNotify (0);

            loading.SetActive (true);
        }
    }

    private string FormatTime (int time) {
        int hours = time / 3600;
        int minutes = (time % 3600) / 60;
        int seconds = (time % 3600) % 60;
        if (hours == 0 && minutes != 0) {
            return minutes.ToString ("00") + ":" + seconds.ToString ("00");
        } else if (hours == 0 && minutes == 0) {
            return "00:" + seconds.ToString ("00");
        } else {
            return hours.ToString ("00") + ":" + minutes.ToString ("00") + ":" + seconds.ToString ("00");
        }
    }

}