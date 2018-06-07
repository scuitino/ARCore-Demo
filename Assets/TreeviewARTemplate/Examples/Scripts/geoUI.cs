using System.Collections;
using System.Collections.Generic;
using UnityARInterface;
using UnityEngine;
using UnityEngine.UI;

public class geoUI : MonoBehaviour
{
    public Toggle toggleGPS;
    public Toggle toggleCompass;
    public Toggle togglePlane;

    public Text textGPS;
    public Text textCompass;
    public Text textPlane;


    public void OnGPS(LocationTools.LocationData gpsData)
    {
        toggleGPS.isOn = true;
        textGPS.text = gpsData.latitude.ToString() + "\n" + gpsData.longitude.ToString();
    }
    public void OnCompass(float heading)
    {
        toggleCompass.isOn = true;
        textCompass.text = heading.ToString();
    }
    public void OnPlane(BoundedPlane plane)
    {
        togglePlane.isOn = true;
        textPlane.text = plane.center.ToString() + "\n" + plane.extents.ToString();
    }

    private void Update()
    {
        if (Input.location.status != LocationServiceStatus.Running)
        {
            textGPS.text = Input.location.status.ToString();
        }
        if (!Input.compass.enabled)
        {
            textCompass.text = "Disabled";
        }
    }
}
