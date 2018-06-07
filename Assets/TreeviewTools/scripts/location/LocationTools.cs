using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTools : MonoBehaviour {

    [Serializable]
    public class LocationData
    {
        public float latitude;
        public float longitude;
    }

    public static bool EqualLocations(LocationInfo loc1, LocationInfo loc2)
    {
        if (loc1.latitude != loc2.latitude)
            return false;
        if (loc1.longitude != loc2.longitude)
            return false;

        return true;
    }

    public static Vector3 GetMetersDistanceHeadingNorth(LocationData from, LocationData to)
    {
        LocationData toLat = new LocationData();
        toLat.latitude = to.latitude;
        toLat.longitude = from.longitude;
        float latDist = GetMetersDistance(from, toLat);

        LocationData toLon = new LocationData();
        toLon.latitude = from.latitude;
        toLon.longitude = to.longitude;
        float lonDist = GetMetersDistance(from, toLon);

        // Does this axis separation works fine???

        float x = (to.longitude > from.longitude)? lonDist : -lonDist;  // Possitive to the East
        float z = (to.latitude > from.latitude) ? latDist : -latDist;    // Possitive to North
        return new Vector3(x, 0f, z); // Forward is North, Right is East
    }

    public static float GetMetersDistance(LocationData from, LocationData to)
    {
        // APROXIMATION using Haversine formula for points distance in a sphere
        //float R = 6371000; // meters - Earth's radius
        //float omega1 = ((from.latitude / 180) * Mathf.PI);
        //float omega2 = ((to.latitude / 180) * Mathf.PI);
        //float variacionomega1 = (((to.latitude - from.latitude) / 180) * Mathf.PI);
        //float variacionomega2 = (((to.longitude - from.longitude) / 180) * Mathf.PI);
        //float a = Mathf.Sin(variacionomega1 / 2) * Mathf.Sin(variacionomega1 / 2) +
        //            Mathf.Cos(omega1) * Mathf.Cos(omega2) *
        //            Mathf.Sin(variacionomega2 / 2) * Mathf.Sin(variacionomega2 / 2);
        ////float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        //float c = 2 * Mathf.Asin(Mathf.Sqrt(a));
        //float d = R * c;

        // APROXIMATION using Haversine formula for points distance in a sphere
        var d1 = from.latitude * (Mathf.PI / 180.0f);
        var num1 = from.longitude * (Mathf.PI / 180.0f);
        var d2 = to.latitude * (Mathf.PI / 180.0f);
        var num2 = to.longitude * (Mathf.PI / 180.0f) - num1;
        var d3 = Mathf.Pow(Mathf.Sin((d2 - d1) / 2.0f), 2.0f) +
                 Mathf.Cos(d1) * Mathf.Cos(d2) * Mathf.Pow(Mathf.Sin(num2 / 2.0f), 2.0f);
        float d = 6376500.0f * (2.0f * Mathf.Atan2(Mathf.Sqrt(d3), Mathf.Sqrt(1.0f - d3)));

        return d;
    }
}
