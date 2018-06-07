using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Packages
{
    [MenuItem("TreeviewTools/Packages/Make Common package")]
    static void MakeARTemplatePackage()
    {
        string packageFile = "TreeviewCommon.unitypackage";
        string[] pathFolders = new string[] {
            
            "Assets/UnityARKitPlugin",
            "Assets/GoogleARCore",
            "Assets/UnityARInterface",
                        
            "Assets/CaptureAndSavePlugin",            
            "Assets/NatCorder",

            "Assets/TreeviewTools",
            "Assets/TreeviewUITemplates",
            "Assets/TreeviewARTemplate",
            "Assets/AssetBundlesSource", // Example of bundle assets

            "Assets/StreamingAssets",   // currently include sample bundles
            "Assets/Plugins",           // include IAP stuff

            "Assets/TreeviewReadme"
        };

        AssetDatabase.ExportPackage(pathFolders, packageFile, ExportPackageOptions.Recurse);
    }
}
