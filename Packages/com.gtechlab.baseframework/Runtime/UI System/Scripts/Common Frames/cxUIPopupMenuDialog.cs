using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class cxUIPopupMenuDialog : cxUIParameterFrame<cxUIPopupMenuDialog.FrameArgs> {

    public class FrameArgs {
        public Vector3 screenPosition;
        public string title;
        public List<string> commandList;
    }

    [SerializeField] private Button prefabMenuItem;
    [SerializeField] private RectTransform menuListRoot;
    [SerializeField] private GameObject titleObject;
    [SerializeField] private TMPro.TMP_Text titleText;

    [Header ("Margin for anchor (Canvas Size)")]
    [SerializeField] private float marginRight = 100;
    [SerializeField] private float marginBottom = 100;

    List<Button> menuList = new List<Button> ();

    protected override void OnInit () { }

    protected override void OnActivated (FrameArgs frameArgs) {
        if (!string.IsNullOrEmpty (frameArgs.title)) {
            titleText.text = frameArgs.title;
            titleObject.gameObject.SetActive (true);
        } else
            titleObject.gameObject.SetActive (false);

        //RepositionAnchor(frameArgs.screenPosition);
        RepositionAnchorEx (frameArgs.screenPosition);

        BuildMenu (frameArgs.commandList);
    }

    protected override void OnDeactivated () {
        //throw new System.NotImplementedException();
    }

    /*
    void RepositionAnchor(Vector3 screenPosition){
        float margin_x = 500;
        float margin_y = 500;

        Vector2 pivot = menuListRoot.pivot;

        Debug.Log("screenPosition: " + screenPosition);

        if(screenPosition.x > (Screen.width - margin_x)){
            pivot.x = 1;
        }else{
            pivot.x = 0;
        }

        if(screenPosition.y < margin_y){
            pivot.y = 0;
        }else{
            pivot.y = 1;
        }

        menuListRoot.pivot = pivot;
        menuListRoot.position = screenPosition;
    }
    */

    void RepositionAnchorEx (Vector3 screenPosition) {

        var targetCanvas = GetComponentInParent<Canvas> ();
        RectTransform canvasRect = targetCanvas.GetComponent<RectTransform> ();
        Vector2 canvasSize = canvasRect.rect.size;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle (
            canvasRect,
            screenPosition,
            targetCanvas.worldCamera,
            out localPoint
        );

        Vector2 pivot = menuListRoot.pivot;

        // Debug.Log("screenPosition: " + screenPosition);
        // Debug.Log("localPoint: " + localPoint);
        // Debug.Log("canvasSize: " + canvasSize);

        if (localPoint.x > (canvasSize.x / 2 - marginRight)) {
            pivot.x = 1;
        } else {
            pivot.x = 0;
        }

        if (localPoint.y < (-canvasSize.y / 2 + marginBottom)) {
            pivot.y = 0;
        } else {
            pivot.y = 1;
        }

        menuListRoot.pivot = pivot;
        menuListRoot.localPosition = localPoint;
    }
    void BuildMenu (List<string> commandList) {
        foreach (var button in menuList) {
            Destroy (button.gameObject);
        }
        menuList.Clear ();

        int idx = 0;
        foreach (var command in commandList) {
            int menuIdx = idx++;

            var button = Instantiate (prefabMenuItem, menuListRoot);
            button.SetLabel (command);
            button.onClick.RemoveAllListeners ();
            button.onClick.AddListener (() => {
                PopResult (menuIdx);
            });
            menuList.Add (button);
        }
    }

}