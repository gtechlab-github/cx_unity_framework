using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class cxUITweenHelper : MonoBehaviour {

    public enum UIEffectFunc {
        MovingFromLeft,
        MovingFromRight,
        MovingFromUp,
        MovingFromDown,

        MovingFastFromLeft,
        MovingFastFromRight,
        MovingFastFromUp,
        MovingFastFromDown,

        MovingBounceFromLeft,
        MovingBounceFromRight,
        MovingBounceFromUp,
        MovingBounceFromDown,

        ScaleX,
        ScaleY,
        ScaleXY,

        ScaleBounceX,
        ScaleBounceY,
        ScaleBounceXY,

        Stamp,
    }

    public enum MovingDir {
        Left,
        Right,
        Up,
        Down,
    }

    public class TParam {
        public GameObject target;
        public bool show;

        public float delay;
        public bool autoActive;

        public TweenFunc fun = TweenFunc.easeOutBounce;
        public float duration = 0.5f;
    }

    /// <summary>
    /// /////////////////////////////////////////////////////
    /// </summary>
    public static cxUITweenHelper Instance;

    static void Init () {
        if (Instance == null) {
            var c = GameObject.FindObjectOfType<cxUITweenHelper> ();
            if (c == null) {
                var obj = new GameObject ("cxUITweenHelper");
                c = obj.AddComponent<cxUITweenHelper> ();
            }

            Instance = c;

            GameObject.DontDestroyOnLoad (Instance);
        }
    }

    public static void Stamp (RectTransform target, float delay, bool use_realtime = false) {
        Init ();

        Instance.StartCoroutine (Instance.DoStamp (target, delay, new Vector3 (1, 1, 1)));
    }

    public static Coroutine Schedule (float time, System.Action callback) {
        Init ();
        return Instance.StartCoroutine (Instance.CoSchedule (time, callback));
    }

    IEnumerator CoSchedule (float time, System.Action callback) {
        yield return new WaitForSeconds (time);
        callback ();
    }

    public static Coroutine PlayCorutine (IEnumerator func) {
        Init ();
        return Instance.StartCoroutine (func);
    }

    /*********************************************
     * Timer
     * *******************************************/

    public delegate bool TimerCallback (Text target);
    public delegate bool TimerObjectCallback (MonoBehaviour target);

    public static Coroutine PlayTimeText (Text target, float checkT, TimerCallback callback) {
        target.StopAllCoroutines ();
        return target.StartCoroutine (DoPlayTimeText (target, checkT, callback));
    }

    public static Coroutine PlayTimeText (MonoBehaviour target, float checkT, TimerObjectCallback callback) {
        target.StopAllCoroutines ();
        return target.StartCoroutine (DoPlayTimeText (target, checkT, callback));
    }

    public static void StopTimeText (Text target) {
        target.StopAllCoroutines ();
    }

    public static void StopTimeText (MonoBehaviour target) {
        target.StopAllCoroutines ();
    }

    static IEnumerator DoPlayTimeText (Text target, float checkT, TimerCallback callback) {
        bool repeat = false;
        do {
            repeat = callback (target);
            yield return new WaitForSeconds (checkT);
        }
        while (repeat);
    }

    static IEnumerator DoPlayTimeText (MonoBehaviour target, float checkT, TimerObjectCallback callback) {
        bool repeat = false;
        do {
            repeat = callback (target);
            yield return new WaitForSeconds (checkT);
        }
        while (repeat);
    }

    /*
    public static Coroutine PlayTimeText(Text target, float remainedT, UIFillProgressBar gageBar=null, float totalTime=0)
    {
        return target.StartCoroutine(DoPlayTimeText(target, remainedT, gageBar, totalTime));
    }

    static IEnumerator DoPlayTimeText(Text target, float remainedT, UIFillProgressBar gageBar, float totalTime)
    {
        System.DateTime startTime = System.DateTime.Now;
        System.DateTime endTime = startTime.AddMinutes(remainedT);
        string text = string.Empty;
        float r = 1.0f;

        do
        {
            r = (float) (endTime - System.DateTime.Now).TotalMinutes;

            text = "00:00:00";// IconUtil.RemainedQuestTime(r);
            target.text = text;

            if (gageBar != null)
            {
                float p = 1.0f - r / totalTime;
                p = Mathf.Clamp01(p) * 100;

                gageBar.SetValueCurrent((int) p);
            }

            yield return new WaitForSeconds(1.0f);
        }
        while ( r > 0 );
    }
    */

    public static Coroutine ShowTyppingText (Text target, string msg, float playTime) {
        Init ();
        int length = msg.Length;
        float term = playTime / length;

        return Instance.StartCoroutine (CoShowTyppingText (target, msg, term));
    }

    public static Coroutine ShowTyppingTextByTyppingTime (Text target, string msg, float term) {
        Init ();
        int length = msg.Length;
        // float term = playTime / length;

        return Instance.StartCoroutine (CoShowTyppingText (target, msg, term));
    }

    static IEnumerator CoShowTyppingText (Text target, string msg, float term) {
        int length = msg.Length;
        target.text = string.Empty;

        if (length == 0)
            yield break;

        System.Text.StringBuilder buidler = new System.Text.StringBuilder ();
        //float term = playTime / length;

        for (int i = 0; i < length;) {
            char c = msg[i++];
            buidler.Append (c);

            for (; i < length;) {
                c = msg[i];
                if (c != ' ')
                    break;

                buidler.Append (c);
                i++;
            }

            target.text = buidler.ToString ();
            yield return new WaitForSeconds (term);
        }
    }

    /*********************************************
     * Scale
     * *******************************************/

    public static void ShowScaleVBounce (GameObject target, bool autoActive = false, float delay = 0.0f) {
        Init ();
        Instance.StartCoroutine (Instance.DoShowScale (target, true, delay, autoActive, false, true));
    }

    public static void HideScaleVBounce (GameObject target, bool autoActive = false, float delay = 0.0f) {
        Init ();
        Instance.StartCoroutine (Instance.DoShowScale (target, false, delay, autoActive, false, true));
    }

    public static void ShowScaleV (GameObject target, bool autoActive = false, float delay = 0.0f) {
        Init ();
        Instance.StartCoroutine (Instance.DoShowScale (target, true, delay, autoActive, false, true, TweenFunc.linear, 0.3f));
    }

    public static void HideScaleV (GameObject target, bool autoActive = false, float delay = 0.0f) {
        Init ();
        Instance.StartCoroutine (Instance.DoShowScale (target, false, delay, autoActive, false, true, TweenFunc.easeOutQuad));
    }

    /*********************************************
     * Loop Rotate,Shake
     * *******************************************/

    public static Coroutine Shake (RectTransform target, Vector2 shakeScale, float delay = 0.0f) {
        Init ();

        return Instance.StartCoroutine (Instance.DoShake (target, shakeScale, delay));
    }

    public static Coroutine Rotation (GameObject target, float delay = 0.0f, TweenFunc tfunc = TweenFunc.easeOutBounce, float duration = 0.5f) {
        Init ();

        return Instance.StartCoroutine (Instance.DoRotate (target, delay, tfunc, duration));
    }

    //public static void LoopRotation(GameObject target, float delay = 0.0f, TweenFunc tfunc = TweenFunc.easeOutBounce, float duration = 0.5f)
    //{
    //    Init();

    //    script.StartCoroutine(script.DoLoopRotate(target, delay, tfunc, duration));
    //}

    /*********************************************
     * Stamp
     * *******************************************/

    public static Coroutine Stamp (RectTransform target, float delay, Vector3 initScale, TweenFunc tfunc = TweenFunc.easeOutBounce, float duration = 1.5f, bool use_realtime = false) {
        Init ();

        return Instance.StartCoroutine (Instance.DoStamp (target, delay, initScale, tfunc, duration, use_realtime));
    }

    /*********************************************
     * Scale
     * *******************************************/
    public static Coroutine ShowScale (GameObject target, bool autoActive = false, float delay = 0.0f, bool xAxis = true, bool yAxis = true, TweenFunc tweener = TweenFunc.easeOutBounce, float duration = 0.5f, bool use_realtime = false) {
        Init ();
        return Instance.StartCoroutine (Instance.DoShowScale (target, true, delay, autoActive, xAxis, yAxis, tweener, duration, use_realtime));
    }

    public static Coroutine HideScale (GameObject target, bool autoActive = false, float delay = 0.0f, bool xAxis = true, bool yAxis = true, TweenFunc tweener = TweenFunc.easeOutBounce, float duration = 0.5f, bool use_realtime = false) {
        Init ();
        return Instance.StartCoroutine (Instance.DoShowScale (target, false, delay, autoActive, xAxis, yAxis, tweener, duration, use_realtime));
    }

    /*********************************************
    * Alpha
    // * *******************************************/
    // public static Coroutine ShowAlpha(UIPanel target, bool autoActive = false, float delay = 0.0f, TweenFunc tfunc = TweenFunc.linear, float duration = 0.3f)
    // {
    //     Init();

    //     return Instance.StartCoroutine(Instance.DoShowAlpha(target, true, autoActive, delay, tfunc, duration));
    // }

    // public static Coroutine HideAlpha(UIPanel target, bool autoActive = false, float delay = 0.0f, TweenFunc tfunc = TweenFunc.linear, float duration = 0.3f)
    // {
    //     Init();

    //    return  Instance.StartCoroutine(Instance.DoShowAlpha(target, false, autoActive, delay,tfunc, duration));
    // }

    public static Coroutine ShowAlpha (CanvasGroup target, bool autoActive = false, float delay = 0.0f, TweenFunc tfunc = TweenFunc.linear, float duration = 0.3f, bool use_realtime = false) {
        Init ();

        return Instance.StartCoroutine (Instance.DoShowAlpha (target, true, autoActive, delay, tfunc, duration, use_realtime));
    }

    public static Coroutine HideAlpha (CanvasGroup target, bool autoActive = false, float delay = 0.0f, TweenFunc tfunc = TweenFunc.linear, float duration = 0.3f, bool use_realtime = false) {
        Init ();

        return Instance.StartCoroutine (Instance.DoShowAlpha (target, false, autoActive, delay, tfunc, duration, use_realtime));
    }

    /*********************************************
     * From/To
     * *******************************************/
    public static Coroutine ShowFrom (MovingDir dirIndex, RectTransform target, Vector3 initPosition, float delay = 0.0f, TweenFunc tweenFunc = TweenFunc.easeOutBounce, float cycleTime = 1.5f, bool use_realtime = false) {
        Init ();

        int axis = (dirIndex == MovingDir.Left || dirIndex == MovingDir.Right) ? 0 : 1;
        int dir = (dirIndex == MovingDir.Left || dirIndex == MovingDir.Down) ? -1 : 1;

        // Vector3 to = target.transform.localPosition;
        // Vector3 from = target.transform.localPosition;
        Vector3 to = target.anchoredPosition3D;
        Vector3 from = target.anchoredPosition3D;

        to[axis] = initPosition[axis];
        from[axis] = initPosition[axis];

        to[axis] = initPosition[axis];
        from[axis] = to[axis] + 800 * dir;

        return Instance.StartCoroutine (Instance.DoShowMoving (target, from, to, delay, tweenFunc, cycleTime, use_realtime));
    }

    public static Coroutine HideTo (MovingDir dirIndex, RectTransform target, Vector3 initPosition, float delay = 0.0f, TweenFunc tweenFunc = TweenFunc.easeOutBounce, float duration = 1.5f, bool use_realtime = false) {
        Init ();

        int axis = (dirIndex == MovingDir.Left || dirIndex == MovingDir.Right) ? 0 : 1;
        int dir = (dirIndex == MovingDir.Left || dirIndex == MovingDir.Down) ? -1 : 1;

        // Vector3 to = target.transform.localPosition;
        // Vector3 from = target.transform.localPosition;

        Vector3 to = target.anchoredPosition3D;
        Vector3 from = target.anchoredPosition3D;

        to[axis] = initPosition[axis];
        from[axis] = initPosition[axis];

        from[axis] = initPosition[axis];
        to[axis] = from[axis] - 800 * dir;

        return Instance.StartCoroutine (Instance.DoHideMoving (target, from, to, delay, true, tweenFunc, duration, use_realtime));
    }

    /*
    public static void ShowFrom(MovingDir dirIndex, GameObject target, float delay = 0.0f, TweenFunc tweenFunc = TweenFunc.easeOutBounce, Transform anchor = null, float cycleTime = 1.5f)
    {
        Init();

        int axis = (dirIndex == MovingDir.Left || dirIndex == MovingDir.Right)? 0 :  1;
        int dir = (dirIndex == MovingDir.Left || dirIndex == MovingDir.Down)? -1 : 1;

        Vector3 to = target.transform.localPosition;
        Vector3 from = target.transform.localPosition;

        to[axis] = anchor != null ? anchor.localPosition[axis] : target.transform.localPosition[axis];
        from[axis] = anchor != null ? anchor.localPosition[axis] : target.transform.localPosition[axis];

        to[axis] = anchor != null ? anchor.localPosition[axis] : 0;
        from[axis] = to[axis] + 800 * dir;

        script.StartCoroutine(script.DoShowMoving(target, from, to, delay, tweenFunc, cycleTime));
    }
    */

    public static Coroutine ShowFromLeft (RectTransform target, float delay = 0.0f, TweenFunc tweenFunc = TweenFunc.easeOutBounce, Transform anchor = null) {
        Init ();

        Vector3 to = anchor != null ? anchor.localPosition : target.transform.localPosition;
        Vector3 from = anchor != null ? anchor.localPosition : target.transform.localPosition;

        to.x = anchor != null ? anchor.localPosition.x : 0;
        from.x = to.x - 800;

        return Instance.StartCoroutine (Instance.DoShowMoving (target, from, to, delay, tweenFunc));
    }

    public static Coroutine ShowFromRight (RectTransform target, Transform anchor = null, float delay = 0.0f, TweenFunc tweenFunc = TweenFunc.easeOutQuad, float duration = 0.5f) {
        Init ();

        // Vector3 to = anchor != null ? anchor.localPosition : target.transform.localPosition;
        // Vector3 from = anchor != null ? anchor.localPosition : target.transform.localPosition;
        Vector3 to = target.anchoredPosition3D;
        Vector3 from = target.anchoredPosition3D;

        to.x = anchor != null ? anchor.localPosition.x : 0;
        from.x = to.x + 800;

        return Instance.StartCoroutine (Instance.DoShowMoving (target, from, to, delay, tweenFunc, duration));
    }

    public static Coroutine ShowFromUp (RectTransform target, float delay = 0.0f) {
        Init ();

        // Vector3 from = target.transform.localPosition;
        // Vector3 to = target.transform.localPosition;
        Vector3 to = target.anchoredPosition3D;
        Vector3 from = target.anchoredPosition3D;

        from.y = 800;
        to.y = 0;

        return Instance.StartCoroutine (Instance.DoShowMoving (target, from, to, delay, TweenFunc.easeInQuad));
    }

    public static Coroutine HideToLeft (RectTransform target, float delay, bool autoActive = false) {
        Init ();

        // Vector3 from = target.transform.localPosition;
        // Vector3 to = target.transform.localPosition;
        Vector3 to = target.anchoredPosition3D;
        Vector3 from = target.anchoredPosition3D;

        from.x = 0;
        to.x = -800;

        return Instance.StartCoroutine (Instance.DoHideMoving (target, from, to, delay, autoActive));
    }

    public static Coroutine IncValue (Text target, int initValue, int nextValue, string textFormat, float delay = 0.0f) {
        Init ();

        return Instance.StartCoroutine (Instance.DoIncValue (target, initValue, nextValue, textFormat, TweenFunc.linear, delay));
    }

    public static Coroutine IncValue (TMPro.TMP_Text target, int initValue, int nextValue, string textFormat, float delay = 0.0f) {
        Init ();

        return Instance.StartCoroutine (Instance.DoIncValue (target, initValue, nextValue, textFormat, TweenFunc.linear, delay));
    }

    public static Coroutine IncProgress (Image target, float initValue, float nextValue, float delay = 0.0f) {
        Init ();
        return Instance.StartCoroutine (Instance.DoIncProgress (target, initValue, nextValue, delay));
    }
    // public static Coroutine IncProgress(UIRepeatProgressBar target, int initValue, int nextValue, float delay = 0.0f)
    // {
    //     Init();
    //     return Instance.StartCoroutine(Instance.DoIncProgress(target, initValue, nextValue, delay));
    // }

    IEnumerator DoStamp (RectTransform target, float delay, Vector3 initScale, TweenFunc tfunc = TweenFunc.easeOutBounce, float duration = 1.5f, bool use_realtime = false) {
        if (delay > 0) {
            target.gameObject.SetActive (false);
            yield return use_realtime ? new WaitForSecondsRealtime (delay) : new WaitForSeconds (delay);
            target.gameObject.SetActive (true);
        }

        target.transform.localScale = new Vector3 (5.0f * initScale.x, 5.0f * initScale.y, 1.0f);
        TweenUtil t1 = new TweenUtil (duration, use_realtime);

        t1.Trigger ();
        while (t1.On) {
            float sx = t1.FloatValue (tfunc, 5.0f * initScale.x, initScale.x);
            float sy = t1.FloatValue (tfunc, 5.0f * initScale.y, initScale.y);
            target.transform.localScale = new Vector3 (sx, sy, 1.0f);

            yield return null;
        }
    }

    IEnumerator DoShake (RectTransform target, Vector2 shakeScale, float delay, TweenFunc tfunc = TweenFunc.easeInQuad, float duration = 1.5f) {

        TweenUtil t1 = new TweenUtil (0.5f, 4, TweenLoopType.PingPong);
        TweenUtil t2 = new TweenUtil (2.0f, 1, TweenLoopType.PingPong);

        if (delay > 0)
            yield return new WaitForSeconds (delay);

        t1.Trigger ();
        t2.Trigger ();

        // Vector3 initPos = target.transform.localPosition;
        Vector3 initPos = target.anchoredPosition3D;

        if (t1.On) {
            float x = t1.FloatValue (TweenFunc.linear, -shakeScale.x, shakeScale.x);
            float y = t1.FloatValue (TweenFunc.linear, -shakeScale.y, shakeScale.y);

            float scale = t2.FloatValue (tfunc, 1.0f, 0.0f);
            x *= scale;
            y *= scale;

            Vector3 pos = new Vector3 (initPos.x + x, initPos.y + y, initPos.z);
            // target.transform.localPosition = pos;
            target.anchoredPosition3D = pos;
            yield return null;
        }

        target.transform.localPosition = initPos;
    }

    IEnumerator DoIncValue (Text target, int initValue, int nextValue, string textFormat, TweenFunc tween = TweenFunc.easeInOutExpo, float delay = 0.0f) {
        target.text = string.Format (textFormat, initValue);

        if (delay > 0)
            yield return new WaitForSeconds (delay);

        TweenUtil tw = new TweenUtil (2.0f);
        tw.Trigger ();

        do {
            yield return null;

            int v = (int) tw.FloatValue (tween, initValue, nextValue);
            target.text = string.Format (textFormat, v);
        }
        while (tw.On);

        target.text = string.Format (textFormat, nextValue);
    }

    IEnumerator DoIncValue (TMPro.TMP_Text target, int initValue, int nextValue, string textFormat, TweenFunc tween = TweenFunc.easeInOutExpo, float delay = 0.0f) {
        target.text = string.Format (textFormat, initValue);

        if (delay > 0)
            yield return new WaitForSeconds (delay);

        TweenUtil tw = new TweenUtil (2.0f);
        tw.Trigger ();

        do {
            yield return null;

            int v = (int) tw.FloatValue (tween, initValue, nextValue);
            target.text = string.Format (textFormat, v);
        }
        while (tw.On);

        target.text = string.Format (textFormat, nextValue);
    }

    IEnumerator DoIncProgress (Image target, float initValue, float nextValue, float delay) {
        //target.text = string.Format(textFormat, initValue);
        target.fillAmount = initValue;

        if (delay > 0)
            yield return new WaitForSeconds (delay);

        TweenUtil tw = new TweenUtil (2.0f);
        tw.Trigger ();

        do {
            yield return null;

            float v = (float) tw.FloatValue (TweenFunc.easeInOutExpo, initValue, nextValue);
            target.fillAmount = v;
        }
        while (tw.On);

        target.fillAmount = nextValue;
    }

    /* 
    IEnumerator DoIncProgress(UIRepeatProgressBar target, int initValue, int nextValue, float delay)
    {
        //target.text = string.Format(textFormat, initValue);
        target.SetValueCurrent(initValue);

        if (delay > 0)
            yield return new WaitForSeconds(delay);


        TweenUtil tw = new TweenUtil(2.0f);
        tw.Trigger();

        do
        {
            yield return null;

            int v = (int)tw.FloatValue(TweenFunc.easeInOutExpo, initValue, nextValue);
            target.SetValueCurrent(v);
        }
        while (tw.On);

        target.SetValueCurrent(nextValue);
    }
    */

    /*
    IEnumerator DoShowAlpha(UIPanel target, bool show, bool autoActive, float delay,TweenFunc tfunc=TweenFunc.linear, float duration = 0.3f)
    {
        if (autoActive)
            if (show)
                target.gameObject.SetActive(show);

        target.alpha = show ? 0.0f : 1.0f;

        if (delay > 0)
            yield return new WaitForSeconds(delay);

        TweenUtil tw = new TweenUtil(duration);
        tw.Trigger();

        float initValue = show ? 0.0f : 1.0f;
        float nextValue = show ? 1.0f : 0.0f;
        do
        {
            yield return null;

            float v = tw.FloatValue(tfunc, initValue, nextValue);
            target.alpha = v;
        }
        while (tw.On);

        target.alpha =nextValue;

        if (autoActive)
            if (!show)
                target.gameObject.SetActive(show);

    }
    */

    IEnumerator DoShowAlpha (CanvasGroup target, bool show, bool autoActive, float delay, TweenFunc tfunc = TweenFunc.linear, float duration = 0.3f, bool use_realtime = false) {
        if (autoActive)
            if (show)
                target.gameObject.SetActive (show);

        target.alpha = show ? 0.0f : 1.0f;

        if (delay > 0)
            yield return use_realtime ? new WaitForSecondsRealtime (delay) : new WaitForSeconds (delay);

        TweenUtil tw = new TweenUtil (duration, 1, TweenLoopType.Loop, use_realtime);
        tw.Trigger ();

        float initValue = show ? 0.0f : 1.0f;
        float nextValue = show ? 1.0f : 0.0f;
        do {
            yield return null;

            float v = tw.FloatValue (tfunc, initValue, nextValue);
            target.alpha = v;
        }
        while (tw.On);

        target.alpha = nextValue;

        if (autoActive)
            if (!show)
                target.gameObject.SetActive (show);

    }

    IEnumerator DoShowScale (GameObject target, bool show, float delay, bool autoActive, bool x, bool y, TweenFunc fun = TweenFunc.easeOutBounce, float duration = 0.5f, bool use_realtime = false) {
        Vector3 initValue = new Vector3 (x ? (show ? 0.0f : 1.0f) : 1.0f, y ? (show ? 0.0f : 1.0f) : 1.0f, 1.0f);
        Vector3 nextValue = new Vector3 (x ? (show ? 1.0f : 0.0f) : 1.0f, y ? (show ? 1.0f : 0.0f) : 1.0f, 1.0f);

        if (autoActive)
            if (show)
                target.SetActive (show);

        target.transform.localScale = initValue;

        if (delay > 0)
            yield return use_realtime ? new WaitForSecondsRealtime (delay) : new WaitForSeconds (delay);

        TweenUtil tw = new TweenUtil (duration, use_realtime);
        tw.Trigger ();

        do {
            yield return null;
            if(!target)
                yield break;

            var v = tw.VectorValue (fun, initValue, nextValue);
            target.transform.localScale = v;
        }
        while (tw.On);

        target.transform.localScale = nextValue;

        if (autoActive)
            if (!show)
                target.SetActive (show);
    }

    IEnumerator DoRotate (GameObject target, float delay, TweenFunc tf = TweenFunc.easeOutBounce, float duration = 0.5f) {
        Vector3 initValue = new Vector3 (0, 0, 0);
        Vector3 nextValue = new Vector3 (0, 0, 360);

        target.transform.localRotation = Quaternion.AngleAxis (initValue.z, Vector3.forward);

        if (delay > 0)
            yield return new WaitForSeconds (delay);

        TweenUtil tw = new TweenUtil (duration);
        tw.Trigger ();

        do {
            yield return null;

            var v = tw.VectorValue (tf, initValue, nextValue);
            target.transform.localRotation = Quaternion.AngleAxis (v.z, Vector3.forward); //Quaternion.Euler(v);
        }
        while (tw.On);

        target.transform.localRotation = Quaternion.AngleAxis (nextValue.z, Vector3.forward); //Quaternion.Euler(nextValue);
    }

    IEnumerator DoLoopRotate (GameObject target, float delay, TweenFunc tf = TweenFunc.easeOutBounce, float duration = 0.5f) {
        Vector3 initValue = new Vector3 (0, 0, 0);
        Vector3 nextValue = new Vector3 (0, 0, 360);

        target.transform.localRotation = Quaternion.AngleAxis (initValue.z, Vector3.forward);

        if (delay > 0)
            yield return new WaitForSeconds (delay);

        TweenUtil tw = new TweenUtil (duration, 1000000, TweenLoopType.Loop);
        tw.Trigger ();

        do {
            yield return null;

            var v = tw.VectorValue (tf, initValue, nextValue);
            target.transform.localRotation = Quaternion.AngleAxis (v.z, Vector3.forward); //Quaternion.Euler(v);
        }
        while (tw.On);

        target.transform.localRotation = Quaternion.AngleAxis (nextValue.z, Vector3.forward); //Quaternion.Euler(nextValue);
    }

    IEnumerator DoShowMoving (RectTransform target, Vector3 from, Vector3 to, float delay, TweenFunc tf = TweenFunc.easeOutBounce, float cycleTime = 1.5f, bool use_realtime = false) {
        if (delay > 0) {
            //target.SetActive(false);
            target.anchoredPosition3D = new Vector3 (1000, 0, 0);
            yield return use_realtime ? new WaitForSecondsRealtime (delay) : new WaitForSeconds (delay);
            target.gameObject.SetActive (true);
        }

        target.transform.localPosition = from;
        TweenUtil t1 = new TweenUtil (cycleTime, use_realtime);
        t1.Trigger ();

        while (t1.On) {
            Vector3 p = t1.VectorValue (tf, from, to);
            //target.transform.localPosition = p;
            target.anchoredPosition3D = p;
            yield return null;
        }

        //target.transform.localPosition = to;
        target.anchoredPosition3D = to;

        //var rectT = target.transform as RectTransform;
        //int k=0;
    }

    IEnumerator DoHideMoving (RectTransform target, Vector3 from, Vector3 to, float delay, bool autoActive, TweenFunc tween = TweenFunc.easeOutBounce, float duration = 1.5f, bool use_realtime = false) {
        if (delay > 0) {
            target.gameObject.SetActive (false);
            yield return use_realtime ? new WaitForSecondsRealtime (delay) : new WaitForSeconds (delay);
            target.gameObject.SetActive (true);
        }

        // target.transform.localPosition = from;
        target.anchoredPosition3D = from;

        TweenUtil t1 = new TweenUtil (duration, use_realtime);
        t1.Trigger ();

        while (t1.On) {
            Vector3 p = t1.VectorValue (tween, from, to);
            //target.transform.localPosition = p;
            target.anchoredPosition3D = p;

            yield return null;
        }

        //target.transform.localPosition = to;
        target.anchoredPosition3D = to;

        if (autoActive)
            target.gameObject.SetActive (false);
    }
}