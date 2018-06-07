using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindShaderUse : EditorWindow
{
    string st = "";
    string stArea = "Empty List";

    [MenuItem("TreeviewTools/Find Shader Use")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindShaderUse));
    }

    public void OnGUI()
    {
        GUILayout.Label("Enter shader to find:");
        st = GUILayout.TextField(st);
        if (GUILayout.Button("Find Materials"))
        {
            FindShader(st);
        }
        GUILayout.Label(stArea);
    }

    private void FindShader(string shaderName)
    {
        int count = 0;
        stArea = "Materials using shader " + shaderName + ":\n\n";

        List<Material> armat = new List<Material>();

        Renderer[] arrend = (Renderer[])Resources.FindObjectsOfTypeAll(typeof(Renderer));
        foreach (Renderer rend in arrend)
        {
            foreach (Material mat in rend.sharedMaterials)
            {
                if (!armat.Contains(mat))
                {
                    armat.Add(mat);
                }
            }
        }

        foreach (Material mat in armat)
        {
            //if (mat != null && mat.shader != null && mat.shader.name != null && mat.shader.name == shaderName)
            if (mat != null && mat.shader != null && mat.shader.name != null && mat.shader.name.IndexOf(shaderName) != -1)
            {
                stArea += ">" + mat.name + "\n";
                count++;
            }
        }

        stArea += "\n" + count + " materials using shader " + shaderName + " found.";
    }
}