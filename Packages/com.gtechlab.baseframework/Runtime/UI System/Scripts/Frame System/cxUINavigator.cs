using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cxUINavigator : MonoSingleton<cxUINavigator> {

    [SerializeField]
    private Canvas rootCanvas;

    [SerializeField]
    private List<cxUIFrame> preOpenFrames;

    [SerializeField]
    private List<cxUIFrame> prefabs;

    private List<cxUIFrame> frames;

    protected override void Awake () {
        base.Awake ();

        frames = new List<cxUIFrame> ();

        foreach (var p in preOpenFrames) {
            var type = p.GetType ();
            var prevFrame = frames.Find (q => q.GetType () == type);
            if (prevFrame != null) {
                Debug.LogError ("PreOpenFrame already exists frame:" + prevFrame.name + " type:" + type);
                continue;
            }

            try {
                var frame = PrefabCreateUGUI<cxUIFrame> (p, rootCanvas.gameObject);
                frames.Add (frame);
            } catch (Exception ex) {
                Debug.LogError ("cxUINavigator Create Frame failed: " + p.name);
                Debug.LogException (ex);
            }
        }

        foreach (var p in prefabs) {
            var type = p.GetType ();
            var prevFrame = frames.Find (q => q.GetType () == type);
            if (prevFrame != null) {
                Debug.LogError ("Prefab already exists frame:" + prevFrame.name + " type:" + type);
                continue;
            }
            try {
                var frame = PrefabCreateUGUI<cxUIFrame> (p, rootCanvas.gameObject);
                frames.Add (frame);
            } catch (Exception ex) {
                Debug.LogError ("cxUINavigator Create Frame failed: " + p.name);
                Debug.LogException (ex);
            }
        }
    }

    void Start () {
        foreach (var p in preOpenFrames) {
            var type = p.GetType ();
            var frame = frames.Find (q => q.GetType () == type);
            if (frame == null) {
                Debug.LogError ("PreOpenedFrame not found type:" + type);
                continue;
            }

            if (p.FrameTypeName == cxUIFrame.FrameType.Dialog) {
                cxUIContext context = new cxUIContext (null);
                frame.PushPopup (context, null);
            } else if (p.FrameTypeName == cxUIFrame.FrameType.Aux) {
                cxUIContext context = new cxUIContext (null);
                frame.Push (context, null);
            } else {
                cxUIContext context = new cxUIContext (null);
                frame.Push (context, null);
            }
        }
    }

    public T FindFrame<T> () where T : cxUIFrame {
        return cxUIFrame.FindFrame<T> ();
    }

    public cxUIFrame ReplacePushFrame<T> (object showParam = null) where T : cxUIFrame {
        var frame = frames.Find (q => q.GetType () == typeof (T));
        if (frame == null)
            throw new Exception ("ReplacePushFrame FrameType not found type:" + typeof (T));
        if (frame.IsDialog)
            throw new Exception ("ReplacePushFrame FrameType is Not Frame type:" + typeof (T));

        cxUIContext context = new cxUIContext (showParam);
        frame.Replace (context, showParam);
        return frame;
    }

    public cxUIFrame PushFrame<T> (object showParam = null) where T : cxUIFrame {
        var frame = frames.Find (q => q.GetType () == typeof (T));

        if (frame == null)
            throw new Exception ("PushFrame FrameType not found type:" + typeof (T));
        if (frame.IsDialog)
            throw new Exception ("PushFrame FrameType is Not Frame type:" + typeof (T));

        cxUIContext context = new cxUIContext (showParam);
        frame.Push (context, showParam);

        return frame;
    }

    public void PopFrame () {
        if (cxUIFrame.ActiveFrame)
            cxUIFrame.ActiveFrame.Pop ();
    }

    public void PopAllFrames () {
        while (cxUIFrame.ActiveFrame) {
            cxUIFrame.ActiveFrame.Pop ();
        }
    }

    //Note.
    //  1. 팝업은 단독으로 나타날 수 없으며, 프레임위에서만 보여진다.
    //  2. 팝업의 라이프사이클은 베이스프레임에 종속되어 있다.
    public cxUIFrame ShowDialog<T> (object showParam = null) where T : cxUIFrame {
        var frame = frames.Find (q => q.GetType () == typeof (T));

        if (frame == null)
            throw new Exception ("ShowDialog FrameType not found type:" + typeof (T));
        if (!frame.IsDialog)
            throw new Exception ("ShowDialog FrameType is Not Dialog type:" + typeof (T));

        cxUIContext context = new cxUIContext (showParam);
        frame.PushPopup (context, showParam);

        return frame;
    }

    public void PopDialog () {
        if (cxUIFrame.ActiveFrame?.ActivePopup)
            cxUIFrame.ActiveFrame?.ActivePopup.Pop ();
    }

    private static T PrefabCreateUGUI<T> (T prefabobj, GameObject prefabparentobj) where T : MonoBehaviour {
        try {
            T prefab = (T) GameObject.Instantiate (prefabobj);
            if (prefab == null) return null;

            prefab.name = prefabobj.name;

            GameObject parent = prefabparentobj;
            if (parent != null) {
                prefab.transform.SetParent (parent.transform);
                //prefab.transform.Translate(parent.transform.position);
            }

            var rectT = prefab.GetComponent<RectTransform> ();
            //Ah-hoc, Jongok
            rectT.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
            rectT.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);

            rectT.anchorMin = new Vector2 (0, 0);
            rectT.anchorMax = new Vector2 (1, 1);
            rectT.offsetMin = new Vector2 (0, 0);
            rectT.offsetMax = new Vector2 (0, 0);

            return prefab;
        } catch (System.Exception ex) {
            Debug.Log ("PrefabCreate except : " + ex.Message);
            throw ex;
        }
        //return null;
    }

}