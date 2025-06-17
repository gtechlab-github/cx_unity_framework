using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class cxUITweenPanel: MonoBehaviour {

    public enum UITweenType
    {
        ShowFromLeft,
        ShowFromRight,
        ShowFromUp,
        ShowFromDown,

        ShowScaleX,
        ShowScaleY,
        ShowScaleXY,
            
        ShowStamp,
        ShowAlpha,

        LoopRotate,
        LoopShake,

        Undef=100,
    }

    [System.Serializable]
    public class UIEffectParam
    {
        public UITweenType type = UITweenType.Undef;
        public GameObject anchor;
        public float delay;
        public TweenFunc tfunc = TweenFunc.easeOutQuad;
        public float duration=0.5f;
    }

    public bool m_activatingOnAwake = true;

    public bool m_auto = false; 

    public bool ignore_reset_pos_scale = false;
    public bool use_realtime = false;

    public UIEffectParam m_param;
    public UIEffectParam m_hideParam;

    Coroutine m_activeFxCo;

    private Vector3 initPosition;
    private Vector3 initScale;
    private CanvasGroup initPanel;
    private RectTransform rectTransform;

	private bool init=false;

    void Awake()
    {
        gameObject.SetActive(m_activatingOnAwake);
		Init ();
    }

	void Init()
	{
		init = true;
		initPanel = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

/*
		if (m_param.anchor != null)
		{
			initPosition = m_param.anchor.transform.localPosition;
			initScale = m_param.anchor.transform.localScale;
		}
		else
		{
			initPosition = transform.localPosition;
			initScale = transform.localScale;
		}
*/
        initPosition = rectTransform.anchoredPosition3D;
		initScale = rectTransform.localScale;
	}

    void OnEnable()
    {
        if (m_auto)
            Play();
    }

    void OnDisable()
    {
        if(m_activeFxCo !=null && cxUITweenHelper.Instance !=null)
            cxUITweenHelper.Instance.StopCoroutine(m_activeFxCo);
    }


    [ContextMenu("Play")]
    void PlayInEditor()
    {
        Play();
    }

    public void Play(bool imm=false)
    {
		if (!init)
			Init ();

        if(!ignore_reset_pos_scale)
        {
            rectTransform.localScale = initScale;
	    	//gameObject.transform.localPosition = initPosition;
            rectTransform.anchoredPosition3D = initPosition;
        }

        if(m_activeFxCo !=null)
            cxUITweenHelper.Instance.StopCoroutine(m_activeFxCo);

        ClearScheduledHide();   

        //var rectT = transform as RectTransform;

        if(imm)
            gameObject.SetActive(true);
        else
		   m_activeFxCo = PlayUIEffect(rectTransform, m_param, m_param.delay, initPosition, initScale, initPanel, use_realtime);
    }

    public void Play(float delay)
    {
		if (!init)
			Init ();
        
        if(m_activeFxCo !=null)
            cxUITweenHelper.Instance.StopCoroutine(m_activeFxCo);

        if(!ignore_reset_pos_scale)
        {
            rectTransform.localScale = initScale;
	    	rectTransform.anchoredPosition3D = initPosition;
        }

		m_activeFxCo = PlayUIEffect(rectTransform, m_param, delay, initPosition, initScale, initPanel, use_realtime);
    }


    public void Play(UIEffectParam param)
    {
        if(m_activeFxCo !=null)
            cxUITweenHelper.Instance.StopCoroutine(m_activeFxCo);
        
        ClearScheduledHide();

		if(!ignore_reset_pos_scale)
        {
            rectTransform.localScale = initScale;
	    	rectTransform.localPosition = initPosition;
        }
        
       m_activeFxCo = PlayUIEffect(rectTransform, param, param.delay, initPosition, initScale, initPanel, use_realtime);
    }

    [ContextMenu("Hide")]
    public void Hide(bool imm=false)
    {
        if (!init)
			Init ();

        if(m_activeFxCo !=null)
             cxUITweenHelper.Instance.StopCoroutine(m_activeFxCo);

        ClearScheduledHide();

    //    gameObject.transform.localScale = initScale;
    //    gameObject.transform.localPosition = initPosition;

        if(imm)
            gameObject.SetActive(false);
        else
            m_activeFxCo=  HideUIEffect(rectTransform, m_hideParam, m_param.delay, initPosition, initScale, initPanel, use_realtime);
    }
	
	// public void Hide(float delayTime)
	// {
    //     if (!init)
	// 		Init ();

    //     if(m_activeFxCo !=null)
    //        afUITweenFunc.Instance.StopCoroutine(m_activeFxCo);

	//     m_activeFxCo =	HideUIEffect(gameObject, m_hideParam, delayTime + m_param.delay, initPosition, initScale, initPanel);
	// }

	Coroutine m_coScheduledHide;

    public void ScheduledHide(float delayTime)
	{
        if (!init)
			Init ();

        ClearScheduledHide();

        m_coScheduledHide = StartCoroutine(CoScheduledHide(delayTime));
	}

    void ClearScheduledHide()
    {
        if(m_coScheduledHide !=null)
        {
            StopCoroutine(m_coScheduledHide);
            m_coScheduledHide =null;
        }
    }
	IEnumerator CoScheduledHide(float delayTime)
	{
		yield return use_realtime ? new WaitForSecondsRealtime(delayTime) : new WaitForSeconds(delayTime);
		Hide();
	}


    public static void Play(GameObject obj)
    {
        var comp = obj.GetComponent<cxUITweenPanel>();
        if (comp != null)
            comp.Play();
    }

    public static void Hide(GameObject obj)
    {
        var comp = obj.GetComponent<cxUITweenPanel>();
        if (comp != null)
            comp.Hide();
        else
            obj.SetActive(false);
    }

    public static void Play(RectTransform obj, UIEffectParam param, float delay = 0.0f)
    {
        PlayUIEffect(obj, param, delay, Vector3.zero, Vector3.one,null, false);
    }

    /***************************************************************
     * 
     * Template
     * 
     * *************************************************************/

    /***************************************************************
     * 
     * Static !!
     * 
     * *************************************************************/
    private static Coroutine PlayUIEffect(RectTransform rectT, UIEffectParam param, float delay, 
                                        Vector3 initPosition, Vector3 initScale, CanvasGroup initPanel, bool use_realtime)
    {
        if (delay == 0)
            rectT.gameObject.SetActive(true);

        switch (param.type)
        {
            case UITweenType.ShowFromLeft:
                {
                   return cxUITweenHelper.ShowFrom(cxUITweenHelper.MovingDir.Left, rectT, initPosition, delay, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowFromRight:
                {
                   return cxUITweenHelper.ShowFrom(cxUITweenHelper.MovingDir.Right, rectT, initPosition, delay, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowFromUp:
                {
                  return  cxUITweenHelper.ShowFrom(cxUITweenHelper.MovingDir.Up, rectT, initPosition, delay, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowFromDown:
                {
                   return cxUITweenHelper.ShowFrom(cxUITweenHelper.MovingDir.Down, rectT, initPosition, delay, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowScaleX:
                {
                   return cxUITweenHelper.ShowScale(rectT.gameObject, true, delay, true, false, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowScaleY:
                {
                   return cxUITweenHelper.ShowScale(rectT.gameObject, true, delay, false, true, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowScaleXY:
                {
                    return cxUITweenHelper.ShowScale(rectT.gameObject, true, delay, true, true, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowStamp:
                {
                   return cxUITweenHelper.Stamp(rectT, delay, initScale, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowAlpha:
                {
                   return cxUITweenHelper.ShowAlpha(initPanel, true, delay, param.tfunc, param.duration, use_realtime);
                }
                break;
            //case UIEffectType.LoopRotate:
            //    {
            //        UIEffectUtil.LoopRotation(gameObject, delay, param.tfunc, param.duration);
            //    }
            //    break;
            //case UIEffectType.LoopShake:
            //    {
            //        UIEffectUtil.Shake(gameObject, new Vector2(1.2f, 1.2f));
            //    }
            //    break;
        }

        return null;
    }

    private static Coroutine HideUIEffect(RectTransform rectT, UIEffectParam param, float delay,
                                       Vector3 initPosition, Vector3 initScale, CanvasGroup initPanel, bool use_realtime)
    {
        switch (param.type)
        {
            case UITweenType.ShowFromLeft:
                {
                   return cxUITweenHelper.HideTo(cxUITweenHelper.MovingDir.Left, rectT, initPosition, delay, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowFromRight:
                {
                   return cxUITweenHelper.HideTo(cxUITweenHelper.MovingDir.Right, rectT, initPosition, delay, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowFromUp:
                {
                   return cxUITweenHelper.HideTo(cxUITweenHelper.MovingDir.Up, rectT, initPosition, delay, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowFromDown:
                {
                  return  cxUITweenHelper.HideTo(cxUITweenHelper.MovingDir.Down, rectT, initPosition, delay, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowScaleX:
                {
                   return cxUITweenHelper.HideScale(rectT.gameObject, true, delay, true, false, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowScaleY:
                {
                   return cxUITweenHelper.HideScale(rectT.gameObject, true, delay, false, true, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowScaleXY:
                {
                   return cxUITweenHelper.HideScale(rectT.gameObject, true, delay, true, true, param.tfunc, param.duration, use_realtime);
                }
                break;
            case UITweenType.ShowStamp:
                {
                    //UIEffectUtil.Stamp(gameObject, delay, initScale, param.tfunc, param.duration);
                }
                break;
            case UITweenType.ShowAlpha:
                {
                   return cxUITweenHelper.HideAlpha(initPanel, true, delay, param.tfunc, param.duration, use_realtime);
                }
                break;
            default:
                {
                    rectT.gameObject.SetActive(false);
                    return null;
                }
                break;
        }

        return null;
    }
}
