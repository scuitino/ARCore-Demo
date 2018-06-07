using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeolocatedLibrary : ObjectLibrary {

    [Header("Single Instance Object - in scene")]
    [SerializeField] GeolocatedObject geolocatedObject;

    protected override void Start()
    {
        libraryObjects = new List<LibraryObject>();
        libraryObjects.Add(geolocatedObject);

        selectedLibraryObject = geolocatedObject;

        base.Start();
    }
}
