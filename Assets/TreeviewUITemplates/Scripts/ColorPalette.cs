using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class ColorPalette : GenericPanel
{

	public RectTransform ColorsParent;
	public GameObject ColorButtonPrefab;
    public OnColorSelectedEvent OnColorSelected = new OnColorSelectedEvent();

    public List<Color> Colors = new List<Color>();

    public List<Button> ColorButtonsList = new List<Button>();
    
	public static ColorPalette Instance;
	public Color SelectedColor = Color.white;
	

	void Awake()
	{
		Instance = this;
		SelectedColor = Color.white;
		
	}

	void Start () {
        
        if (Colors.Count > 0)
            InitColors();
	}

	public void OnColorClick(Color col)
	{
		Debug.Log("Color click: " + col);
		SelectedColor = col;
		
		OnColorSelected.Invoke(col);
		
		//Show(false);
	}
	public virtual void OnColorClick(string colorName){
		
		Debug.Log ("Parent Color Click: " + colorName);
	}
    public override void Show(bool show)
    {
        base.Show(show);
    }


    public void InitColors(List<string> colors)
    {
        Colors = new List<Color>();
        Colors.Add(Color.white);
        foreach (string s in colors)
        {
            Color col = Color.white;

            if (ColorUtility.TryParseHtmlString(s, out col))
            {
                Debug.Log("Color is : " + col.ToString());
            }
            else
            {
                Debug.Log("Color parse error cant parse " + s);
            }
            Colors.Add(col);
        }
        InitColors();
    }
    public virtual void InitColors()
    { 
		
	}
}
	
public class OnColorSelectedEvent : UnityEvent<Color>
{
	
}