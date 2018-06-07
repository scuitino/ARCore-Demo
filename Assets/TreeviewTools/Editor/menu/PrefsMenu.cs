
using UnityEditor;
using UnityEngine;
public class PrefsMenu : MonoBehaviour
{
    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    [MenuItem("TreeviewTools/Clear Prefs")]
    static void ClearPrefs()
    {
        Debug.Log("Clearing Prefs");
        PlayerPrefs.DeleteAll();
    }
}