using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectLibrary : MonoBehaviour {

    public UnityEvent<LibraryObject> OnLibraryObjectSelected;

    [Header("Library Objects")]
    [SerializeField]
    protected List<LibraryObject> libraryObjects = new List<LibraryObject>();

    [SerializeField] protected LibraryObject selectedLibraryObject = null;
    public virtual LibraryObject SelectedLibraryObject
    {
        get
        {
            return selectedLibraryObject;
        }
    }


    // Use this for initialization
    protected virtual void Start () {

        PopulateMenu();
    }

    public void SetLibraryObjects(List<LibraryObject> objects)
    {
        Debug.Log("SetLibraryObjects "+objects.Count);
        libraryObjects = objects;
        PopulateMenu();
    }
    public bool IsLibraryObject(LibraryObject libObj)
    {
        if (libObj == null)
            return true;
        bool ret = libraryObjects.Contains(libObj);
        if (!ret)
            Debug.Log("IsLibraryObject : " + libObj.ToString() + " : " + ret);
        return ret;
    }

    protected virtual void PopulateMenu()
    {
        return; // NO UI
    }

    public void SelectLibraryObject(int value)
    {
        if (value < 0 || value > libraryObjects.Count)
        {
            Debug.LogError("value invalid");
            return;
        }
        SelectLibraryObject(libraryObjects[value]);
    }
    public virtual void SelectLibraryObject(LibraryObject libObj)
    {
        if (!IsLibraryObject(libObj))
        {
            return;
        }
        selectedLibraryObject = libObj;
        if (OnLibraryObjectSelected != null)
            OnLibraryObjectSelected.Invoke(libObj);
    }
    // method signature to assign in Inspector
    public virtual void DeselectLibraryObject()
    {
        SelectLibraryObject(null);
    }
}
