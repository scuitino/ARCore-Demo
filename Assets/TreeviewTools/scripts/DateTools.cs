using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateTools {

    public static DateTime ConvertUnixTimeStamp(string unixTimeStamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(Convert.ToDouble(unixTimeStamp));
    }

    public static string NoUnwantedInfo(DateTime dLocal)
    {
        DateTime nowLocal = DateTime.UtcNow.ToLocalTime();
        //DateTime dLocal = d.ToLocalTime();
        if (dLocal.Date == nowLocal.Date)
        {
            TimeSpan ts = dLocal.TimeOfDay;
            return ts.Hours + ":" + ts.Minutes;//ts.ToString();//"13:16";
        }
        else if (dLocal.Year == nowLocal.Year)
        {
            return dLocal.Date.ToString("MM/dd");// "1/19";
        }
        else
        {
            return dLocal.Date.ToString("yyyy/MM/dd");// "2018/1/19";
        }
    }

    public static string AgoFromUTCNow(DateTime dUTC)
    {
        // 1.
        // Get time span elapsed since the date.
        TimeSpan s = DateTime.UtcNow.Subtract(dUTC);

        // 2.
        // Get total number of days elapsed.
        int dayDiff = (int)s.TotalDays;

        // 3.
        // Get total number of seconds elapsed.
        int secDiff = (int)s.TotalSeconds;

        // 4.
        // Don't allow out of range values.
        if (dayDiff < 0 || dayDiff >= 31)
        {
            return null;
        }

        // 5.
        // Handle same-day times.
        if (dayDiff == 0)
        {
            // A.
            // Less than one minute ago.
            if (secDiff < 60)
            {
                return "just now";
            }
            // B.
            // Less than 2 minutes ago.
            if (secDiff < 120)
            {
                return "1 minute ago";
            }
            // C.
            // Less than one hour ago.
            if (secDiff < 3600)
            {
                return string.Format("{0} minutes ago",
                    Math.Floor((double)secDiff / 60));
            }
            // D.
            // Less than 2 hours ago.
            if (secDiff < 7200)
            {
                return "1 hour ago";
            }
            // E.
            // Less than one day ago.
            if (secDiff < 86400)
            {
                return string.Format("{0} hours ago",
                    Math.Floor((double)secDiff / 3600));
            }
        }
        // 6.
        // Handle previous days.
        if (dayDiff == 1)
        {
            return "yesterday";
        }
        if (dayDiff < 7)
        {
            return string.Format("{0} days ago",
                dayDiff);
        }
        if (dayDiff < 31)
        {
            return string.Format("{0} weeks ago",
                Math.Ceiling((double)dayDiff / 7));
        }
        return null;
    }

    // Use this for initialization
 //   void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}
}
