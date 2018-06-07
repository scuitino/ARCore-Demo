using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "scriptableBundleDescription", menuName = "Treeview/ScriptablebundleDescription")]
public class ScriptableBundleDescription : ScriptableObject
{
    public string assetBundleBaseURL = "www.example.com/";
    public string assetBundleName = "myassetbundle";
    public string versionFileSuffix = "_version.txt";

    [Space]
    public string mainAssetName = "scriptableLibrary.asset";


    public string GetBundleName()
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
    public string GetBundleURL()
    {
        return assetBundleBaseURL + GetPlatform() + "/" + GetBundleName();
    }
    public string GetBundleVersionURL()
    {
        return GetBundleURL() + versionFileSuffix;
    }
    public string GetBundleManifestURL()
    {
        // CONVENTION:: we upload bundle manifest at the side of bundle itself in the server
        //return GetBundleURL() + ".manifest";

        // CONVENTION:: we upload manifest bundle at the side of bundle itself in the server - this one can be loaded as a bundle itself
        return assetBundleBaseURL + GetPlatform() + "/" + GetPlatform(); // url/Android/Android file or url/iOS/iOS file
    }

    public AssetBundleAssetLoader MakeLoader()
    {
        AssetBundleAssetLoader loader = new AssetBundleAssetLoader(this);
        return loader;
    }
}
