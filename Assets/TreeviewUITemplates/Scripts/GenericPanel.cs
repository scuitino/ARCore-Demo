using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPanel : MonoBehaviour
{
    bool shown = false;
    public bool Shown {
        get { return shown; }
    }

    public virtual void Init()
    {

    }


    public virtual void Show(bool show)
    {
        shown = show;

        this.gameObject.SetActive(show);
    }


}