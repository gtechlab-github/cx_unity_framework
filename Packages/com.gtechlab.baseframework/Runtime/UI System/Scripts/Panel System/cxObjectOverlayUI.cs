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

    [SerializeField] protected ScaleProperties scaleProperties;

    protected Canvas sortingCanvas;
    protected Transform objectAnchor;
    protected RectTransform rectTransform;

    protected virtual void Awake () {
        sortingCanvas = GetComponent<Canvas> ();
        rectTransform = GetComponent<RectTransform> ();
    }

    public void SetObjectAnchor (Transform anchor) {
        this.objectAnchor = anchor;
    }

    protected virtual void LateUpdate () {
        if (objectAnchor == null) {
            return;
        }

        var worldPosition = objectAnchor.position;
        var screenPosition = Camera.main.WorldToScreenPoint (worldPosition);
        transform.position = screenPosition;

        bool isVisible = screenPosition.z > 0;
        sortingCanvas.enabled = isVisible;

        if (isVisible) {
            sortingCanvas.sortingOrder = -(int) (screenPosition.z * 1000);
        }

        if (scaleProperties.applyDynamicScale) {
            // 카메라와의 거리에 따라 UI 크기 조절
            float distance = Vector3.Distance (Camera.main.transform.position, worldPosition);
            float scale = Mathf.Clamp (scaleProperties.baseScale * (scaleProperties.scaleDistance / distance), scaleProperties.minScale, scaleProperties.maxScale);
            rectTransform.localScale = Vector3.one * scale;
        }
    }
}