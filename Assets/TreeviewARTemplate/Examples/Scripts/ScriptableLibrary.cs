using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ScriptableObject to put in AssetBundle
[CreateAssetMenu(fileName ="scriptableLibrary", menuName ="Treeview/ScriptableLibrary")]
public class ScriptableLibrary : ScriptableObject
{
    public List<ScriptableLibraryObject> libObjects = new List<ScriptableLibraryObject>();

}
