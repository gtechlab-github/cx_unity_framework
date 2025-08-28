using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cxUIDynamicGridList : MonoBehaviour {

	public GameObject prefabItem;
	public GameObject prefabDummy;
	public Transform gridLayout;

	public int lineCount;

	List<GameObject> m_itemList = new List<GameObject> ();
	List<GameObject> m_dummyList = new List<GameObject> ();

	public List<GameObject> list { get { return m_itemList; } }

	public void Init () {
		for (int j = 0; j < m_itemList.Count; j++) {
			GameObject.DestroyImmediate (m_itemList[j]);
		}
		m_itemList.Clear ();
	}

	public void BeginListBuild () {

	}

	public GameObject GetItemListObject (int i) {
		if (i < m_itemList.Count)
			return m_itemList[i];
		else {
			GameObject go = AddChild (gridLayout, prefabItem) as GameObject;

			go.name = (100 + i).ToString ();
			m_itemList.Add (go);
			return go;
		}
	}

	public T GetItemListObject<T> (int i) where T : MonoBehaviour {
		// if (i < m_itemList.Count)
		// 	return m_itemList[i].GetComponent<T> ();
		// else {
		// 	GameObject go = AddChild (gridLayout, prefabItem) as GameObject;

		// 	go.name = (100 + i).ToString ();
		// 	m_itemList.Add (go);
		// 	return go.GetComponent<T> ();
		// }

		return GetItemListObject<T> (prefabItem.GetComponent<T>(), i);
	}

	public T GetItemListObject<T> (T otherPrefab, int i) where T : MonoBehaviour {
		int replaceSiblingIndex = -1;

		if (i < m_itemList.Count) {
			var comp = m_itemList[i].GetComponent<T> ();
			if (comp == null) {
				m_itemList[i].SetActive (false);
				GameObject.Destroy (m_itemList[i]);
				m_itemList[i] = null;
				replaceSiblingIndex = i;
			} else {
				return comp;
			}
		}

		GameObject go = AddChild (gridLayout, otherPrefab.gameObject) as GameObject;
		go.name = (100 + i).ToString ();
		if (replaceSiblingIndex != -1) {
			go.transform.SetSiblingIndex (replaceSiblingIndex);
			m_itemList[replaceSiblingIndex] = go;
		} else {
			go.transform.SetAsLastSibling ();
			m_itemList.Add (go);
		}

		var comp2 = go.GetComponent<T> ();
		// if (comp2 == null) {
		// 	throw new System.Exception ("GetItemListObject<T> : " + go.name + " is not " + typeof (T).Name);
		// }

		return comp2;
	}

	public void AddDummyObject (int count) {
		for (int j = 0; j < m_dummyList.Count; j++) {
			GameObject.Destroy (m_dummyList[j]);
		}

		for (int i = 0; i < count; i++) {
			GameObject go = AddChild (gridLayout, prefabDummy) as GameObject;

			go.name = (10000 + i).ToString ();
			m_dummyList.Add (go);
		}
	}

	public void EndListBuild (int used, bool refreshLayout = true) {
		ClearUnusedItemList (used);
		if (refreshLayout) {
			RefeshLayout ();
		}
	}

	public void ClearUnusedItemList (int used) {
		if (used < m_itemList.Count) {
			int remove = 0;
			for (int j = used; j < m_itemList.Count; j++) {
				GameObject.Destroy (m_itemList[j]);
				remove++;
			}

			if (remove > 0)
				m_itemList.RemoveRange (used, remove);
		}

		if (prefabDummy != null && lineCount > 0) {
			int need = lineCount - m_itemList.Count;
			need = Mathf.Max (need, 0);
			if (m_dummyList.Count > need) {
				int remove = 0;
				for (int j = need; j < m_dummyList.Count; j++) {
					GameObject.Destroy (m_dummyList[j]);
					remove++;
				}

				if (remove > 0)
					m_dummyList.RemoveRange (need, remove);
			} else {
				for (int j = m_dummyList.Count; j < need; j++) {
					GameObject go = AddChild (gridLayout, prefabDummy);
					//go.name = (900 + j).ToString();
					go.name = "9999999";
					m_dummyList.Add (go);
				}
			}
		}
	}

	public void RefeshLayout () {
		cxUISystemUtil.RefreshLayout(gridLayout, true);
		//LayoutRebuilder.ForceRebuildLayoutImmediate ((RectTransform) gridLayout.transform);
	}

	static public GameObject AddChild (Transform parent, GameObject prefab) {
		GameObject go = Instantiate (prefab) as GameObject;
		int layer = -1;

		//		#if UNITY_EDITOR
		//		if (undo && !Application.isPlaying)
		//			UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
		//		#endif
		if (parent != null) {
			Transform t = go.transform;
			t.SetParent (parent);
			//t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			if (layer == -1) go.layer = parent.gameObject.layer;
			else if (layer > -1 && layer < 32) go.layer = layer;
		}
		return go;
	}
}