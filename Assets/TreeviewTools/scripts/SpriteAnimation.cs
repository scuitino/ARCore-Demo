using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour {

    [SerializeField] Image image;
    [SerializeField] Sprite[] frames;
    [SerializeField] int framesPerSecond = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (image == null)
            return;
        if (frames.Length == 0 || frames[0] == null)
            return;

        int idx = Mathf.FloorToInt(Time.timeSinceLevelLoad * framesPerSecond);
        idx = idx % frames.Length;
        image.sprite = frames[idx];
    }
}
