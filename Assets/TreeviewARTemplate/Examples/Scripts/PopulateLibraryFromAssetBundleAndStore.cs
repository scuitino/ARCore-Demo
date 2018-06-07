using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateLibraryFromAssetBundleAndStore : PopulateLibraryFromAssetBundle {

    [SerializeField] TreeviewStore store;

    List<LibraryObject> bundleObjects;
    List<LibraryObject> storeObjects;

    protected void Awake()
    {
        store.state.onStateChanged.AddListener(OnStoreState);
        store.onStoreItemReady.AddListener(OnStoreItemReady);
    }

    // FROM single AssetBundle
    protected override void FetchLibrary(ScriptableLibrary scriptableLibrary)
    {
        if (scriptableLibrary != null)
        {
            bundleObjects = new List<LibraryObject>();
            //foreach (GameObject prefab in scriptableLibrary.prefabs)
            Debug.Log("FetchLibrary " + scriptableLibrary.libObjects.Count);
            foreach (ScriptableLibraryObject scriptLibObj in scriptableLibrary.libObjects)
            {
                Debug.Log("scriptableLibraryObject: " + scriptLibObj.displayName);
                LibraryObject libObj = new LibraryObject();
                libObj.prefab = scriptLibObj.prefab;
                libObj.displayName = scriptLibObj.displayName;
                libObj.thumbnail = scriptLibObj.thumbnail;
                bundleObjects.Add(libObj);
            }

            ResetLibraryObjects();
        }
        else
        {
            assetLoader.ReportError("AssetBundle object: " + "scriptableLibrary.asset" + " not present or incorrect.");
        }
    }

    // FROM Store bundles
    void OnStoreState(TreeviewStore.State newState)
    {
        if (newState == TreeviewStore.State.LOADING_DOWNLOADED_ITEMS)
        {
            // Init & Clear storeItems when Store is to be loaded again - and OnStoreItemReady are going to be called
            storeObjects = new List<LibraryObject>();
        }
    }
    void OnStoreItemReady(ScriptableStoreItem item, Object asset)
    {
        // event is called at startup for downloaded items, then when bought&downloaded
        //add asset items to Library
        ScriptableLibrary lib = asset as ScriptableLibrary; // item asset is a library itself, and can contain several objects

        foreach (ScriptableLibraryObject scriptLibObj in lib.libObjects)
        {
            Debug.Log("STORE scriptableLibraryObject: " + scriptLibObj.displayName);
            LibraryObject libObj = new LibraryObject();
            libObj.prefab = scriptLibObj.prefab;
            libObj.displayName = scriptLibObj.displayName;
            libObj.thumbnail = scriptLibObj.thumbnail;
            storeObjects.Add(libObj); // each item is appended - 
        }
        ResetLibraryObjects();
    }

    void ResetLibraryObjects()
    {
        List<LibraryObject> allObjects = new List<LibraryObject>();
        if (bundleObjects != null)
            allObjects.AddRange(bundleObjects);
        if (storeObjects != null)
            allObjects.AddRange(storeObjects);

        objectLibrary.SetLibraryObjects(allObjects);
    }
}
