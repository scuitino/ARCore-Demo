using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Behavior to handle Location Service startup
public class LocationHandler : MonoBehaviour {

    [SerializeField] bool startCompass;

    [SerializeField] float DESIRED_GPS_ACCURACY = 10f;
    [SerializeField] float UPDATE_DISTANCE = 10f;

    int INITIALIZATION_TIMEOUT = 20;

    public UnityEvent onUserDisabledLocation;
    public UnityEvent onInitTimeout;
    public UnityEvent onInitFailed;
    public UnityEvent onInitOK;
    [Serializable] public class NewDataEvent : UnityEvent<LocationInfo> { } 
    public NewDataEvent onNewData = new NewDataEvent();
    public UnityEvent onStopped;


    public enum State
    {
        IDLE,

        INITIALIZING,
        USER_DISABLED_LOCATION,
        INIT_TIMEOUT,
        INIT_FAILED,
        
        RUNNING,
        STOPPED
    }
    StateMachine<State> state = new StateMachine<State>(); 

    // Use this for initialization
    IEnumerator Start()
    {
        state.SetState(State.INITIALIZING);

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            state.SetState(State.USER_DISABLED_LOCATION);
            Debug.LogError("User disabled Location");
            onUserDisabledLocation.Invoke(); //TODO:: Should we ask the user to enable Location?
            yield break;
        }

        // Start service before querying location
        int maxWait = INITIALIZATION_TIMEOUT;
        if (startCompass)
        {
            Input.compass.enabled = true;
            while (!Input.compass.enabled)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }
        }

        Input.location.Start(DESIRED_GPS_ACCURACY, UPDATE_DISTANCE);

        // Wait until service initializes
        maxWait = INITIALIZATION_TIMEOUT;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            state.SetState(State.INIT_TIMEOUT);
            Debug.LogError("Timed out");
            onInitTimeout.Invoke();
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            state.SetState(State.INIT_FAILED);
            Debug.LogError("Unable to determine device location. LocationServiceStatus.Failed");
            onInitFailed.Invoke();
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            state.SetState(State.RUNNING);
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            onInitOK.Invoke();
        }
    }
    
    private void OnApplicationFocus(bool focus)
    {
        if (state == State.USER_DISABLED_LOCATION && focus)
        {
            if (Input.location.isEnabledByUser)
            {
                Debug.Log("User now enabled Location.");
                StartCoroutine(Start());
            }
        }
    }

    LocationInfo? lastData = null;

    bool EqualLocation(LocationInfo loc1, LocationInfo loc2)
    {
        if (loc1.latitude != loc2.latitude)
            return false;
        if (loc1.longitude != loc2.longitude)
            return false;

        return true;
    }

    // Update is called once per frame
    void Update () {
        


        if (state == State.RUNNING)
        {
            if (Input.location.status != LocationServiceStatus.Running)
            {
                state.SetState(State.STOPPED);
                Debug.LogError("Location should be running but now is: "+Input.location.status);
                onStopped.Invoke();
                return;
            }

            if (!lastData.HasValue || !EqualLocation(Input.location.lastData, lastData.Value))
            {
                lastData = Input.location.lastData;
                onNewData.Invoke(lastData.Value);
            }
        }
    }
}
