using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class TreeviewStore : MonoBehaviour {

    [System.Serializable] public class StoreReadyEvent : UnityEvent<ScriptableStoreItems> { }
    [SerializeField] public StoreReadyEvent onStoreReady = new StoreReadyEvent();

    [System.Serializable] public class StoreItemReadyEvent : UnityEvent<ScriptableStoreItem, Object> { }
    [SerializeField] public StoreItemReadyEvent onStoreItemReady = new StoreItemReadyEvent();


    [SerializeField] AssetBundleAssetLoader storeBundleLoader = new AssetBundleAssetLoader();
    [SerializeField] bool loadStoreBundleAtStart = true;

    public enum State
    {
        IDLE,
        LOADING_STORE,
        LOADING_DOWNLOADED_ITEMS,
        READY
    }
    public StateMachine<State> state = new StateMachine<State>();

    [Tooltip("SINGLE Android or iOS manifest bundle")]
    [SerializeField] string manifestBundleBaseURL;
    AssetBundle manifestBundle;
    AssetBundleManifest manifest;

    public ScriptableStoreItems storeItems;

    #region RUNTIME:: platform specific ID dictionaries
    Dictionary<string, ScriptableStoreItem> itemsRef;
    Dictionary<string, Hash128> itemsLatestHash;
    Dictionary<string, bool> itemsBought;
    Dictionary<string, bool> itemsDownloaded;

    int itemsToLoad;
    Dictionary<string, Object> itemsAsset;

    public bool IsItemBought(string itemPlatformId)
    {
        if (itemsBought == null || !itemsBought.ContainsKey(itemPlatformId))
            return false;
        return itemsBought[itemPlatformId];
    }
    public bool IsItemDownloaded(string itemPlatformId)
    {
        if (itemsDownloaded == null || !itemsDownloaded.ContainsKey(itemPlatformId))
            return false;
        return itemsDownloaded[itemPlatformId];
    }
    public Object GetItemAsset(string itemPlatformId)
    {
        if (itemsAsset == null || !itemsAsset.ContainsKey(itemPlatformId))
            return null;
        return itemsAsset[itemPlatformId];
    }
    #endregion


    private void Awake()
    {

        storeBundleLoader.onAssetLoaded.AddListener(OnStoreLoaded);
    }

    // Use this for initialization
    void Start() {
        
        if (loadStoreBundleAtStart)
            LoadStore();
    }
    public void LoadStore()
    {
        state.SetState(State.LOADING_STORE);

        StartCoroutine(LoadManifestBundle());

        //storeBundleLoader.LoadBundleAndAsset();
	}

    IEnumerator LoadManifestBundle()
    {
        // SINGLE "Android" or "iOS" manifest
        string url = manifestBundleBaseURL;
#if UNITY_ANDROID
        url += "Android/Android";
#elif UNITY_IOS
        url += "iOS/iOS";
#endif
        //AssetBundle manifestBundle = AssetBundle.LoadFromFile(manifestBundlePath);
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Failed to load AssetBundle: " + url + "\nerror: " + request.error + "\nresponse: " + request.responseCode);
            yield break;
        }

        manifestBundle = DownloadHandlerAssetBundle.GetContent(request);
        manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        //Continue
        storeBundleLoader.LoadBundleAndAsset();
    }

    void OnStoreLoaded(Object asset)
    {
        //Debug.Log("OnStoreLoaded");
        storeItems = asset as ScriptableStoreItems;
        itemsRef = new Dictionary<string, ScriptableStoreItem>();
        foreach (ScriptableStoreItem item in storeItems.items)
        {
            itemsRef[item.GetThisPlatformId()] = item;
        }

        //RetrieveItemHashesInStoreBundle();
        //OnHashesReady();

        //StartCoroutine(RetrieveItemHashesFromManifestServerFiles());

        StartCoroutine(RetrieveItemHashesFromManifestServerBundles());
    }

    // DIDNT WORKED OUT
    //    void RetrieveItemHashesInStoreBundle()
    //    {
    //        // Get item HASHES for this platform latest versions in server, from within Store bundle (checking item bundles manifest files)
    //        string platformManifest = "";
    //#if UNITY_ANDROID
    //        platformManifest = "Android";
    //#elif UNITY_IOS
    //        platformManifest = "iOS";
    //#endif
    //        AssetBundleManifest manifest = storeBundleLoader.AssetBundle.LoadAsset<AssetBundleManifest>(platformManifest);
    //        if (manifest == null)
    //        {
    //            Debug.LogError("platform manifest NULL ");
    //            return;
    //        }

    //        itemsLatestHash = new Dictionary<string, Hash128>();
    //        foreach (ScriptableStoreItem item in storeItems.items)
    //        {
    //            //string manifestAsset = item.bundle.GetBundleName();
    //            //AssetBundleManifest manifest = storeBundleLoader.AssetBundle.LoadAsset<AssetBundleManifest>(manifestAsset);
    //            Hash128 hash = manifest.GetAssetBundleHash(item.bundle.GetBundleName());
    //            itemsLatestHash[item.GetThisPlatformId()] = hash;
    //        }
    //    }    

    // DIDNT WORKED OUT - hash from .manifest file is not comparable later with Caching.GetCachedVersions
    //IEnumerator RetrieveItemHashesFromManifestServerFiles()
    //{
    //    Debug.Log("item manifests loading");
    //    itemsLatestHash = new Dictionary<string, Hash128>();
    //    foreach (ScriptableStoreItem item in storeItems.items)
    //    {
    //        // WWW to download item_bundle.manifest file and get Hash from that text file
    //        //Debug.Log("item manifest loading:: " + item.bundle.GetBundleManifestURL());
    //        WWW w = new WWW(item.bundle.GetBundleManifestURL());
    //        yield return w;

    //        if (!string.IsNullOrEmpty(w.error))
    //        {
    //            Debug.LogError("item manifest " + item.bundle.GetBundleManifestURL() + "\nerror: " + w.error);
    //            continue;
    //        }

    //        string manifestContents = w.text;
    //        //Debug.Log("item manifest " + item.bundle.GetBundleManifestURL() + ":\n" + manifestContents);
    //        string[] lines = manifestContents.Split('\n');
    //        string hashLine = "Hash: ";
    //        foreach (string line in lines)
    //        {
    //            int hashIdx = line.IndexOf(hashLine);
    //            //if (line.Contains("Hash: "))
    //            if (hashIdx != -1)
    //            {
    //                // Get First "Hash: "
    //                string hashString = line.Substring(hashIdx + hashLine.Length);
    //                //Debug.Log("hashString: " + hashString);
    //                Hash128 hash = Hash128.Parse(hashString);
    //                itemsLatestHash[item.GetThisPlatformId()] = hash;
    //                break; //foreach line
    //            }
    //        }
    //    }
    //    OnHashesReady();
    //}



    IEnumerator RetrieveItemHashesFromManifestServerBundles()
    {
        Debug.Log("item manifests loading");
        itemsLatestHash = new Dictionary<string, Hash128>();

        if (storeItems.items.Length == 0)
        {
            OnHashesReady();
            yield break;
        }

        // SINGLE "Android" or "iOS" manifest
        foreach (ScriptableStoreItem item in storeItems.items)
        {
            Hash128 hash = manifest.GetAssetBundleHash(item.bundle.assetBundleName);
            Debug.Log("item "+item.descriptiveName+" hash: " + hash.ToString());

            itemsLatestHash[item.GetThisPlatformId()] = hash;
        }

        OnHashesReady();
    }


    void OnHashesReady()
    {
        CheckItemsState();

        LoadDownloadedItems();
        //SetStoreReady();
    }

    

    void CheckItemsState()
    {
        itemsBought = new Dictionary<string, bool>();
        foreach(ScriptableStoreItem item in storeItems.items)
        {
            //TODO:: check also if any item is bought in IAP (might not been downloaded in this phone - or even deleted?)
            itemsBought[item.GetThisPlatformId()] = true; // DEBUG
        }

        itemsDownloaded = new Dictionary<string, bool>();
        foreach (ScriptableStoreItem item in storeItems.items)
        {
            // check if latest version is in cache, if so trigger an event (as someone will want to use it - this might be at startup)
            List<Hash128> cachedVersions = new List<Hash128>();
            Caching.GetCachedVersions(item.bundle.GetBundleName(), cachedVersions);
            //Caching.GetCachedVersions("pepe", cachedVersions);
            string str = "";
            foreach (Hash128 h in cachedVersions)
            {
                //str += h.ToString() + ", ";
                str += h.ToString()+" :: "+h.GetHashCode() + " :: "+h.isValid+"\n";
            }
            Debug.Log("item "+ item.bundle.GetBundleName() + "\ncachedVersions:"+str+"\nlatestHash:"+ itemsLatestHash[item.GetThisPlatformId()].ToString());

            bool cached = false;
            foreach (Hash128 cachedHash in cachedVersions)
            {
                //if (cachedVersions.Contains(itemsLatestHash[item.GetThisPlatformId()]))
                if (cachedHash == itemsLatestHash[item.GetThisPlatformId()])
                {
                    Debug.Log("latest is in cache");
                    itemsDownloaded[item.GetThisPlatformId()] = true;
                    cached = true;
                }
                //else
            }
            if (!cached)
            {
                Debug.Log("version available to download");
                itemsDownloaded[item.GetThisPlatformId()] = false;
            }
        }

        itemsAsset = new Dictionary<string, Object>();
    }

    void LoadDownloadedItems()
    {
        // Loading in parallel now - TODO:: load them in sequence
        state.SetState(State.LOADING_DOWNLOADED_ITEMS);
        itemsToLoad = 0;
        foreach (KeyValuePair<string, bool> kvp in itemsDownloaded)
        {
            if (kvp.Value == true)
                itemsToLoad++;
        }

        if (itemsToLoad > 0)
        {
            Debug.Log("Started to (down)load items #" + itemsToLoad);
            foreach (KeyValuePair<string, bool> kvp in itemsDownloaded)
            {
                if (kvp.Value == true)
                {
                    // Load it from cache - triggers events
                    LoadItem(itemsRef[kvp.Key]);
                }
            }
            // when all items are loaded, onStoreReady() is invoked
        }
        else
        {
            SetStoreReady();
        }
    }



    public void BuyItem(ScriptableStoreItem item)
    {
        //TODO:: use IAP
    }
    
    public void LoadItem(ScriptableStoreItem item)
    {

        AssetBundleAssetLoader itemBundleLoader = new AssetBundleAssetLoader(item.bundle);
        itemBundleLoader.behaviorToRunCoroutines = this;
        itemBundleLoader.hasShippedVersion = false;
        itemBundleLoader.autoDownloadNewVersion = true;
        Hash128 hash = manifest.GetAssetBundleHash(item.bundle.assetBundleName);
        itemBundleLoader.hardcodedServerBundleHash = hash;
        itemBundleLoader.useHardcodedHash = true;
        itemBundleLoader.onAssetLoaded.AddListener( (Object asset) => {
            Debug.Log("Item ready: "+item.descriptiveName+", asset: "+asset.name);
            itemsAsset[item.GetThisPlatformId()] = asset;
            itemsDownloaded[item.GetThisPlatformId()] = true; // Mark it as downloaded right now for consistency
            onStoreItemReady.Invoke(item, asset);

            if (state.CurrentState == State.LOADING_DOWNLOADED_ITEMS)
            {
                itemsToLoad--; // CHECKING all items downloaded
                if (itemsToLoad==0)
                {
                    SetStoreReady();
                }
            }
        });
        itemBundleLoader.LoadBundleAndAsset(); // can load from cache if available, or download it
    }

    void SetStoreReady()
    {
        Debug.Log("ON STORE READY");
        state.SetState(State.READY);
        onStoreReady.Invoke(storeItems);
    }
}
