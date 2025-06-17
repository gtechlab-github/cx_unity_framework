using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class cxUISystemUtil {

    public static bool CursorLocked {
        get { return Cursor.lockState == CursorLockMode.Locked; }
    }

    public static void SetCursorLock (bool toLock) {
#if UNITY_EDITOR

        if (toLock) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
#endif
    }

    public static bool IsMousePointerOnUI () {
        EventSystem system = UnityEngine.EventSystems.EventSystem.current;

        if (system != null) {
            //for web
            if (Input.touchCount > 0) {
                return system.IsPointerOverGameObject (Input.GetTouch (0).fingerId);

                // for (int i = 0; i < Input.touchCount; i++) {
                //     Touch touch = Input.GetTouch (i);
                //     if (system.IsPointerOverGameObject (touch.fingerId)) {
                //         return true;
                //     }
                // }

            } else {
                return system.IsPointerOverGameObject ();
            }
        }
        return false;
    }

    /// <summary>
    /// 현재 포인터가 UI 요소 위에 있는지 확인합니다. ( UI Layer Only)
    /// </summary>
    /// <returns>UI 요소 위에 있으면 true, 아니면 false</returns>
    public static bool IsPointerOnUIEx (int layerMask = -1) {
        EventSystem system = UnityEngine.EventSystems.EventSystem.current;

        if (Input.touchCount > 0) {
            return IsPointerOverUI (Input.GetTouch (0).position, layerMask);
        } else {
            return IsPointerOverUI (Input.mousePosition, layerMask);
        }
    }


    // /// <summary>
    // /// 특정 터치 ID에 대해 UI 요소 위에 포인터가 있는지 확인합니다.
    // /// </summary>
    // /// <param name="touchId">체크할 터치 ID</param>
    // /// <returns>UI 요소 위에 있으면 true, 아니면 false</returns>
    // public static bool IsTouchOverUI () {
    //     if (Input.touchCount == 0) return false;

    //     // for (int i = 0; i < Input.touchCount; i++) {
    //     //     Touch touch = Input.GetTouch (i);
    //     //     if (touch.fingerId == touchId) {
    //     //         return IsPointerOverUI (touch.position);
    //     //     }
    //     // }

    //     if (Input.touchCount > 0) {
    //         return IsPointerOverUI (Input.GetTouch (0).position);
    //     }

    //     return false;
    // }

    // static int _uiLayer = 0;

    // static int GetUILayer () {
    //     if (_uiLayer == 0) {
    //         _uiLayer = LayerMask.NameToLayer ("UI");
    //     }
    //     return _uiLayer;
    // }

    public static bool IsPointerOverUI (Vector2 position, int layerMask = -1) {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData (EventSystem.current);
        eventData.position = position;
        List<RaycastResult> results = new List<RaycastResult> ();
        EventSystem.current.RaycastAll (eventData, results);

        //int uiLayer = GetUILayer ();

        // GraphicRaycaster만 체크 (UI 요소만)
        return results.Exists (result => result.module is UnityEngine.UI.GraphicRaycaster
            && (layerMask == -1 || (layerMask & (1 << result.gameObject.layer)) != 0)
        );
    }

    public static void RefreshLayout (Transform transform, bool deep = false) {
        RefreshLayout (transform as RectTransform, deep);
    }

    public static void RefreshLayout (RectTransform rectTransform, bool deep = false) {
        LayoutRebuilder.MarkLayoutForRebuild (rectTransform);

        rectTransform.gameObject.GetComponent<MonoBehaviour> ().StartCoroutine (RefreshLayoutCoroutine (rectTransform, deep));
    }

    public static IEnumerator RefreshLayoutCoroutine (RectTransform rectTransform, bool deep) {
        if (deep) {
            LayoutRebuilder.ForceRebuildLayoutImmediate (rectTransform);
        }
        for (int i = 0; i < 2; i++) {
            yield return new WaitForEndOfFrame ();
            LayoutRebuilder.MarkLayoutForRebuild (rectTransform);

            // if (deep) {
            //     LayoutRebuilder.ForceRebuildLayoutImmediate (rectTransform);
            // } else {
            //     LayoutRebuilder.MarkLayoutForRebuild (rectTransform);
            // }
        }
    }
}