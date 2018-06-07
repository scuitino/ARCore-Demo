using System.Collections;
using System.Collections.Generic;
using UnityARInterface;
using UnityEngine;

public class TapToCreate : ARBase {

    [SerializeField] protected ObjectLibrary objectLibrary;
    [SerializeField] protected WorldObjects worldObjects;

    [SerializeField] protected float heightAbovePlane = 1f;
    [SerializeField] protected int maxObjectsToInstantiate = 1;

    // to disable planes visualizations
    [SerializeField]
    GameObject _ARRoot;
    
    // Use this for initialization
    protected virtual void Start () {
		
	}

    // Update is called once per frame
    protected virtual void Update () {

        if (Input.GetMouseButtonDown(0) && objectLibrary.SelectedLibraryObject != null && worldObjects.InstantiatedObjectsCount < maxObjectsToInstantiate)
        {
            var camera = GetCamera();

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            int layerMask = 1 << LayerMask.NameToLayer("ARGameObject"); // Planes are in layer ARGameObject

            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, float.MaxValue, layerMask))
            {
                worldObjects.InstantiateLibraryObject(objectLibrary.SelectedLibraryObject, rayHit.point + Vector3.up * heightAbovePlane);
                // disable all particles and planes
                _ARRoot.GetComponent<ARPlaneVisualizer>().DestroyAllPlanes();
                _ARRoot.GetComponent<ARPlaneVisualizer>().enabled = false;
                _ARRoot.GetComponent<PlaneVisualizationManager>().DestroySpawnedPlanes();
                _ARRoot.GetComponent<PlaneVisualizationManager>().enabled = false;
                _ARRoot.GetComponent<ARPointCloudVisualizer>().enabled = false;                
                //m_ObjectToPlace.transform.position = rayHit.point;
            }
        }
    }
}
