using System.Collections;
using System.Collections.Generic;
using UnityARInterface;
using UnityEngine;
using UnityEngine.Events;

public class SelectPointInInfinitePlane : ARBase {

    [SerializeField] bool DEBUG = false;

    [SerializeField] bool disableThisAfterSelect = true;
    [SerializeField] bool disablePointCloudAfterSelect = true;

    // EVENTS
    [System.Serializable] public class FirstPlaneDetectedEvent : UnityEvent<BoundedPlane> { }
    [SerializeField] public FirstPlaneDetectedEvent onFirstPlaneDetected = new FirstPlaneDetectedEvent();

    [System.Serializable] public class FirstPlaneUpdatedEvent : UnityEvent<BoundedPlane> { }
    [SerializeField] public FirstPlaneUpdatedEvent onFirstPlaneUpdated = new FirstPlaneUpdatedEvent();

    [System.Serializable] public class PointSelectedEvent : UnityEvent<Vector3> { }
    [SerializeField] public PointSelectedEvent onPointSelected = new PointSelectedEvent();
    //

    enum State
    {
        DISABLED,
        SCANNING,
        FIRST_PLANE_DETECTED,
        POINT_SELECTED
    }
    StateMachine<State> state = new StateMachine<State>();

    string firstPlaneId = null;
    BoundedPlane firstPlane;
    public BoundedPlane GetBoundedPlane()
    {
        return firstPlane;
    }

    Vector3 selectedPoint;


    protected virtual void OnEnable()
    {
        ARInterface.planeAdded += PlaneAddedHandler;
        ARInterface.planeUpdated += PlaneUpdatedHandler;
        ARInterface.planeRemoved += PlaneRemovedHandler;

        state.debug = DEBUG;
        if (state.CurrentState == State.DISABLED)
            state.SetState(State.SCANNING);
    }

    protected virtual void OnDisable()
    {
        ARInterface.planeAdded -= PlaneAddedHandler;
        ARInterface.planeUpdated -= PlaneUpdatedHandler;
        ARInterface.planeRemoved -= PlaneRemovedHandler;

        //if (state.CurrentState == State.SCANNING)
        state.SetState(State.DISABLED);
    }

    // Use this for initialization
    void Start () {
		
	}

    

    protected virtual void PlaneAddedHandler(BoundedPlane plane)
    {
        if (firstPlaneId != null)
            return;

        firstPlaneId = plane.id;
        firstPlane = plane;
        if (DEBUG)
            Debug.Log("PlaneAdded c:"+firstPlane.center + " exts:"+firstPlane.extents);
        state.SetState(State.FIRST_PLANE_DETECTED);
        onFirstPlaneDetected.Invoke(plane);
    }

    protected virtual void PlaneUpdatedHandler(BoundedPlane plane)
    {
        if (firstPlaneId == null)
        {
            Debug.LogError("PlaneUpdate without firstPlaneId");
            return;
        }

        if (firstPlaneId == plane.id)
        {
            // Update plane position & rotation - until selecting a point
            firstPlane = plane;
            if (DEBUG)
                Debug.Log("PlaneUpd c:" + firstPlane.center + " exts:" + firstPlane.extents);
            onFirstPlaneUpdated.Invoke(plane);
        }
    }

    protected virtual void PlaneRemovedHandler(BoundedPlane plane)
    {
        if (firstPlaneId == null)
        {
            Debug.LogError("PlaneRemove without firstPlaneId");
            return;
        }

        if (firstPlaneId == plane.id)
        {
            Debug.LogWarning("PlaneRemove of firstPlaneId. Infinit Plane stays as it were before being removed");
        }
    }


    bool ValidMouseDown()
    {
        if (!Input.GetMouseButtonDown(0))
            return false;
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return false;
        if (Input.touchCount > 1)
            return false;
        if (Input.touchCount == 1 && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
            return false;

        // Throw a ray, if it intersects an instantiated object then this is not Valid to create a new one
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);
        if (hits.Length > 0)
        {
            Debug.Log("POINT INVALID, raycast hit: "+hits[0].transform.name);
            return false;
        }

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (state.CurrentState == State.FIRST_PLANE_DETECTED || state.CurrentState == State.POINT_SELECTED)
        {
            if (ValidMouseDown())
            {
                Vector3 vpPos = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0f);
                bool intersected = IntersectInfinitePlane(vpPos, out selectedPoint);
                if (intersected)
                {
                    state.SetState(State.POINT_SELECTED);
                    onPointSelected.Invoke(selectedPoint);

                    if (disableThisAfterSelect)
                    {
                        enabled = false;
                    }
                    if (disablePointCloudAfterSelect)
                    {
                        ARPointCloudVisualizer pointCloud = GetComponent<ARPointCloudVisualizer>();
                        if (pointCloud!=null)
                        {
                            pointCloud.enabled = false;
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Tap didnt intersected infinite plane.");
                }
            }
        }
    }

    bool IntersectInfinitePlane(Vector3 viewportPosition, out Vector3 worldPos)
    {
        if (!gameObject.activeSelf || firstPlaneId == null)
        {
            Debug.LogWarning("IntersectInfinitePlane with no plane yet!");
            worldPos = Vector3.zero;
            return false;
        }

        Ray ray = GetCamera().ViewportPointToRay(viewportPosition);
        //Debug.Log("Ray: "+ray.ToString());
//#if UNITY_EDITOR
//        ray = new Ray(Vector3.up, Vector3.forward + Vector3.down); // Debug ray to raycast floor plane in Editor
//#endif
        
        float d;
        bool hit = firstPlane.plane.Raycast(ray, out d);
        if (!hit)
        {
            worldPos = Vector3.zero;
            return false;
        }
        else
        {
            worldPos = ray.GetPoint(d);
            return true;
        }
    }
}
