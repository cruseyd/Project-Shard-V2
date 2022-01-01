using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SearchFilterUI : MonoBehaviour
{
    public Toggle toggle;
    public TMP_InputField input;
    private void Awake()
    {
        toggle.isOn = false;
    }
}
