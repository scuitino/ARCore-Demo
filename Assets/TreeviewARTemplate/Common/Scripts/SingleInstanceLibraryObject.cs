using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SingleInstanceLibraryObject : LibraryObject {

    public GameObject singleInstance;

    public override GameObject Instantiate(Vector3 worldPos)
    {
        //return base.Instantiate(worldPos);

        singleInstance.SetActive(true);
        singleInstance.transform.position = worldPos;
        singleInstance.transform.rotation = Quaternion.identity;
        singleInstance.transform.SetParent(null);
        return singleInstance;
    }
}
