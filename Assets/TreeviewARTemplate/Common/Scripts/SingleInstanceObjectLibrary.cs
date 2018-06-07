using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleInstanceObjectLibrary : ObjectLibrary {

    [Header("Single Instance Object - in scene")]
    [SerializeField] GameObject instantiatedObject;

    protected override void Start()
    {
        libraryObjects = new List<LibraryObject>();
        SingleInstanceLibraryObject libObj = new SingleInstanceLibraryObject();
        libObj.singleInstance = instantiatedObject;
        libraryObjects.Add(libObj);

        selectedLibraryObject = libObj;

        base.Start();
    }
}
