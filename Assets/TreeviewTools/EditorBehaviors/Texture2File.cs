

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

using UnityEditor;
[CustomEditor(typeof(Texture2File))]
public class Texture2FileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Texture2File myTarget = (Texture2File)target;

        myTarget.image = (Image)EditorGUILayout.ObjectField("Image", myTarget.image, typeof(Image), true);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(3));// Separator

        myTarget.rawImage = (RawImage)EditorGUILayout.ObjectField("RawImage", myTarget.rawImage, typeof(RawImage), true);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(3));// Separator

        myTarget.texture = (Texture2D) EditorGUILayout.ObjectField("Texture", myTarget.texture,typeof(Texture2D), true);

        myTarget.writeFilename = EditorGUILayout.TextField("Write filename", myTarget.writeFilename);
        if (GUILayout.Button("Write Texture to File"))
        {
            myTarget.WriteTexture();
        }
    }
}


public class Texture2File : MonoBehaviour {

    //public Renderer textureRenderer;
    public Image image;
    public RawImage rawImage;
    public Texture2D texture;
    public string writeFilename;
    
	// Use this for initialization
	void Start () {
        //if (textureRenderer = null)
        //    textureRenderer = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void WriteTexture()
    {
        Texture2D t = texture;
        if (t == null)
        {
            if (rawImage != null)
            {
                t = rawImage.texture as Texture2D;
            }
            else if (image != null)
            {
                t = image.sprite.texture;
            }
        }

        if (t == null)
        {
            Debug.LogError("No texture renderer assigned.");
            return;
        }

        string path = writeFilename;
        byte[] bytes = t.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        Debug.Log("Texture wrote OK to file: "+path);
    }
}
#endif