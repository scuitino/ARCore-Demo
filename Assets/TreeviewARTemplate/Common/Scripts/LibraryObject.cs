using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic Library Object - representing a prefab to instantiate - subclass it to add custom data
/// </summary>
[Serializable]
public class LibraryObject
{
    [SerializeField] public GameObject prefab;
    [SerializeField] public string displayName;
    [SerializeField] public Sprite thumbnail;

    public virtual string ToStringDebug()
    {
        if (prefab != null)
            return "LibObj::Prefab::" + prefab.name;
        else
            return "LibObj::Prefab::null";
    }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(displayName))
            return displayName;
        if (prefab != null)
            return prefab.name;
        return base.ToString();
    }

    public virtual GameObject Instantiate(Vector3 worldPos)
    {
        return GameObject.Instantiate(prefab, worldPos, Quaternion.identity, null);
    }
}
