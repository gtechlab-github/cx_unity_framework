using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (RectTransform))]
public class cxUIViewportCanvas : MonoBehaviour {

    private RectTransform sceneViewRectT;
    private new Camera camera;
    private float keepWidth = 0;
    private float keepHeight = 0;

    void Awake () {
        sceneViewRectT = GetComponent<RectTransform> ();
    }

    public void CaptureCamera (Camera camera) {
        this.camera = camera;
        OnScreenResized ();
    }

    public void ReleaseCamera () {
        if (camera) {
            this.camera.rect = new Rect (0, 0, 1, 1);
            this.camera = null;
        }
    }

    Vector3 GetWorldBottomLeft (RectTransform rectTransform) {
        // RectTransform의 크기와 피벗을 고려하여 로컬 좌표에서 왼쪽 하단 지점을 계산합니다.
        Vector2 localBottomLeft = new Vector2 (
            -rectTransform.pivot.x * rectTransform.rect.width, 
            -rectTransform.pivot.y * rectTransform.rect.height);

        // 로컬 좌표를 월드 좌표로 변환합니다.
        Vector3 worldBottomLeft = rectTransform.TransformPoint (localBottomLeft);
        return worldBottomLeft;
    }

    Vector3 GetWorldTopRight (RectTransform rectTransform) {
        // RectTransform의 크기와 피벗을 고려하여 로컬 좌표에서 오른쪽 상단 지점을 계산합니다.
        Vector2 localTopRight = new Vector2 (
            (1 - rectTransform.pivot.x) * rectTransform.rect.width,
            (1 - rectTransform.pivot.y) * rectTransform.rect.height
        );

        // 로컬 좌표를 월드 좌표로 변환합니다.
        Vector3 worldTopRight = rectTransform.TransformPoint (localTopRight);
        return worldTopRight;
    }

    void OnScreenResized () {

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        Vector3 bottomLeft = GetWorldBottomLeft (sceneViewRectT);
        Vector3 topRight = GetWorldTopRight (sceneViewRectT);

        float w = (topRight.x - bottomLeft.x) / screenWidth;
        float h = (topRight.y - bottomLeft.y) / screenHeight;

        float x = bottomLeft.x / screenWidth;
        float y = bottomLeft.y / screenHeight;

        camera.rect = new Rect (x, y, w, h);

        keepWidth = sceneViewRectT.rect.width;
        keepHeight = sceneViewRectT.rect.height;
    }

    void Update () {
        if (camera) {
            if (!Mathf.Approximately (keepWidth, sceneViewRectT.rect.width) ||
                !Mathf.Approximately (keepHeight, sceneViewRectT.rect.height)) {
                OnScreenResized ();
            }
        }
    }
}