using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using NatCorderU.Core;
using NatShareU;

// This uses NatCorder Asset
public class ScreencastManager : MonoBehaviour {

    [Serializable]
    public class VideoReadyEvent : UnityEvent<string> { }
    public VideoReadyEvent onVideoReady = new VideoReadyEvent();

    [SerializeField] Camera recCamera;

    enum State
    {
        IDLE,
        RECORDING,
        RECORDED
    }
    StateMachine<State> state = new StateMachine<State>();


    //private void Awake()
    //{
    //}

    // Use this for initialization
 //   void Start () {
		
	//}
	
	// Update is called once per frame
	//void Update () {
		
	//}

    public void OnRecToggleClick()
    {
        if (state.CurrentState == State.IDLE || state.CurrentState == State.RECORDED)
        {
            StartRecording();
        }
        else if (state.CurrentState == State.RECORDING)
        {
            StopRecording();
        }
    }

    public void StartRecording()
    {
        state.SetState(State.RECORDING);

        if (recCamera!=null)
            Replay.StartRecording(recCamera, Configuration.Screen, OnReplay);
        else
            Replay.StartRecording(Camera.main, Configuration.Screen, OnReplay);
    }
    public void StopRecording()
    {
        if (!Replay.IsRecording)
        {
            state.SetState(State.IDLE);
            return;
        }

        state.SetState(State.RECORDED);
        Replay.StopRecording();
        
    }

    void OnReplay(string videoPath)
    {
        onVideoReady.Invoke(videoPath);

        //Debug.Log("Saved recording to: " + videoPath);
    }

    public void GetThumbnail(string videoPath, Action<Texture2D> callback)
    {
        NatShare.GetThumbnail(videoPath, callback);
    }
    public void Preview(string videoPath)
    {
        #if UNITY_IOS
            Handheld.PlayFullScreenMovie("file://" + videoPath);
        #elif UNITY_ANDROID
            Handheld.PlayFullScreenMovie(videoPath);
        #endif
    }
    public bool SaveToCameraRoll(string videoPath)
    {
        return NatShare.SaveToCameraRoll(videoPath);
    }
    public void Share(string videoPath)
    {
#if UNITY_IOS || UNITY_ANDROID
        NatShare.Share(videoPath);
#endif
    }
}
