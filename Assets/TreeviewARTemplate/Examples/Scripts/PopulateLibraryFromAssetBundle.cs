using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[RequireComponent(typeof(ObjectLibrary))]
public class PopulateLibraryFromAssetBundle : MonoBehaviour {

    [Header("Direct Reference to avoid loading bundle")]
    [SerializeField]
    protected ScriptableLibrary directLibraryAssetReference;

    [Space]
    [SerializeField]
    public AssetBundleAssetLoader assetLoader = new AssetBundleAssetLoader();
    
    protected ObjectLibrary objectLibrary;

    // Use this for initialization
    protected virtual IEnumerator Start() {

        if (directLibraryAssetReference != null)
        {
            yield return new WaitForEndOfFrame(); // guarantee all Start methods are called
            yield return new WaitForEndOfFrame(); // guarantee all Start methods are called
            Debug.Log("bypassing bundle loading with a direct reference to a ScriptableLibrary SHIPPED in build.");
            FetchLibrary(directLibraryAssetReference);
        }
        else
        {

            assetLoader.state.onStateChanged.AddListener(OnStateChanged);

            yield return new WaitForEndOfFrame(); // guarantee all Start methods are called
            yield return new WaitForEndOfFrame(); // guarantee all Start methods are called

            assetLoader.LoadBundleAndAsset();
        }
    }


    protected virtual void OnEnable()
    {
        objectLibrary = GetComponent<ObjectLibrary>();
    }


    protected virtual void OnStateChanged(AssetBundleAssetLoader.State state)
    {
        if (state == AssetBundleAssetLoader.State.LOADED)
        {
            FetchLibrary(assetLoader.Asset as ScriptableLibrary);
        }
    }
    protected virtual void FetchLibrary(ScriptableLibrary scriptableLibrary)
    {
        if (scriptableLibrary != null)
        {
            List<LibraryObject> libObjects = new List<LibraryObject>();
            //foreach (GameObject prefab in scriptableLibrary.prefabs)
            Debug.Log("FetchLibrary " + scriptableLibrary.libObjects.Count);
            foreach (ScriptableLibraryObject scriptLibObj in scriptableLibrary.libObjects)
            {
                Debug.Log("scriptableLibraryObject: "+scriptLibObj.displayName);
                LibraryObject libObj = new LibraryObject();
                libObj.prefab = scriptLibObj.prefab;
                libObj.displayName = scriptLibObj.displayName;
                libObj.thumbnail = scriptLibObj.thumbnail;
                libObjects.Add(libObj);
            }
            objectLibrary.SetLibraryObjects(libObjects);
        }
        else
        {
            assetLoader.ReportError("AssetBundle object: "+ "scriptableLibrary.asset"+" not present or incorrect.");            
        }
    }
    
}
