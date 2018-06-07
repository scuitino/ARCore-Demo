using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwapSprite : MonoBehaviour {

    [SerializeField] Image targetImage;

    [SerializeField] List<Sprite> sprites;
    Sprite _currentSprite;
    int _currentSpriteIdx = -1;
    public Sprite CurrentSprite {
        get
        {
            return _currentSprite;
        }
        set
        {
            _currentSprite = value;
            _currentSpriteIdx = sprites.IndexOf(_currentSprite);
            targetImage.sprite = _currentSprite;
        }
    }

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponentInChildren<Image>();

        if (targetImage == null || sprites.Count == 0)
            return;

        CurrentSprite = sprites[0];
    }

    public void Next()
    {
        if (targetImage == null || sprites.Count == 0)
            return;

        int nextIdx = (_currentSpriteIdx + 1) % sprites.Count;
        CurrentSprite = sprites[nextIdx];
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	//void Update () {
		
	//}
}
