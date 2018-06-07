using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePanel : GenericPanel {

    [SerializeField] public TreeviewStore store;

    [SerializeField] GameObject loading;
    [SerializeField] Transform itemsParent;
    [SerializeField] GameObject itemPrefab;

    List<StoreItemUI> uiItems;

    private void Awake()
    {
        store.onStoreReady.AddListener(OnStoreReady);
        store.onStoreItemReady.AddListener(OnStoreItemReady);
    }

    public override void Show(bool show)
    {
        base.Show(show);

        if (show)
        {
            if (store.state.CurrentState == TreeviewStore.State.READY)
            {
                // populate items
                loading.SetActive(false);
                PopulateItems();
            }
            else
            {
                // show loading if Store isnt ready
                loading.SetActive(true);
            }
        }
    }

    void PopulateItems()
    {
        foreach(Transform t in itemsParent)
        {
            Destroy(t.gameObject);
        }
        uiItems = new List<StoreItemUI>();
        foreach(ScriptableStoreItem item in store.storeItems.items)
        {
            GameObject GO = Instantiate(itemPrefab, itemsParent);
            StoreItemUI newItemUI = GO.GetComponent<StoreItemUI>();
            newItemUI.Populate(this, item);
            uiItems.Add(newItemUI);
        }
    }


    void OnStoreReady(ScriptableStoreItems items)
    {
        // populate items
        loading.SetActive(false);
        PopulateItems();
    }

    void OnStoreItemReady(ScriptableStoreItem item, Object asset)
    {
        // event is called at startup for downloaded items, then when bought&downloaded
        if (store.state.CurrentState == TreeviewStore.State.LOADING_DOWNLOADED_ITEMS)
        {
            //add asset items to Library

        }
        else
        {
            // hide item buy loading
            StoreItemUI itemUI = GetItemUI(item);
            itemUI.RefreshState();

            //add asset items to Library
        }
    }

    StoreItemUI GetItemUI(ScriptableStoreItem item)
    {
        foreach(StoreItemUI itemUI in uiItems)
        {
            if (itemUI.item == item)
                return itemUI;
        }
        return null;
    }
}
