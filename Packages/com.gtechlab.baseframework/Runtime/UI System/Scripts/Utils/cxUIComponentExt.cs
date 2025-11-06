using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public static class cxUIComponentExt {

    public static void SetLabel (this Button button, string label) {
        var text = button.GetComponentInChildren<Text> ();
        if (text) text.text = label;

        var tmp = button.GetComponentInChildren<TMPro.TMP_Text> ();
        if (tmp) tmp.text = label;
    }

     public static void SetLabel (this GameObject button, string label) {
        var text = button.GetComponentInChildren<Text> ();
        if (text) text.text = label;

        var tmp = button.GetComponentInChildren<TMPro.TMP_Text> ();
        if (tmp) tmp.text = label;
    }


    public static void SetAllColor (this RectTransform widget, Color color) {
        var images = FindAllComponents<Graphic> (widget);
        var texts = FindAllComponents<Text> (widget);
        var tmps = FindAllComponents<TMPro.TMP_Text> (widget);

        foreach (var image in images)
            image.color = color;

        foreach (var text in texts)
            text.color = color;

        foreach (var tmp in tmps)
            tmp.color = color;
    }


	public static List<T> FindAllComponents<T> (Transform model) where T : Component {
		List<T> comps = new List<T> ();

		var list = model.GetComponents<T> ();
		comps.AddRange (list);
		for (int i = 0; i < model.childCount; i++) {
			var list2 = FindAllComponents<T> (model.GetChild (i));
			if (list2 != null)
				comps.AddRange (list2);
		}

		return comps;
	}
}