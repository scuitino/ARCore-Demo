using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[System.Serializable]
public class AssetBundleAssetLoader {

    [System.Serializable] public class AssetBundleAvailableEvent : UnityEvent { }
    [Tooltip("To DOWNLOAD file call LoadFromWeb(). If chosing NOT to download, call TryToLoadShippedVersion.")]
    [SerializeField] public AssetBundleAvailableEvent onAssetBundleAvailableToDownload = new AssetBundleAvailableEvent();

    [System.Serializable] public class AssetBundleStartedDownloadEvent : UnityEvent { }
    [SerializeField] public AssetBundleStartedDownloadEvent onAssetBundleStartedDownload = new AssetBundleStartedDownloadEvent();

    [System.Serializable] public class AssetBundleErrorEvent : UnityEvent<string> { }
    [SerializeField] public AssetBundleErrorEvent onAssetBundleError = new AssetBundleErrorEvent();

    [System.Serializable] public class AssetBundleFinishedDownloadEvent : UnityEvent { }
    [SerializeField] public AssetBundleFinishedDownloadEvent onAssetBundleFinishedDownload = new AssetBundleFinishedDownloadEvent();

    [System.Serializable] public class AssetBundleLoadedEvent : UnityEvent<AssetBundle> { }
    [SerializeField] public AssetBundleLoadedEvent onAssetBundleLoaded = new AssetBundleLoadedEvent();

    [System.Serializable] public class AssetBundleAssetFinishedLoadEvent : UnityEvent<Object> { }
    [SerializeField] public AssetBundleAssetFinishedLoadEvent onAssetLoaded = new AssetBundleAssetFinishedLoadEvent();


    public MonoBehaviour behaviorToRunCoroutines;

    [SerializeField] public string assetBundleBaseURL = "www.example.com/";
    [SerializeField] public string assetBundleName = "myassetbundle";
    [SerializeField] public string versionFileSuffix = "_version.txt";
    
    [SerializeField] public string assetName = "scriptableLibrary.asset";

    [Tooltip("To ship an asset bundle version, put it in StreamingAssets, and set version here")]
    [SerializeField] public bool hasShippedVersion = true;
    [Tooltip("Will download bundle only if version is greater than this")]
    [SerializeField] public int shippedVersion = 1;
    [Tooltip("Auto download bundle when new version is available. Otherwise you need to call LoadFromWeb() manually.")]
    [SerializeField] public bool autoDownloadNewVersion = false;


    [SerializeField] public bool useHardcodedHash = false;
    [SerializeField] public Hash128 hardcodedServerBundleHash;

    uint serverAssetBundleVersion = 1; // download server version txt every time requested

    public enum State
    {
        IDLE,

        LOADING_BUNDLE,
        ERROR,
        LOADING_ASSET,
        LOADED,

        HAS_NEW_VERSION
    }
    public StateMachine<State> state = new StateMachine<State>();
    string error;
    public string Error { get { return error; } }
    AssetBundle assetBundle;
    public AssetBundle AssetBundle { get { return assetBundle; } }
    Object asset;
    public Object Asset { get { return asset; } }


    string GetBundleName()
    {
        return assetBundleName;
    }
    public string GetPlatform()
    {
#if UNITY_ANDROID
        return "Android";
#elif UNITY_IOS
        return "iOS";
#else
        return "";
#endif
    }
    string GetBundleURL()
    {
        return assetBundleBaseURL + GetPlatform()+"/"+ GetBundleName();
    }
    string GetBundleVersionURL()
    {
        return GetBundleURL() + versionFileSuffix;
    }

    // Use this for initialization
    //private void Start()
    //{
        //yield return new WaitForEndOfFrame(); // guarantee all Start methods are called
        //yield return new WaitForEndOfFrame(); // guarantee all Start methods are called
        //LoadBundleAndAsset();
    //}

    public AssetBundleAssetLoader() : base()
    {

        state.onStateChanged.AddListener(OnStateChanged);
    }

    public AssetBundleAssetLoader(ScriptableBundleDescription bundleDescription) : this()
    {
        assetBundleBaseURL = bundleDescription.assetBundleBaseURL;
        assetBundleName = bundleDescription.assetBundleName;
        versionFileSuffix = bundleDescription.versionFileSuffix;
        assetName = bundleDescription.mainAssetName;
    }


    public void LoadBundleAndAsset() {

        Debug.Log("STARTED LOADING "+assetBundleName+"::"+assetName+"\n"+GetBundleURL());

        if (behaviorToRunCoroutines == null)
        {
            Debug.LogError("Need a behavior assigned to run coroutines.");
            return;
        }                        
        if (string.IsNullOrEmpty(assetBundleName))
        {
            Debug.LogError("empty assetBundleName");
            return;
        }

        behaviorToRunCoroutines.StartCoroutine(_CheckVersion());
    }

    IEnumerator _CheckVersion()
    {
        if (string.IsNullOrEmpty(assetBundleBaseURL))
        {
            Debug.LogError("empty assetBundleBaseURL");
            VersionCheckFailed();
            yield break;
        }

        if (useHardcodedHash)
        {
            LoadFromWeb();
            yield break;
        }

        if (string.IsNullOrEmpty(versionFileSuffix))
        {
            Debug.LogError("empty versionFileSuffix");
            VersionCheckFailed();
            yield break;
        }

        // get latest assetbundle version file from server
        WWW versionWWW = new WWW(GetBundleVersionURL());
        yield return versionWWW;

        if (!string.IsNullOrEmpty(versionWWW.error))
        {
            ReportError("assetBundleVersionURL "+ GetBundleVersionURL() + "error: "+versionWWW.error);
            VersionCheckFailed();
            yield break;
        }
        else
        {
            Debug.Log("assetBundle Version in backend: " + versionWWW.text);
            serverAssetBundleVersion = (uint)(int.Parse(versionWWW.text));

            if (hasShippedVersion && serverAssetBundleVersion <= shippedVersion)
            {
                // load shipped version
                VersionCheckFailed();
                yield break;
            }

            bool isCached = Caching.IsVersionCached(GetBundleURL(), (int)serverAssetBundleVersion);
            if (isCached)
            {
                // load it from cache
                LoadFromWeb();
            }
            else
            {
                // has new version available to download
                state.SetState(State.HAS_NEW_VERSION);                
            }
        }
    }


    void VersionCheckFailed()
    {
        TryToLoadShippedVersion();
    }


    public void TryToLoadShippedVersion()
    { 
        if (hasShippedVersion)
            LoadFromFile();
    }
    void LoadFromFile()
    {
        string filepath = Path.Combine(Application.streamingAssetsPath, GetPlatform());
        filepath = Path.Combine(filepath, GetBundleName());

        error = null;
        assetBundle = null;
        state.SetState(State.LOADING_BUNDLE);

        //assetBundle = AssetBundle.LoadFromFile(filepath); // blocking option
        behaviorToRunCoroutines.StartCoroutine(_LoadFromFile(filepath));            // async option
    }
    IEnumerator _LoadFromFile(string filepath)
    {
        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(filepath);
        yield return req;

        assetBundle = req.assetBundle;
        if (assetBundle != null)
        {
            Debug.Log("succesfully loaded AssetBundle from file: "+filepath);
            onAssetBundleLoaded.Invoke(assetBundle);

            FetchAsset();
        }
        else
        {
            ReportError("Failed to load AssetBundle from file: " + filepath);
        }
    }


    public void LoadFromWeb()
    {
        error = null;
        assetBundle = null;
        state.SetState(State.LOADING_BUNDLE);

        behaviorToRunCoroutines.StartCoroutine(_LoadFromWeb(GetBundleURL()));
    }
    IEnumerator _LoadFromWeb(string uri)
    {
        UnityWebRequest request = null;
        if (useHardcodedHash)
        {
            Debug.Log("Loading AssetBundle: " + uri + " (hash: " + hardcodedServerBundleHash.ToString() + ")");
            request = UnityWebRequestAssetBundle.GetAssetBundle(uri, hardcodedServerBundleHash, 0);
        }
        else
        {
            Debug.Log("Loading AssetBundle: " + uri + " (v: " + serverAssetBundleVersion + ")");
            //UnityWebRequest request = UnityWebRequest.GetAssetBundle(uri, serverAssetBundleVersion, 0);
            request = UnityWebRequestAssetBundle.GetAssetBundle(uri, serverAssetBundleVersion, 0);
        }
        
        //Debug.Log("Loading AssetBundle2");
        yield return request.SendWebRequest();
        //Debug.Log("Loading AssetBundle3");
        if (request.isNetworkError || request.isHttpError)
        {
            ReportError("Failed to load AssetBundle, error: "+request.error +" response: "+request.responseCode);
        }
        else
        {
            assetBundle = DownloadHandlerAssetBundle.GetContent(request);
            Debug.Log("Loaded AssetBundle was successful: "+assetBundle.name + ", #assets: "+assetBundle.GetAllAssetNames().Length);
            onAssetBundleLoaded.Invoke(assetBundle);

            FetchAsset();
        }
    }
    

    void FetchAsset()
    {
        if (string.IsNullOrEmpty(assetName))
        {
            state.SetState(State.LOADED);
            return;
        }

        state.SetState(State.LOADING_ASSET);

        //asset = assetBundle.LoadAsset(assetName); // blocking option
        behaviorToRunCoroutines.StartCoroutine(_FetchAsset());    // async option
    }
    IEnumerator _FetchAsset()
    {
        AssetBundleRequest req = assetBundle.LoadAssetAsync(assetName);
        yield return req;

        asset = req.asset;

        if (asset != null)
        {
            state.SetState(State.LOADED);
        }
        else
        {
            ReportError("AssetBundle object: "+ assetName+" not present or incorrect.");
        }
    }


    public void ReportError(string err)
    {
        error = err;
        state.SetState(State.ERROR);
    }

    void OnStateChanged(State state)
    {
        if (state == State.LOADING_BUNDLE)
        {
            onAssetBundleStartedDownload.Invoke();
        }
        else if (state == State.LOADING_ASSET)
        {
            onAssetBundleFinishedDownload.Invoke();
        }
        else if (state == State.ERROR)
        {
            Debug.Log(error);
            onAssetBundleError.Invoke(error);
        }
        else if (state == State.LOADED)
        {
            if (Asset != null)
                onAssetLoaded.Invoke(Asset);
        }
        else if (state == State.HAS_NEW_VERSION)
        {
            onAssetBundleAvailableToDownload.Invoke();
            if (autoDownloadNewVersion)
                LoadFromWeb();
        }
    }
}
