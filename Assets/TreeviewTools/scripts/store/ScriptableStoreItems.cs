using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "scriptableStoreItems", menuName = "Treeview/ScriptableStoreItems")]
public class ScriptableStoreItems : ScriptableObject
{
    public ScriptableStoreItem[] items;
}
