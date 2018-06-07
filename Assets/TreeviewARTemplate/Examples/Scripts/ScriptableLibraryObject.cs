using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "scriptableLibraryObject", menuName = "Treeview/ScriptableLibraryObject")]
public class ScriptableLibraryObject : ScriptableObject
{
    public GameObject prefab;
    public string displayName;
    public Sprite thumbnail;
}
