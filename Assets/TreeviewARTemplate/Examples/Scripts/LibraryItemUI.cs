using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibraryItemUI : MonoBehaviour {

    [SerializeField] Toggle toggle;
    [SerializeField] Image bg;
    [SerializeField] Image icon;
    [SerializeField] Image on;
    [SerializeField] Text label;

    ScrollLibrary scrollLib;
    LibraryObject libObj;

    // Use this for initialization
    void Start () {
		
	}
	
    public void Populate(ScrollLibrary scrollLib, LibraryObject libObj, ToggleGroup group = null)
    {
        this.scrollLib = scrollLib;
        this.libObj = libObj;

        label.text = libObj.displayName;
        if (libObj.thumbnail != null)
        {
            icon.enabled = true;
            icon.sprite = libObj.thumbnail;
        }
        if (group!=null)
        {
            toggle.group = group;
        }
    }

    public void OnToggleChanged(bool isOn)
    {
        Debug.Log("OnToggleChanged "+(isOn?"ON":""), gameObject);
        if (scrollLib!=null && isOn)
        {
            scrollLib.SelectLibraryObject(libObj);
        }
    }
    public void Deselect()
    {
        if (toggle.isOn)
        {
            toggle.isOn = false;
        }
    }

	// Update is called once per frame
	//void Update () {
		
	//}
}
