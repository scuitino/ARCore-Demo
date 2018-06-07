using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Text = UnityEngine.UI.Text;

public class ScreenshotManager : MonoBehaviour
{
    public class OnPhotoProcessedEvent : UnityEvent<Texture2D> { }
    [Tooltip("param Texture2D texture")]
    public OnPhotoProcessedEvent OnPhotoTaken = new OnPhotoProcessedEvent();

    //public class OnPhotoSavedEvent : UnityEvent<string> { }
    //[Tooltip("param string path")]
    //public OnPhotoSavedEvent OnPhotoSaved = new OnPhotoSavedEvent();

    [Tooltip("path after persistentDataPath")]
    public string savingFolder = "/photos";

    public string Subject = "Share";
    public string shareMessage = "Share message";

    [Tooltip("include CaptureAndSave Plugin for saving in native gallery")]    
    public CaptureAndSave captureAndSavePlugin;

    private bool canSave = true;
    private bool savedFrame = false;
    private bool share = false;

    void Awake()
    {
    }

    void Start()
    {
    }


    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = ((float) 1 / source.width) * ((float) source.width / targetWidth);
        float incY = ((float) 1 / source.height) * ((float) source.height / targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float) px % targetWidth),
                incY * ((float) Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }
    

    Texture2D __TakeScreenshot()
    {
        int resWidth = Screen.width;
        int resHeight = Screen.height;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);

        Camera camera = Camera.main;
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        screenShot.Apply();
        camera.targetTexture = null;
        RenderTexture.active = null;

        rt.DiscardContents();
        rt.Release();
        Destroy(rt);

        return screenShot;

        //return ScreenCapture.CaptureScreenshotAsTexture();
    }
    IEnumerator _TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();
        Texture2D screenshot = __TakeScreenshot();
        //yield return new WaitForEndOfFrame();
        //yield return new WaitForEndOfFrame(); // Wait 1 frame for texture to be ready
        //yield return new WaitForSeconds(5f);
        OnPhotoTaken.Invoke(screenshot);
    }
    public void TakeScreenshot()
    {
        StartCoroutine(_TakeScreenshot());
    }

    public string SavePhoto(Texture2D photo, bool saveOnNativeGallery = true)
    {   
        if (saveOnNativeGallery && captureAndSavePlugin!=null)
        {
            captureAndSavePlugin.SaveTextureToGallery(photo);
            Debug.Log("Saved photo on Gallery");
        }

        //NatCam.SavePhoto(photo, SaveMode.SaveToPhotoAlbum, Orientation.Rotation_0, new SaveCallback(onSave));
        if (!Directory.Exists(Application.persistentDataPath + savingFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savingFolder);
        }
        string path = Application.persistentDataPath + savingFolder + "screenshot_" +
                        DateTime.Now.Millisecond + ".png";
        System.IO.File.WriteAllBytes(path, photo.EncodeToPNG());
        Debug.Log("Saved photo on path: "+path);

        return path;
        //onSave(path);
    }

    public void Share(string path)
    {
        //NativeShare.Share(shareMessage, path, path, Subject, "image/png", true); // produced 2 images
        NativeShare.Share(shareMessage, path, null, Subject, "image/png", true);
    }
}

