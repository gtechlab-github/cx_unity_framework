using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder (-99)]
public class cxUIInputController : MonoSingleton<cxUIInputController> {


    [SerializeField]
    private Vector2 referenceResolution = new Vector2 (1920, 1080);

    [SerializeField]
    private bool clickAtLongPress = true;

    private GameObject focused {  get;  set; }



    bool OnMouseDown => Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1);
    bool OnMouseUp => Input.GetMouseButtonUp (0) || Input.GetMouseButtonUp (1);
    int MouseButtonID => Input.GetMouseButton (0) ? 0 : Input.GetMouseButton (1) ? 1 : -1;
    
    public bool IsLeftButton => mouseButton == 0;
    public bool IsRightButton => mouseButton == 1;
    public bool IsLongPress => pressTime > 1.0f;
    
    public Vector2? ScreenClickedThisFrame { get; private set; }
    public Vector2? PressedDragDeltaThisFrame { get; private set; }
    public Vector2? ScrollDeltaThisFrame { get; private set; }

    public Vector2? PressedDragNormalizedDeltaThisFrame { get {
        if(PressedDragDeltaThisFrame.HasValue) {
            return GetNormalizedPosition(PressedDragDeltaThisFrame.Value);
        }
        return null;
    } }
    
    Vector3? beginPosition;
    Vector3? lastPosition;
    bool mouseDragged;
    int mouseButton;
    float downTime;
    float pressTime;

    public bool IsFocused => focused != null || IsAnyInputFieldFocused ();
    public bool HasFocus(GameObject gObject) => focused == gObject ||  EventSystem.current.currentSelectedGameObject == gObject;

    public bool CanAcquireFocus(GameObject go) {
        if(focused == go)
            return true;

        if(focused != null) {
            return false;
        }

        return true;
    }

    public bool AcquireFocus (GameObject go, bool force = false) {
        if(focused == go)
            return true;

        if(focused != null && !force) {
           return false;
        }

        focused = go;
        Debug.Log($"AcquireFocus {go.name}");
        return true;
    }

    public void ReleaseFocus (GameObject go) {
        if (focused == go) {
            focused = null;
            Debug.Log($"ReleaseFocus {go.name}");
        }
    }

    //<summary>
    // 입력 필드에 포커스 되어 있는지 상태 체크
    //</summary>
    public bool IsAnyInputFieldFocused () {
        var selectedObject = EventSystem.current.currentSelectedGameObject;
        return selectedObject != null && (
            selectedObject.GetComponent<InputField> () != null ||
            selectedObject.GetComponent<TMP_InputField> () != null
        );
    }

    //<summary>
    // 화면 좌표를 정규화된 좌표로 변환
    //</summary>
    public Vector2 GetNormalizedPosition(Vector2 position) {
        float refX = (float) referenceResolution.x / Screen.width;
        float refY = (float) referenceResolution.y / Screen.height;

        return new Vector2(position.x * refX, position.y * refY);
    }

    void Update () {
        PressedDragDeltaThisFrame = null;
        ScrollDeltaThisFrame = null;
        ScreenClickedThisFrame = null;

        if (focused != null) {
            beginPosition = null;
            lastPosition = null;
            mouseButton = -1;
            pressTime = 0;
            return;
        }

        if (OnMouseDown) {
            if (!cxUISystemUtil.IsMousePointerOnUI ()) {
                beginPosition = Input.mousePosition;
                lastPosition = beginPosition;
                mouseButton = MouseButtonID;
                mouseDragged = false;
                downTime = Time.realtimeSinceStartup;
                pressTime = 0;
            }
        }

        if (beginPosition.HasValue) {
            PressedDragDeltaThisFrame = Input.mousePosition - lastPosition.Value;
            lastPosition = Input.mousePosition;
            mouseDragged = mouseDragged || PressedDragDeltaThisFrame.Value.sqrMagnitude > float.Epsilon;
        }

        bool release = false;

        if (OnMouseUp) {
            if (!cxUISystemUtil.IsMousePointerOnUI ()) {
                if (!mouseDragged) {
                    ScreenClickedThisFrame = Input.mousePosition;
                    pressTime = Time.realtimeSinceStartup - downTime;
                }
            }

            release = true;
        }

        ScrollDeltaThisFrame = Input.mouseScrollDelta;

        if(clickAtLongPress && beginPosition.HasValue && !mouseDragged) {
            pressTime = Time.realtimeSinceStartup - downTime;
            if(pressTime > 1) {
                ScreenClickedThisFrame = Input.mousePosition;
                release = true;
            }
        }

        if (release) {
            beginPosition = null;
            lastPosition = null;
            // mouseButton = -1;
        }
    }


}