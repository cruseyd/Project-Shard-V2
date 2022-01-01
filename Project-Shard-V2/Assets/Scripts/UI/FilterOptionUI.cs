using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class FilterOptionUI : MonoBehaviour
{
    [System.Serializable]
    public enum DropdownType
    {
        Default,
        Color,
        Keyword,
        Level
    }
    [SerializeField] private TextMeshProUGUI _title;

    public DropdownType dropdownType;
    public Toggle toggle;
    public TMP_Dropdown dropdown;
    public int rangeLow;
    public int rangeHigh;
    private void Awake()
    {
        toggle.isOn = false;
        _title.text = dropdownType.ToString();
        PopulateDropdown(dropdown, dropdownType);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void PopulateDropdown(TMP_Dropdown a_dropdown, DropdownType a_type)
    {
        a_dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();
        Type enumType = null;
        switch (a_type)
        {
            case DropdownType.Color: enumType = typeof(Card.Color); break;
            case DropdownType.Keyword: enumType = typeof(Keyword); break;
            default: break;
        }
        if (enumType == null)
        {
            for (int ii = rangeLow; ii <= rangeHigh; ii++)
            {
                newOptions.Add(new TMP_Dropdown.OptionData(ii.ToString()));
            }
        } else if (enumType == typeof(Keyword))
        {
            for (int ii = 0; ii < Enum.GetNames(enumType).Length; ii++)
            {
                string baseKeyName = Enum.GetName(enumType, ii);
                string keyName = baseKeyName.Split('_')[1];
                keyName = char.ToUpper(keyName[0]) + keyName.Substring(1).ToLower();
                newOptions.Add(new TMP_Dropdown.OptionData(keyName));
            }
            for (int ii = rangeLow; ii <= rangeHigh; ii++)
            {
                newOptions.Add(new TMP_Dropdown.OptionData(ii.ToString()));
            }
        } else 
        {
            for (int ii = 0; ii < Enum.GetNames(enumType).Length; ii++)
            {
                newOptions.Add(new TMP_Dropdown.OptionData(Enum.GetName(enumType, ii)));
            }
        }
        a_dropdown.AddOptions(newOptions);
    }
}
