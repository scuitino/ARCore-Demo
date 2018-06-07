using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityARInterface
{
    public class TreeviewARPlaneVisualizer : ARPlaneVisualizer
    {

        [SerializeField] bool destroyPlanesOnDisable = true;

        protected override void OnDisable()
        {
            base.OnDisable();
            if (destroyPlanesOnDisable)
                DestroyPlanes();
        }


        public void DestroyPlanes()
        {
            foreach (KeyValuePair<string, GameObject> kvp in m_Planes)
            {
                Destroy(kvp.Value);
            }

            m_Planes = new Dictionary<string, GameObject>();
        }
    }
}
