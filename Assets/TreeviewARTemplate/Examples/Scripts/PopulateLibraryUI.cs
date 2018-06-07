using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateLibraryUI : UIManager {

    [SerializeField] GenericPanel mainPanel;
    [SerializeField] GenericPanel assetBundleAvailablePanel;
    [SerializeField] Button downloadYesButton;
    [SerializeField] Button downloadNoButton;

    [SerializeField] GenericPanel assetBundleDownloadingPanel;
    [SerializeField] Text errorText;

    [SerializeField] PopulateLibraryFromAssetBundle populateScript;
    

    protected override void Awake()
    {
        base.Awake();

        populateScript.assetLoader.state.onStateChanged.AddListener(OnPopulateScriptStateChanged);

        downloadNoButton.onClick.AddListener(OnDownloadNo);
        downloadYesButton.onClick.AddListener(OnDownloadYes);
    }

    void OnPopulateScriptStateChanged(AssetBundleAssetLoader.State state)
    {
        if (state == AssetBundleAssetLoader.State.HAS_NEW_VERSION)
        {
            SetCurrentPanel(assetBundleAvailablePanel);
        }
        else if (state == AssetBundleAssetLoader.State.LOADING_BUNDLE)
        {
            SetCurrentPanel(assetBundleDownloadingPanel);
        }
        else if (state == AssetBundleAssetLoader.State.ERROR)
        {
            errorText.text = populateScript.assetLoader.Error;
        }
        else if (state == AssetBundleAssetLoader.State.LOADED)
        {
            SetCurrentPanel(mainPanel);
        }
    }

    void OnDownloadNo()
    {
        SetCurrentPanel(mainPanel);
        populateScript.assetLoader.TryToLoadShippedVersion();
    }
    void OnDownloadYes()
    {
        populateScript.assetLoader.LoadFromWeb();
    }
}
