using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldObjects : SingletonBehaviourKeepLast<WorldObjects> {

    [SerializeField] protected ObjectLibrary objectLibrary;

    [Tooltip("Automatically add ManipulableObject behavior to instantiated objects to enable Move/Rotate/Scale")]
    [SerializeField] protected bool addManipulableBehaviorToInstances = false;

    public enum RaycastMode
    {
        MOUSE_POSITION,
        CAMERA_CENTER
    }
    public RaycastMode raycastMode = RaycastMode.MOUSE_POSITION;

    // EVENTS
    [System.Serializable] public class InstantiatedObjectEvent : UnityEvent<GameObject> { }
    [SerializeField] public InstantiatedObjectEvent onInstantiatedObject = new InstantiatedObjectEvent();

    [System.Serializable] public class SelectedObjectEvent : UnityEvent<GameObject> { }
    [SerializeField] public SelectedObjectEvent onSelectedObject = new SelectedObjectEvent();
    //

    protected List<GameObject> instantiatedObjects = new List<GameObject>();
    public int InstantiatedObjectsCount
    {
        get { return instantiatedObjects.Count; }
    }

    protected GameObject selectedObject = null;
    public virtual GameObject SelectedObject
    {
        get
        {
            return selectedObject;
        }

        set
        {
            GameObject go = value;
            if (go != null && !IsInstantiatedObject(go))
            {
                Debug.LogError("Gameobject not instantiated in Library");
                return;
            }

            // If go is currently selected - deselect it
            //if (go == SelectedInstantiatedObject)
            //{
            //    go = null;
            //}

            Debug.Log("SELECTED " + (go == null ? "null" : go.name));
            selectedObject = go;
            // change states of objects (highlight)
            foreach (GameObject other in instantiatedObjects)
            {
                ManipulableObject sel = other.GetComponent<ManipulableObject>();
                if (sel != null)
                    sel.Select(other == go);
            }
            onSelectedObject.Invoke(selectedObject);
        }
    }

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    //void Update () {

    //}

    public void ButtonInstantiateLibraryObject()
    {
        InstantiateLibraryObject();
    }
    
    public virtual bool InstantiateLibraryObject()
    {
        if (objectLibrary == null)
        {
            Debug.LogError("NO LIB");
            return false;
        }
        LibraryObject libObj = objectLibrary.SelectedLibraryObject;
        if (libObj == null)
        {
            Debug.Log("NO SEL OBJ");
            return false;
        }
        else
        {
            return InstantiateLibraryObject(libObj);
        }
    }
    public virtual bool InstantiateLibraryObject(LibraryObject libObj)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (raycastMode == RaycastMode.MOUSE_POSITION)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }else if (raycastMode == RaycastMode.CAMERA_CENTER)
        {
#if UNITY_EDITOR
            ray.direction = new Vector3(0f, -1f, 1f); // EDITOR hardcoded front down ray
#endif
        }


        int layerMask = 1 << LayerMask.NameToLayer("ARGameObject"); // Planes are in layer ARGameObject

        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, float.MaxValue, layerMask))
        {
            GameObject go = InstantiateLibraryObject(libObj, rayHit.point);
            return go != null;
        }
        Debug.Log("NO HIT");
        return false;
    }
    //Added this property in order to be able to assign it in inspector, for a UnityEvent dynamic Vector3
    public Vector3 InstantiateLibraryObjectAtPosition {
        set{
            InstantiateLibraryObject(value);
        }
    }
    public virtual bool InstantiateLibraryObject(Vector3 worldPos)
    {
        if (objectLibrary == null)
        {
            Debug.LogError("NO LIB");
            return false;
        }
        LibraryObject libObj = objectLibrary.SelectedLibraryObject;
        if (libObj == null)
        {
            Debug.Log("NO SEL OBJ");
            return false;
        }
        else
        {
            return InstantiateLibraryObject(libObj, worldPos) != null;
        }
    }
    public virtual GameObject InstantiateLibraryObject(LibraryObject libObj, Vector3 worldPos)
    {
        //Debug.Log("inst, ");
        GameObject go = libObj.Instantiate(worldPos);
        //Debug.Log("inst "+go.name);
        go.transform.SetParent(transform, true);
        
        ManipulableObject man = go.GetComponent<ManipulableObject>();
        if (man==null && addManipulableBehaviorToInstances)
        {
            go.AddComponent<ManipulableObject>();
            man = go.GetComponent<ManipulableObject>();

            man.boxCollider = go.GetComponent<BoxCollider>();
            man.VERTICAL_MOVEMENT_ENABLED = false;
            man.START_ENABLED = true;
        }

        Animator anim = go.GetComponentInChildren<Animator>();
        if (anim!=null)
        {
            Debug.Log("Object has an Animator");
        }

        if (man!=null)
            man.worldObjects = this;

        AddInstantiatedObject(go);
        SelectedObject = go; // Select it
        onInstantiatedObject.Invoke(go);
        return go;
    }

    public void DeleteSelectedObject()
    {
        if (SelectedObject == null)
            return;
        DeleteInstantiatedObject(selectedObject);
        SelectedObject = null;
    }


    public virtual void DeleteInstantiatedObject(GameObject go)
    {
        if (!IsInstantiatedObject(go))
        {
            Debug.LogError("Gameobject not instantiated in Library");
            return;
        }
        RemoveInstantiatedObject(go);
        Destroy(go);
    }



    public bool IsInstantiatedObject(GameObject go)
    {
        bool ret = instantiatedObjects.Contains(go);
        if (!ret)
            Debug.Log("IsInstantiatedObject : " + go.name + " : " + ret);
        return ret;
    }
    public GameObject FindInstantiatedParent(GameObject goAnyChild)
    {
        GameObject testGO = goAnyChild;
        while (testGO != null)
        {
            if (IsInstantiatedObject(testGO))
            {
                // Found a parent present in Library
                return testGO;
            }

            if (testGO.transform.parent == null)
                testGO = null;
            else
                testGO = testGO.transform.parent.gameObject;
        }
        return null;
    }
    public virtual void AddInstantiatedObject(GameObject go)
    {
        if (IsInstantiatedObject(go))
        {
            Debug.LogError("Gameobject already instantiated in Library");
            return;
        }
        instantiatedObjects.Add(go);
    }
    public virtual void RemoveInstantiatedObject(GameObject go)
    {
        if (!IsInstantiatedObject(go))
        {
            Debug.LogError("Gameobject not instantiated in Library");
            return;
        }
        instantiatedObjects.Remove(go);
        if (SelectedObject == go)
            SelectedObject = null; // Unselect it
    }
}
