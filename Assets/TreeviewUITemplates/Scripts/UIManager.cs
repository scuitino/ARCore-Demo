using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : SingletonBehaviourKeepLast <UIManager> {

    [SerializeField] protected Transform xorPanelsParent;
    protected List<GenericPanel> xorPanels = new List<GenericPanel>();
    protected GenericPanel currentPanel = null;
    public GenericPanel CurrentPanel { get { return currentPanel; } }

    public class PanelChangedEvent : UnityEvent<GenericPanel, GenericPanel> { }
    public PanelChangedEvent onPanelChanged = new PanelChangedEvent();

    protected override void Awake()
    {
        base.Awake();

        FetchPanels();
        InitPanels();
        if (xorPanels.Count>0)
            SetCurrentPanel(xorPanels[0]);
    }

    protected virtual void FetchPanels()
    {
        if (xorPanelsParent == null)
            xorPanelsParent = transform;
        xorPanels = xorPanelsParent.GetComponentsInChildren<GenericPanel>(true).ToList();
    }
    protected virtual void InitPanels()
    { 
        foreach (var p in xorPanels)
        {
            p.Init();
        }
    }

    // Use this for initialization
    protected virtual void Start () {
		
	}


    public T GetPanel<T>() where T : GenericPanel
    {
        bool hasPanel = false;
        Type actualT;
        foreach (var p in xorPanels)
        {
            if (typeof(T) == p.GetType())
            {
                //actualT = p.GetType();
                return p as T;
            }            
        }
        return null;
    }

    public virtual void SetCurrentPanel(GenericPanel panel)
    {
        if (!xorPanels.Contains(panel))
        {
            Debug.LogError("SetCurrentPanel not valid "+panel.gameObject.name);
            return;
        }

        GenericPanel oldPanel = currentPanel;
        currentPanel = panel;
        foreach(GenericPanel p in xorPanels)
        {
            p.Show(p == panel);
        }

        onPanelChanged.Invoke(oldPanel, currentPanel);
    }

    

    // Update is called once per frame
    protected virtual void Update () {
		
	}
}
