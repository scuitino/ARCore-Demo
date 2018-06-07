using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreencastPanel : GenericPanel {
    
    [SerializeField] Image thumbnail;
    [SerializeField] Button saveBt;
    [SerializeField] Button shareBt;

    [SerializeField] ScreencastManager screencastManager;

    [SerializeField] GameObject savedOkPanel;

    string savedVideoPath;

    public enum State
    {
        IDLE,
        RECORDING,
        RECORDED,
        PREVIEWING,
        SAVING,
        SAVED,
        SHARING
    }
    StateMachine<State> state = new StateMachine<State>();

    protected virtual void Awake()
    {
        if (saveBt!=null)
            saveBt.onClick.AddListener(Save);
        if (shareBt != null)
            shareBt.onClick.AddListener(Share);

        screencastManager.onVideoReady.AddListener(OnVideoReady);

        state.onStateChanged2.AddListener(OnStateChanged);
        state.SetState(State.IDLE, true);
    }

    void OnStateChanged(State oldState, State newState)
    {
        thumbnail.gameObject.SetActive(state.CurrentState > State.RECORDING);

        if (saveBt != null)
            saveBt.gameObject.SetActive(state.CurrentState == State.RECORDED || state.CurrentState == State.PREVIEWING);
        if (shareBt != null)
            shareBt.gameObject.SetActive(state.CurrentState == State.RECORDED || state.CurrentState == State.PREVIEWING || state.CurrentState == State.SAVED);

        if (newState == State.SAVED)
        {
            if (savedOkPanel!=null)
                StartCoroutine(_ShowSavedOK());
        }
    }
    IEnumerator _ShowSavedOK()
    {
        savedOkPanel.SetActive(true);
        yield return new WaitForSeconds(5f);
        savedOkPanel.SetActive(false);
    }

    public override void Show(bool show)
    {
        if (!show)
        {
            // closing
            state.SetState(State.IDLE);
        }

        base.Show(show);

        if (show)
        {
            // opening
            //savedVideoPath = null;
            //StartRecording();
        }
    }

    public void OnRecToggle(bool toggleState)
    {
        if (state.CurrentState == State.IDLE)
        {
            StartRecording();
        }
        else if (state.CurrentState == State.RECORDING)
        {
            StopRecording();
        }
    }

    void StartRecording()
    {
        state.SetState(State.RECORDING);
#if UNITY_EDITOR
        // Couldnt make NatCorder ffmpeg to work - bypass rec to test UI
#else
        screencastManager.StartRecording();
#endif
    }
    void StopRecording()
    {
        Show(true);

#if UNITY_EDITOR
        // Couldnt make NatCorder ffmpeg to work - bypass rec to test
        //OnVideoReady("Assets/StreamingAssets/video.mp4");
        state.SetState(State.RECORDED);
#else
        screencastManager.StopRecording();
#endif
    }

    void OnVideoReady(string path)
    {
        Debug.Log("Screencast video ready at: " + path);
        savedVideoPath = path;
        screencastManager.GetThumbnail(savedVideoPath, OnThumbnail);
    }
    void OnThumbnail(Texture2D thumbnail)
    {
        if (thumbnail != null)
        {
            Debug.Log("thumbnail for video: "+savedVideoPath+" size:"+thumbnail.width+"x"+thumbnail.height);
            this.thumbnail.sprite = Sprite.Create(thumbnail, 
                new Rect(0, 0, thumbnail.width, thumbnail.height), 
                new Vector2(thumbnail.width / 2f, thumbnail.height / 2f
            ));
        }
        else
        {
            Debug.LogError("couldnt get Thumbnail from video: "+savedVideoPath);
        }

        state.SetState(State.RECORDED);
        PreviewVideo();
    }
    public void PreviewVideo()
    {
        if (string.IsNullOrEmpty(savedVideoPath))
            return;

        state.SetState(State.PREVIEWING);
        screencastManager.Preview(savedVideoPath);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus) 
        {
            if (state.CurrentState == State.PREVIEWING)
            {
                // back from preview - or back in focus per other reason? user can switch focus anytime...
                // CAUTION - iOS is not triggering this after back from video preview
                Debug.Log("Back from preview");
                state.SetState(State.RECORDED);
            }
            else if (state.CurrentState == State.SHARING)
            {
                // back from sharing
                Debug.Log("Back from sharing");
                state.SetState(State.RECORDED);
            }
        }
    }


    //void OnShotTaken(Texture2D shot)
    //{
    //    Debug.Log("ScreenShot: "+shot+" w:"+shot.width+" h:"+shot.height+" fmt:"+shot.format.ToString());

    //    state.SetState(State.SHOWING_SCREENSHOT);
    //    if (screenshotRaw!=null)
    //        screenshotRaw.texture = shot;
    //    else if (screenshot!=null)
    //        screenshot.sprite = Sprite.Create(shot, new Rect(0, 0, shot.width, shot.height), new Vector2(shot.width / 2f, shot.height / 2f));
    //    //screenshot.gameObject.SetActive(false);
    //    //screenshot.gameObject.SetActive(true);
    //}

    void Save()
    {
        if (string.IsNullOrEmpty(savedVideoPath))
            return;

        state.SetState(State.SAVING);
        bool saved = screencastManager.SaveToCameraRoll(savedVideoPath);
        Debug.Log("Saved Screenshot? "+saved);
        if (saved)
            state.SetState(State.SAVED);
        else
            state.SetState(State.RECORDED);
    }
    void Share()
    {
        if (string.IsNullOrEmpty(savedVideoPath))
            return;

        state.SetState(State.SHARING);
        screencastManager.Share(savedVideoPath);
    }
}
