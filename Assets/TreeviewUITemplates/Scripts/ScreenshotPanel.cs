using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotPanel : GenericPanel {

    [SerializeField] RawImage screenshotRaw;
    [SerializeField] Image screenshot;
    [SerializeField] Button saveBt;
    [SerializeField] Button shareBt;

    [SerializeField] ScreenshotManager shotManager;

    [SerializeField] GameObject savedOkPanel;

    string savedPhotoPath;

    public enum State
    {
        IDLE,
        TAKING_SCREENSHOT,
        SHOWING_SCREENSHOT,
        SAVING,
        SAVED,
        SHARING
    }
    StateMachine<State> state = new StateMachine<State>();

    protected virtual void Awake()
    {
        if (saveBt != null)
            saveBt.onClick.AddListener(Save);
        if (shareBt != null)
            shareBt.onClick.AddListener(Share);
        
        shotManager.OnPhotoTaken.AddListener(OnShotTaken);

        state.onStateChanged2.AddListener(OnStateChanged);
        state.SetState(State.IDLE, true);
    }

    void OnStateChanged(State oldState, State newState)
    {
        screenshot.gameObject.SetActive(state.CurrentState > State.TAKING_SCREENSHOT);

        if (saveBt != null)
            saveBt.gameObject.SetActive(state.CurrentState == State.SHOWING_SCREENSHOT);
        if (shareBt != null)
            shareBt.gameObject.SetActive(state.CurrentState == State.SHOWING_SCREENSHOT || state.CurrentState == State.SAVED);

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
            savedPhotoPath = null;
            TakeScreenshot();      
        }
    }

    void TakeScreenshot()
    {
        state.SetState(State.TAKING_SCREENSHOT);
        shotManager.TakeScreenshot();
    }
    void OnShotTaken(Texture2D shot)
    {
        Debug.Log("ScreenShot: "+shot+" w:"+shot.width+" h:"+shot.height+" fmt:"+shot.format.ToString());

        state.SetState(State.SHOWING_SCREENSHOT);
        if (screenshotRaw!=null)
            screenshotRaw.texture = shot;
        else if (screenshot!=null)
            screenshot.sprite = Sprite.Create(shot, new Rect(0, 0, shot.width, shot.height), new Vector2(shot.width / 2f, shot.height / 2f));
        //screenshot.gameObject.SetActive(false);
        //screenshot.gameObject.SetActive(true);
    }

    void Save()
    {
        state.SetState(State.SAVING);
        if (screenshotRaw!=null)
            savedPhotoPath = shotManager.SavePhoto(screenshotRaw.texture as Texture2D);
        else if (screenshot != null)
            savedPhotoPath = shotManager.SavePhoto(screenshot.mainTexture as Texture2D);
        state.SetState(State.SAVED);
    }
    void Share()
    {
        state.SetState(State.SHARING);
        if (savedPhotoPath != null)
        {
            // Already saved
            shotManager.Share(savedPhotoPath);
        }
        else
        {
            if (screenshotRaw != null)
                savedPhotoPath = shotManager.SavePhoto(screenshotRaw.texture as Texture2D, false);
            else if (screenshot != null)
                savedPhotoPath = shotManager.SavePhoto(screenshot.mainTexture as Texture2D, false);
            shotManager.Share(savedPhotoPath);
        }
    }
}
