using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownLibrary : ObjectLibrary {

    [SerializeField] public Dropdown dropdown;


    protected override void Start()
    {
        base.Start();

        //dropdown.onValueChanged.AddListener(SelectLibraryObject);// += OnDropdownChanged;
        //if (dropdown.options.Count > 0)
        //    SelectLibraryObject(dropdown.value);
    }

    protected override void PopulateMenu()
    {
        dropdown.ClearOptions();
        if (libraryObjects == null)
            return;
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (LibraryObject libObj in libraryObjects)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = libObj.ToString();
            //option.image = ;
            options.Add(option);
            Debug.Log("Dropdown option: "+libObj.ToString());
        }
        dropdown.AddOptions(options);

        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener(SelectLibraryObject);// += OnDropdownChanged;
        if (dropdown.options.Count > 0)
            SelectLibraryObject(dropdown.value);
    }
}
