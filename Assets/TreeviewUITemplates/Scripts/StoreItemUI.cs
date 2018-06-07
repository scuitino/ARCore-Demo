using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemUI : MonoBehaviour {

    [SerializeField] Text nameLabel;
    [SerializeField] Image image;
    [SerializeField] Button buyBt;
    [SerializeField] Button downloadBt;

    public StorePanel store;
    public ScriptableStoreItem item;

    // Use this for initialization
    void Start () {
		
	}

    //// Update is called once per frame
    //void Update () {

    //}

    public void Populate(StorePanel store, ScriptableStoreItem item)
    {
        this.store = store;
        this.item = item;

        nameLabel.text = item.descriptiveName;
        if (item.thumbnail != null)
            image.sprite = item.thumbnail;

        buyBt.onClick.AddListener(() => {
            store.store.BuyItem(item);
        });
        downloadBt.onClick.AddListener(() => {
            store.store.LoadItem(item);
        });

        RefreshState();
    }

    public void RefreshState()
    {
        buyBt.interactable = !store.store.IsItemBought(item.GetThisPlatformId());        
        
        downloadBt.interactable = (store.store.IsItemBought(item.GetThisPlatformId()) && 
                                    !store.store.IsItemDownloaded(item.GetThisPlatformId()));
    }
}
