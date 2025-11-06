using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (AspectRatioFitter))]
[RequireComponent (typeof (Image))]
[ExecuteInEditMode]
public class cxUINetImage : MonoBehaviour {

    public string url;

    [Tooltip("로딩 전 , 로딩 실패시 설정되는 이미지")]
    public Sprite defaultImg;
    [Tooltip("로딩 전 , 로딩 실패시, defaultImage 설정이 없을 경우 배경색")]
    public Color defaultColor = Color.gray;

    private Image image;
    private AspectRatioFitter aspectRatio;

    void Awake () {
        image = GetComponent<Image> ();
        aspectRatio = GetComponent<AspectRatioFitter> ();
    }

    private void Start () {
        if (!string.IsNullOrEmpty (url))
            Load (url);
        else
            SetEmpty ();
    }

    public async void Load (string url) {
        this.url = url;
        SetEmpty ();

        if (!string.IsNullOrEmpty (url)) {
            try {
                var loaded = await cxUniversalResourceLoader.Instance.LoadAsset<Sprite> (url);
                if (loaded) {
                    image.color = Color.white;
                    image.sprite = loaded;
                    aspectRatio.aspectRatio = (float) loaded.texture.width / (float) loaded.texture.height;
                }
            } catch (System.Exception ex) {
                Debug.LogWarning ($"cxUINetImage({url}) load failed:" + ex.Message);
            }
        }
    }

    public void LoadSprite (Sprite sprite) {
        if(sprite == null) {
            SetEmpty();
            return;
        }

        image.color = Color.white;
        image.sprite = sprite;
        aspectRatio.aspectRatio = (float) sprite.texture.width / (float) sprite.texture.height;
    }

    void SetEmpty () {
        if(image == null) {
            image = GetComponent<Image> ();
            aspectRatio = GetComponent<AspectRatioFitter> ();
        }

        if (defaultImg) {
            image.color = Color.white;
            image.sprite = defaultImg;
        } else {
            image.color = defaultColor;
            image.sprite = null;
        }
    }

    void OnValidate () {
        // Unity Editor에서 속성 변경 시 즉시 적용
        if (!Application.isPlaying) {
            if (!string.IsNullOrEmpty(url)) {
              //  Load(url);
            } else {
                SetEmpty();
            }
        }
    }
}