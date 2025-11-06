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

    // Auto Managed Properties
    protected float opacityDistanceFactor = 1;
    protected float scaleDistanceFactor = 1;

    // Manual Managed Properties
    protected bool manualManaged { get; private set; } = false;
    protected float manualOpacity = 1;
    protected float manualScale = 1;
    protected int manualSortingOrder = 0;

    protected Camera overlayCamera;
    protected RectTransform overlayCanvas;

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
        //this.canvasOffset = canvasOffset;
        transform.SetParent (canvas);
    }

    public void SetManualManaged (bool manualManaged) {
        this.manualManaged = manualManaged;
    }

    public void SetManualProperties (float? opacity, float? scale, int? sortingOrder) {
        this.manualOpacity = opacity ?? this.manualOpacity;
        this.manualScale = scale ?? this.manualScale;
        this.manualSortingOrder = sortingOrder ?? this.manualSortingOrder;
    }

    /// <summary>
    /// 실시간 거리에 따른 투명도 조절 계수 설정
    /// </summary>
    /// <param name="opacityDistanceFactor">거리에 따른 투명도 조절 계수
    ///     0 에 가까울 수록 가까운 거리에서만 보임, 0이면 투명 상태
    ///     값이 높을 수록  멀리 있어도 보임, 
    /// </param>
    public void SetDistanceOpacity (float opacityDistanceFactor) {
        this.opacityDistanceFactor = opacityDistanceFactor;
    }

    /// <summary>
    /// 실시간 거리에 따른 크기 조절 계수 설정
    /// </summary>
    /// <param name="scaleDistanceFactor">거리에 따른 크기 조절 계수
    ///     0 에 가까울 수록 -> 멀리서 더 커짐
    ///     값이 높을 수록 -> 멀리서 더 작아짐
    /// </param>
    public void SetDistanceScale (float scaleDistanceFactor) {
        this.scaleDistanceFactor = scaleDistanceFactor;
    }

    protected virtual void LateUpdate () {
        // LateUpdate_MainCamera ();
        LateUpdate_3rdCamera ();
        // if (overlayCamera == Camera.main || overlayCamera == null) {
        //     LateUpdate_MainCamera ();
        // } else {
        //     LateUpdate_3rdCamera ();
        // }
    }
    /*
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
                canvasGroup.alpha = opacity * manualOpacity;
            }
        }
    */
    protected virtual void LateUpdate_3rdCamera () {
        if (objectAnchor == null) {
            return;
        }

        if (overlayCamera == null) {
            return;
        }

        var camera = overlayCamera ?? Camera.main;
        /*
               

                var worldPosition = objectAnchor.position;
                var vp = camera.WorldToViewportPoint (worldPosition);

                float width = overlayCanvas.rect.width;
                float height = overlayCanvas.rect.height;

                float px = vp.x * width;
                float py = vp.y * height;

                Vector3 screenPosition = new Vector3 (px, py, vp.z);
        */
        var worldPosition = objectAnchor.position;
        var screenPosition = camera.WorldToScreenPoint (objectAnchor.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle (
            overlayCanvas, // 부모 RectTransform
            screenPosition, // renderCam 기준 픽셀좌표
            null, // camera, // 이 카메라로 해석
            out Vector2 localPt);

        (transform as RectTransform).anchoredPosition = localPt;
        // transform.localPosition = screenPosition;

        bool isVisible = screenPosition.z > 0;
        sortingCanvas.enabled = isVisible;

        if (manualManaged) {
            canvasGroup.alpha = manualOpacity;
            rectTransform.localScale = Vector3.one * manualScale;
            sortingCanvas.sortingOrder = manualSortingOrder;
        } else {

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
}