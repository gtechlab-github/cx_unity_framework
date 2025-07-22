using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Button))]
public class cxUIMultiToggleWidget : MonoBehaviour
{
    [SerializeField] private List<GameObject> buttons;
    [SerializeField] private bool isPingPong = false;
    [SerializeField] private int selectedNo = 0;
   
    public int SelectedNo => selectedNo;
    public UnityEngine.Events.UnityEvent<int> OnSelected {get;private set;}= new UnityEngine.Events.UnityEvent<int> ();
    private int pingPongDir = 1;

    void Awake()
    {
        GetComponent<Button> ().onClick.AddListener (() => {
            OnSelect(GetNextIndex());
            OnSelected.Invoke (selectedNo);
        });  

        OnSelect(selectedNo);
    }

    int GetNextIndex()
    {
        if(isPingPong)
        {
            var next = selectedNo + pingPongDir;
            if(next >= buttons.Count)
            {
                pingPongDir = -1;
                next = selectedNo + pingPongDir;
            }else if(next < 0)
            {
                pingPongDir = 1;
                next = selectedNo + pingPongDir;
            }
            return next;
        }
        else
        {
            return  (selectedNo + 1) % buttons.Count;
        }
    }

    public void SwitchButtonWithoutNotify(int index)
    {
        OnSelect(index);
    }

    public void SwitchButton(int index)
    {
        OnSelect(index);
        OnSelected.Invoke (selectedNo);
    }

    void OnSelect(int index)
    {
        if(buttons.Count == 0)
            return;

        index = index % buttons.Count;
        selectedNo = index;
        for (int i = 0; i < buttons.Count; i++) {
            buttons[i].SetActive (selectedNo == i);
        }
    }

    void OnValidate()
    {
        OnSelect(selectedNo);
    }

}
