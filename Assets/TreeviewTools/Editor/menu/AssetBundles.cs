using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundles
{
    [MenuItem("TreeviewTools/AssetBundles/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "AssetBundles/" + EditorUserBuildSettings.activeBuildTarget.ToString()+"/";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }

    [MenuItem("TreeviewTools/AssetBundles/Clean Cache of AssetBundles")]
    public static void CleanCache()
    {
        if (Caching.ClearCache())
        {
            Debug.Log("Successfully cleaned the cache.");
        }
        else
        {
            Debug.LogError("Cache is being used.");
        }
    }
}