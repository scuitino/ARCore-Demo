using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This uses NatCorder Asset
public class ScreenCaptureUI : MonoBehaviour {


    [SerializeField] protected Button screenshotButton;
    [SerializeField] protected Toggle recToggle;

    [SerializeField] protected ScreenshotPanel screenshotPanel;
    [SerializeField] protected ScreencastPanel screencastPanel;

    protected enum State
    {
        IDLE,

        HANDLING_SCRENSHOT,
        HANDLING_SCREENCAST
    }
    protected StateMachine<State> state = new StateMachine<State>();


    protected virtual void Awake()
    {
        screenshotButton.onClick.AddListener(OnScreenshotClick);
		recToggle.onValueChanged.AddListener(OnRecClick);
    }

    // Use this for initialization
    protected virtual void Start () {
		
	}

    // Update is called once per frame
    protected virtual void Update () {
		
	}


    protected virtual void OnScreenshotClick()
    {
        state.SetState(State.HANDLING_SCRENSHOT);
        screenshotPanel.Show(true); // ScreenshotPanel captures screen and opens panel to handle it
    }


    protected virtual void OnRecClick(bool toggleState)
    {
        state.SetState(State.HANDLING_SCREENCAST);
        screencastPanel.OnRecToggle(toggleState);
    }
    
    // Call it when finished handling screenshot and cast - currently assigned on Back buttons
    public virtual void OnHandlingFinished()
    {
        state.SetState(State.IDLE);
        screenshotPanel.Show(false);
        screencastPanel.Show(false);
    }
}
