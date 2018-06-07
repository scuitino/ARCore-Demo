using System;
using System.Collections;
using System.Collections.Generic;
using UnityARInterface;
using UnityEngine;
using UnityEngine.Events;

public class GeolocatedWorldObjects : WorldObjects {


    [Serializable] public class GPSEvent : UnityEvent<LocationTools.LocationData> { }
    [Space]
    [SerializeField] GPSEvent onGPSData = new GPSEvent();

    [Serializable] public class CompassEvent : UnityEvent<float> { }
    [SerializeField] CompassEvent onCompassData = new CompassEvent();

    [Serializable] public class PlaneEvent : UnityEvent<BoundedPlane> { }
    [SerializeField] PlaneEvent onPlane = new PlaneEvent();



    public SelectPointInInfinitePlane selectPointScript;

    bool hasDetectedPlane = false;
    BoundedPlane detectedPlane;

    bool hasGPSData = false;
    LocationTools.LocationData gpsData;
    
    bool hasCompassData = false;
    float compassHeading;

    bool objectInstantiated = false;


    public LocationTools.LocationData editorDebugGPSData;
    public float editorDebugHeading;
    public GameObject editorDebugScaleReference;



    private void Awake()
    {
        selectPointScript.onFirstPlaneDetected.AddListener(OnFirstPlaneDetected);
    }

    private void Start()
    {
        Debug.Log("LOCATION status: " +Input.location.status.ToString()+" enabledByUser?: "+Input.location.isEnabledByUser);
        Input.location.Start();
        Input.compass.enabled = true;
        Debug.Log("LOCATION status: " + Input.location.status.ToString() + " enabledByUser?: " + Input.location.isEnabledByUser);
    }

    bool EqualLocations(LocationInfo loc1, LocationInfo loc2)
    {
        if (loc1.latitude != loc2.latitude)
            return false;
        if (loc1.longitude != loc2.longitude)
            return false;

        return true;
    }
    private void Update()
    {   

        if (!objectInstantiated)
        {
            if (Input.compass.enabled &&
                (!hasCompassData || compassHeading != Input.compass.trueHeading))
            {
                compassHeading = Input.compass.trueHeading;
                onCompassData.Invoke(compassHeading);
                hasCompassData = true;
            }
            if (Input.location.status == LocationServiceStatus.Running && 
                (!hasGPSData || 
                gpsData.latitude != Input.location.lastData.latitude ||
                gpsData.longitude != Input.location.lastData.longitude))
            {
                gpsData = new LocationTools.LocationData();
                gpsData.latitude = Input.location.lastData.latitude;
                gpsData.longitude = Input.location.lastData.longitude;
                onGPSData.Invoke(gpsData);
                hasGPSData = true;
            }

#if UNITY_EDITOR
            compassHeading = editorDebugHeading;
            onCompassData.Invoke(compassHeading);
            hasCompassData = true;

            gpsData = editorDebugGPSData;
            onGPSData.Invoke(gpsData);
            hasGPSData = true;
#endif

            CheckInstantiation();
        }
    }


    public virtual void OnFirstPlaneDetected(BoundedPlane plane)
    {

        // DISABLE AR scripts
        selectPointScript.enabled = false;
        ARPointCloudVisualizer pointCloud = selectPointScript.GetComponent<ARPointCloudVisualizer>();
        if (pointCloud != null)
        {
            pointCloud.enabled = false;
        }

        detectedPlane = plane;
        onPlane.Invoke(detectedPlane);
        hasDetectedPlane = true;

#if UNITY_EDITOR
        editorDebugScaleReference.SetActive(true);
        editorDebugScaleReference.transform.position = detectedPlane.center;
#endif

        CheckInstantiation();
    }

    void CheckInstantiation()
    {
        if (objectInstantiated)
            return; // one instance only
        if (!hasDetectedPlane)
            return; // wait plane is detected
        if (!hasGPSData)
            return; // wait GPS data
        if (!hasCompassData)
            return; // wait Compass data

        // Instantiate Now
        objectInstantiated = InstantiateLibraryObject(objectLibrary.SelectedLibraryObject);
    }


    public override bool InstantiateLibraryObject(LibraryObject libObj)
    {
        //return base.InstantiateLibraryObject(libObj);

        // DONT raycast planes, but place it by GPS difference
        if (!hasDetectedPlane || !hasGPSData || !hasCompassData)
            return false;

        GeolocatedObject geoObj = libObj as GeolocatedObject;
        if (geoObj==null)
        {
            Debug.Log("object is not Geolocated");
            return false;
        }
        
        LocationTools.LocationData to = new LocationTools.LocationData();
        to.latitude = geoObj.latitude;
        to.longitude = geoObj.longitude;
        Vector3 offset = LocationTools.GetMetersDistanceHeadingNorth(gpsData, to);

        Vector3 camPosOnFloor = detectedPlane.plane.ClosestPointOnPlane(Camera.main.transform.position);
        //Vector3 worldPos = camOnPlane + Quaternion.identity * offset; // assuming north is forward for now

        // ORIENT North to apply offset
        Vector3 headingVector = Camera.main.transform.up; // Cam up is heading compassHeading (top of screen vector)
        if (Camera.main.transform.forward.y >= 0)
        {
            // If cam is looking to the sky - then we have to use the FWD instead, since compass heading seems to be in accordance with Forward instead of Up
            headingVector = Camera.main.transform.forward;
        }
        //-project into the plane and get north in plane
        Vector3 headingOnFloor = Vector3.ProjectOnPlane(headingVector, detectedPlane.plane.normal).normalized;
        Vector3 northOnFloor = Quaternion.AngleAxis(-compassHeading, Vector3.up) * headingOnFloor;
        northOnFloor = northOnFloor.normalized;
        Vector3 worldPos = camPosOnFloor + Quaternion.FromToRotation(Vector3.forward, northOnFloor) * offset;

        Debug.Log("Instantiating Geolocated obj. GPSData: (" + gpsData.latitude + "," + gpsData.longitude + ")" +
            " CompassHeading: " + compassHeading+
            " metersDist: "+offset.ToString("F2")+
            " Resulting worldPos: "+worldPos.ToString("F2")
        );

        GameObject obj = InstantiateLibraryObject(libObj, worldPos);
        //obj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, northOnPlane);// when heading is exactly 180.0 Unity AngleAxis chooses to make a rotation in X axis instead of Y, turning things up-down
        obj.transform.rotation = Quaternion.LookRotation(northOnFloor, Vector3.up); // Orient North - model is done with fwd North
        return true;
    }
}
