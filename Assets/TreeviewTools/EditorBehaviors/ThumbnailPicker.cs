
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ThumbnailPicker : MonoBehaviour {

    //public Camera camera;
    public string captureFilename = "Assets/thumbnail.png";

    [Tooltip("This is currently printing grey area of Game view still... So better use FullScreenshot mode, modifying Game resolution as desired.")]
    public bool takePartialScreenshot = false;
    public Rect partialViewport = Rect.MinMaxRect(0f,0f,1f,1f);


	// Use this for initialization
	protected virtual void Start () {

        StartCoroutine(TakeThumbnail());
    }

    protected virtual IEnumerator TakeThumbnail()
    {
        if (takePartialScreenshot)
            yield return StartCoroutine(TakePartialScreenshot());
        else
            yield return StartCoroutine(TakeFullScreenshot());
    }
    protected virtual IEnumerator TakeFullScreenshot()
    {
        ScreenCapture.CaptureScreenshot(captureFilename);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame(); // Wait image is done
        Debug.Log("Taken Full Screenshot at: " + captureFilename);
    }

    protected virtual IEnumerator TakePartialScreenshot()
    {
        ScreenCapture.CaptureScreenshot(captureFilename);
        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame(); // Wait image is done

        // Crop
        Debug.Log("screen: "+Screen.width+"x"+Screen.height+"  res:"+Screen.safeArea.ToString());
        Vector2Int fullSize = new Vector2Int(Screen.width, Screen.height);
        Vector2Int cropStart = new Vector2Int(
            Mathf.FloorToInt(fullSize.x * partialViewport.x),
            Mathf.FloorToInt(fullSize.y * partialViewport.y)
        );
        Vector2Int cropSize = new Vector2Int(
            Mathf.FloorToInt(fullSize.x * partialViewport.width),
            Mathf.FloorToInt(fullSize.y * partialViewport.height)
        );

        Texture2D finalTex = new Texture2D(cropSize.x, cropSize.y);
        Color[] cropPixels = tex.GetPixels(cropStart.x, cropStart.y, cropSize.x, cropSize.y);
        finalTex.SetPixels(cropPixels);

        //Rect screenRect = Rect.MinMaxRect(
        //    Mathf.FloorToInt(fullSize.x * partialViewport.x),
        //    Mathf.FloorToInt(fullSize.y * partialViewport.y),
        //    Mathf.FloorToInt(fullSize.x * partialViewport.xMax),
        //    Mathf.FloorToInt(fullSize.y * partialViewport.yMax)
        //);
        //finalTex.ReadPixels(screenRect, 0, 0); // PROBLEM :: This includes grey area of Game view

        // Save
        byte[] bytes = finalTex.EncodeToPNG();
        System.IO.File.WriteAllBytes(captureFilename, bytes);

        Debug.Log("Taken Partial Screenshot at: " + captureFilename + " ::  " + fullSize.ToString() + " -> " + cropStart.ToString()+" "+cropSize.ToString());
        //Debug.Log("Taken Partial Screenshot at: " + captureFilename + " ::  "+fullSize.ToString()+" -> "+screenRect.ToString());
        yield break;
    }
}
#endif