using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CompassCalibration : MonoBehaviour {

    public class ErrorEvent : UnityEvent<string> { }
    [SerializeField] public ErrorEvent onError = new ErrorEvent();

    public class ProgressEvent : UnityEvent<float> { }
    [SerializeField] public ProgressEvent onProgress = new ProgressEvent();

    [SerializeField] public UnityEvent onReady = new UnityEvent();

    const float CALIBRATION_TIMEOUT = 60f;

    enum State
    {
        IDLE,
        CALIBRATING,
        ERROR,
        READY
    }
    StateMachine<State> state = new StateMachine<State>();

    float startTime;
    Quaternion startOrientation;
    Quaternion startOrientationInverse;
    Vector3 minRange;
    Vector3 maxRange;
    float progress;

    private void Start()
    {

    }

    public void StartCalibrating()
    {
        if (!SystemInfo.supportsGyroscope)
        {
            state.SetState(State.ERROR);
            Debug.LogError("NO GYRO AVAILABLE");
            onError.Invoke("NO GYRO AVAILABLE");
            return;
        }
        
        Input.gyro.enabled = true;
        if (!Input.gyro.enabled)
        {
            state.SetState(State.ERROR);
            Debug.LogError("NO GYRO ENABLED");
            onError.Invoke("NO GYRO ENABLED");
            return;
        }

        startTime = Time.time;
        startOrientation = MathTools.GyroToUnity(Input.gyro.attitude);
        startOrientationInverse = Quaternion.Inverse(startOrientation);
        minRange = Vector3.zero;
        maxRange = Vector3.zero;
        progress = 0f;
        state.SetState(State.CALIBRATING);

        Debug.Log("Start Calibrating "+startOrientation.ToString("F4")+" "+startOrientationInverse.ToString("F4")+ " "+Input.gyro.rotationRate.ToString("F4"));
    }

    public void Update()
    {
        if (state.CurrentState != State.CALIBRATING)
            return;
        if (!SystemInfo.supportsGyroscope || !Input.gyro.enabled)
        {
            state.SetState(State.ERROR);
            Debug.LogError("UPD NO GYRO");
            onError.Invoke("UPD NO GYRO");
            return;
        }                
        if (Time.time - startTime > CALIBRATION_TIMEOUT)
        {
            state.SetState(State.ERROR);
            Debug.LogError("CALIBRATION TIMEOUT");
            onError.Invoke("CALIBRATION TIMEOUT");
            return;
        }

        Quaternion offset = MathTools.GyroToUnity(Input.gyro.attitude) * startOrientationInverse;
        Vector3 offsetEuler = offset.eulerAngles; // In range [0..360]
        Vector3 offsetEuler180 = MathTools.EulerAnglesToNegative(offsetEuler); // In range [-180..180]

        for(int i=0; i<3; i++)
        {
            if (offsetEuler180[i] < minRange[i])
                minRange[i] = offsetEuler180[i];
            if (offsetEuler180[i] > maxRange[i])
                maxRange[i] = offsetEuler180[i];
        }

        //if (offsetEuler180.x < minRange.x)
        //    minRange.x = offsetEuler180.x;
        //if (offsetEuler180.x > maxRange.x)
        //    maxRange.x = offsetEuler180.x;

        //if (offsetEuler180.y < minRange.y)
        //    minRange.y = offsetEuler180.y;
        //if (offsetEuler180.y > maxRange.y)
        //    maxRange.y = offsetEuler180.y;

        //if (offsetEuler180.z < minRange.z)
        //    minRange.z = offsetEuler180.z;
        //if (offsetEuler180.z > maxRange.z)
        //    maxRange.z = offsetEuler180.z;

        float progressX = -minRange.x + maxRange.x;
        float progressY = -minRange.y + maxRange.y;
        float progressZ = -minRange.z + maxRange.z;
        Debug.Log("off:"+offsetEuler.ToString("F0")+" min:"+minRange.ToString("F2")+ "max:" + maxRange.ToString("F2")+" prog: "+progressX.ToString("F2")+", "+ progressY.ToString("F2")+", "+ progressZ.ToString("F2"));

        progress = progressY / 180f; //reach 180 degrees to be ready
        onProgress.Invoke(progress);

        if (progress >= 1f)
        {
            state.SetState(State.READY);
            onReady.Invoke();
        }
    }
}
