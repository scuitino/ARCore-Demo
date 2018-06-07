
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ThumbnailPickerBatch : ThumbnailPicker {

    [Space]
    [Header("BATCH THUMBNAILS - configure below")]
    public string thumbnailFolder = "Assets/";

    [Tooltip("Will make thumbnails for all children of batchParent. Filename will be determined by GameObject name.")]
    public Transform batchParent;

	// Use this for initialization
	protected override void Start () {

        //base.Start();
        StartCoroutine(PickThumbnails());
    }

    IEnumerator PickThumbnails()
    {
        foreach (Transform t in batchParent)
        {
            t.gameObject.SetActive(false);
        }

        foreach (Transform t in batchParent)
        {
            t.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame(); // wait renderer is ready

            captureFilename = thumbnailFolder + t.name + ".png";
            yield return TakeThumbnail();
            t.gameObject.SetActive(false);
        }
    }
    
}
#endif