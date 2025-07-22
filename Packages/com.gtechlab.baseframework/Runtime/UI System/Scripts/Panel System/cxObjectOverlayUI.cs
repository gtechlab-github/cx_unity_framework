using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class cxObjectOverlayUI : MonoBehaviour {

    [Serializable]
    public class ScaleProperties {
        public bool applyDynamicScale = false;
        public float baseScale = 1f;
        public float minScale = 0.5f;
        public float maxScale = 2f;
        public float scaleDistance = 10f;
    }

    [Serializable]
    public class OpacityProperties {
        public bool applyDistanceFade = false;
        public bool applyObscureCheck = false;
        public float fadeMinDistance = 10f;
        public float fadeMaxDistance = 20f;
    }

    [SerializeField] protected ScaleProperties scaleProperties;
    [SerializeField] protected OpacityProperties opacityProperties;
    protected Canvas sortingCanvas;
    protected Transform objectAnchor;
    protected RectTransform rectTransform;
    protected CanvasGroup canvasGroup;

    protected float opacityDistanceFactor = 1;
    protected float scaleDistanceFactor = 1;

    private Camera overlayCamera;
    private RectTransform overlayCanvas;

    protected virtual void Awake () {
        sortingCanvas = GetComponent<Canvas> ();
        rectTransform = GetComponent<RectTransform> ();
        canvasGroup = GetComponent<CanvasGroup> ();
    }

    /// <summary>
    /// 오버레이 캔버스 설정
    /// </summary>
    /// <param name="anchor">오브젝트의 위치</param>
    /// <param name="canvas">오브젝트 UI를 위한 캔버스</param>
    /// <param name="camera">오브젝트 렌더링을 위해 사용한 카메라, 기본값은 Camera.main</param>

    public void SetObjectAnchor (Transform anchor, RectTransform canvas, Camera camera = null) {
        this.overlayCanvas = canvas;
        this.overlayCamera = camera;
        this.objectAnchor = anchor;
        this.scaleDistanceFactor = 1;
        this.opacityDistanceFactor = 1;

        transform.SetParent (canvas);
    }

    /// <summary>
    /// 실시간 거리에 따른 투명도 조절 계수 설정
    /// </summary>
    /// <param name="opacityDistanceFactor">거리에 따른 투명도 조절 계수</param>
    public void SetDynamicOpacity (float opacityDistanceFactor) {
        this.opacityDistanceFactor = opacityDistanceFactor;
    }

    /// <summary>
    /// 실시간 거리에 따른 크기 조절 계수 설정
    /// </summary>
    /// <param name="scaleDistanceFactor">거리에 따른 크기 조절 계수</param>
    public void SetDynamicScale (float scaleDistanceFactor) {
        this.scaleDistanceFactor = scaleDistanceFactor;
    }

    protected virtual void LateUpdate () {
        if (overlayCamera == Camera.main || overlayCamera == null) {
            LateUpdate_MainCamera ();
        } else {
            LateUpdate_3rdCamera ();
        }
    }

    protected virtual void LateUpdate_MainCamera () {
        if (objectAnchor == null) {
            return;
        }

        var camera = overlayCamera ?? Camera.main;

        var worldPosition = objectAnchor.position;
        var screenPosition = camera.WorldToScreenPoint (worldPosition);
        transform.position = screenPosition;

        bool isVisible = screenPosition.z > 0;
        sortingCanvas.enabled = isVisible;

        if (isVisible) {
            sortingCanvas.sortingOrder = -(int) (screenPosition.z * 100);
        }

        if (scaleProperties.applyDynamicScale) {
            // 카메라와의 거리에 따라 UI 크기 조절
            float distance = Vector3.Distance (Camera.main.transform.position, worldPosition) * scaleDistanceFactor;
            float scale = Mathf.Clamp (scaleProperties.baseScale * (scaleProperties.scaleDistance / distance), scaleProperties.minScale, scaleProperties.maxScale);
            rectTransform.localScale = Vector3.one * scale;
        }

        if (canvasGroup && opacityProperties.applyDistanceFade) {
            float distance = Vector3.Distance (Camera.main.transform.position, worldPosition) * opacityDistanceFactor;
            float opacity = Mathf.Lerp (1, 0, (distance - opacityProperties.fadeMinDistance) / (opacityProperties.fadeMaxDistance - opacityProperties.fadeMinDistance));
            canvasGroup.alpha = opacity;
        }
    }

    protected virtual void LateUpdate_3rdCamera () {
        if (objectAnchor == null) {
            return;
        }

        if (overlayCamera == null) {
            return;
        }

        var camera = overlayCamera;

        var worldPosition = objectAnchor.position;
        var vp = camera.WorldToViewportPoint (worldPosition);

        float width = overlayCanvas.rect.width;
        float height = overlayCanvas.rect.height;

        float px = vp.x * width;
        float py = vp.y * height;

        // Debug.Log ("overlayCanvas.sizeDelta: " + overlayCanvas.sizeDelta.x + " " + overlayCanvas.sizeDelta.y);
        // Debug.Log ("overlayCanvas.rect: " + width + " " + height);
        // Debug.Log ("vp: " + vp.x + " " + vp.y + " -> " + px + " " + py);

        Vector3 screenPosition = new Vector3 (px, py, vp.z);

        RectTransformUtility.ScreenPointToLocalPointInRectangle (
            overlayCanvas, // 부모 RectTransform
            screenPosition, // renderCam 기준 픽셀좌표
            camera, // 이 카메라로 해석
            out Vector2 localPt);

        (transform as RectTransform).anchoredPosition = localPt;
        // transform.localPosition = screenPosition;

        bool isVisible = vp.z > 0;
        sortingCanvas.enabled = isVisible;

        if (isVisible) {
            sortingCanvas.sortingOrder = -(int) (screenPosition.z * 100);
        }

        if (scaleProperties.applyDynamicScale) {
            // 카메라와의 거리에 따라 UI 크기 조절
            float distance = Vector3.Distance (Camera.main.transform.position, worldPosition) * scaleDistanceFactor;
            float scale = Mathf.Clamp (scaleProperties.baseScale * (scaleProperties.scaleDistance / distance), scaleProperties.minScale, scaleProperties.maxScale);
            rectTransform.localScale = Vector3.one * scale;
        }

        if (canvasGroup && opacityProperties.applyDistanceFade) {
            float distance = Vector3.Distance (Camera.main.transform.position, worldPosition) * opacityDistanceFactor;
            float opacity = Mathf.Lerp (1, 0, (distance - opacityProperties.fadeMinDistance) / (opacityProperties.fadeMaxDistance - opacityProperties.fadeMinDistance));
            canvasGroup.alpha = opacity;
        }
    }
}