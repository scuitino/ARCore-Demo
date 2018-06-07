using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkTools : MonoBehaviour {

    

    public static string GetBasicAuthentication(string username, string password)
    {
        string auth = username + ":" + password;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = "Basic " + auth;
        return auth;
    }

    public static UnityWebRequest CreateJSONPost(string url, string jsonWithSingleQuotes, string authentication = null)
    {
        //Replace single ' for double " 
        //This is usefull if we have a big json object, is more easy to replace in another editor the double quote by singles one
        string json = jsonWithSingleQuotes.Replace("'", "\"");
        //Debug.Log("JSON: " + json);
        //byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        byte[] bodyRaw = Encoding.ASCII.GetBytes(json);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");
        www.chunkedTransfer = false; // FIX 411 - LENGTH
        if (!string.IsNullOrEmpty(authentication))
            www.SetRequestHeader("AUTHORIZATION", authentication);
        return www;
    }
}
