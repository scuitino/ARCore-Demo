using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollLibrary : ObjectLibrary {
    
    [SerializeField] public Transform scrollContents;
    [SerializeField] public ToggleGroup toggleGroup;

    [SerializeField] public GameObject itemPrefab;
    List<LibraryItemUI> uiItems;

    protected override void Start()
    {
        base.Start();

        //dropdown.onValueChanged.AddListener(SelectLibraryObject);// += OnDropdownChanged;
        //if (dropdown.options.Count > 0)
        //    SelectLibraryObject(dropdown.value);
    }

    protected override void PopulateMenu()
    {
        foreach(Transform t in scrollContents)
        {
            Destroy(t.gameObject);
        }
        
        if (libraryObjects == null)
            return;
        uiItems = new List<LibraryItemUI>();
        foreach (LibraryObject libObj in libraryObjects)
        {
            GameObject item = Instantiate(itemPrefab, scrollContents);
            LibraryItemUI itemUI = item.GetComponent<LibraryItemUI>();
            if (itemUI == null)
            {
                Debug.LogError("ScrollLibrary item prefab doesnt contain a LibraryItemUI");
                return;
            }
            itemUI.Populate(this, libObj, toggleGroup);
            uiItems.Add(itemUI);

            Debug.Log("Scroll item: " + libObj.ToString());
        }

        //dropdown.onValueChanged.RemoveAllListeners();
        //dropdown.onValueChanged.AddListener(SelectLibraryObject);// += OnDropdownChanged;
        //if (dropdown.options.Count > 0)
        //SelectLibraryObject(dropdown.value);

        ScrollAlignHACK();
    }

    void ScrollAlignHACK()
    {
        StartCoroutine(_ScrollAlignHACK());
    }
    IEnumerator _ScrollAlignHACK()
    {
        // HACK to make horizontal layout group aligns stuff correctly - all items superposed otherwise

        //LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContents.transform as RectTransform);

        //LayoutGroup layoutGroup = scrollContents.GetComponent<LayoutGroup>();
        //if (layoutGroup != null)
        //{
        //yield return new WaitForEndOfFrame();
        //yield return new WaitForEndOfFrame();
        //layoutGroup.enabled = false;
        yield return new WaitForEndOfFrame();
        //yield return new WaitForEndOfFrame();
        //yield return new WaitForEndOfFrame();
        //yield return new WaitForEndOfFrame();
        ////yield return new WaitForSeconds(5f);
        //layoutGroup.enabled = true;
        //}

        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContents.transform as RectTransform);
    }

    public override void SelectLibraryObject(LibraryObject libObj)
    {
        if (!IsLibraryObject(libObj))
        {
            return;
        }

        base.SelectLibraryObject(libObj);
        
        if (libObj == null)
        {
            foreach(LibraryItemUI item in uiItems)
            {
                item.Deselect();
            }
        }
    }
}
