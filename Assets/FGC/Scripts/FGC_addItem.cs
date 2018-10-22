using UnityEngine;
using System.Collections;

public class FGC_addItem : MonoBehaviour {

	// gear, cloth etc. with SkinnedMeschRendered you want to add to this gameObject with bones
	public GameObject addItem1=null;
	public GameObject addItem2=null;


	void Start () {
		if (addItem1!=null) {
			AddNewItem(addItem1);
		}
		else {
			Debug.Log("FGC_addItem Script. No Item1 found.");
		}
		if (addItem2!=null) {
			AddNewItem(addItem2);
		}
		else {
			Debug.Log("FGC_addItem Script. No Item2 found.");
		}
	}


	 
	private void AddNewItem(GameObject item)
	{
		// find SkinnedMeschRendered on this gameObject
		var existingItem = GetComponentInChildren<SkinnedMeshRenderer>();

		var newObj = Instantiate<GameObject>(item);
		foreach(var r in newObj.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			// move the SkinnedMeshRenderer object from instantiated item to this object
			// and change its rootBone and bones properties
			r.transform.parent = transform;
			//r.transform.name = item.name; //se eliminato mostra i nomi di sub-item
			r.rootBone = existingItem.rootBone;
			r.bones = existingItem.bones;
		}
		Destroy(newObj);
	}
}