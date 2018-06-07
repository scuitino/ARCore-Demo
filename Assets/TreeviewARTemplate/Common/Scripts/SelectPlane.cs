using System.Collections;
using System.Collections.Generic;
using UnityARInterface;
using UnityEngine;
using UnityEngine.Events;

public class SelectPlane : ARBase {

    [System.Serializable] public class OnPlaneSelected : UnityEvent<Transform> { }
    [SerializeField] public OnPlaneSelected onPlaneSelected = new OnPlaneSelected();

    [SerializeField] bool disableThisAfterSelect = true;
    [SerializeField] bool disablePlanesAfterSelect = true;
    [SerializeField] bool hidePointCloudAfterSelect = true;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var camera = GetCamera();
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << LayerMask.NameToLayer("ARGameObject"); // Planes are in layer ARGameObject
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, float.MaxValue, layerMask))
            {
                Transform planeTransform = rayHit.collider.transform.parent;
                onPlaneSelected.Invoke(planeTransform);
                if (disableThisAfterSelect)
                {
                    this.enabled = false;
                }
                if (disablePlanesAfterSelect)
                {
                    // Stop adding/removing/updating planes position
                    ARPlaneVisualizer arPlaneVis = GetComponent<ARPlaneVisualizer>();
                    if (arPlaneVis != null)
                    {
                        arPlaneVis.enabled = false;
                    }
                }
                if (hidePointCloudAfterSelect)
                { 
                    // Hide point cloud
                    ARPointCloudVisualizer arPointCloudVis = GetComponent<ARPointCloudVisualizer>();
                    if (arPointCloudVis != null)
                    {
                        arPointCloudVis.enabled = false;
                    }
                }
            }
        }
    }
}
