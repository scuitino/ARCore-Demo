using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "scriptableStoreItem", menuName = "Treeview/ScriptableStoreItem")]
public class ScriptableStoreItem : ScriptableObject
{
    public string androidId;
    public string iOSId;

    public string descriptiveName;
    public string description;

    public Sprite thumbnail;

    public ScriptableBundleDescription bundle;


    public string GetThisPlatformId()
    {
#if UNITY_ANDROID
        return androidId;
#elif UNITY_IOS
        return iOSId;
#else
        return "";
#endif
    }
}
