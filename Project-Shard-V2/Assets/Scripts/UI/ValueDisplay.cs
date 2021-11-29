using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ValueDisplay : MonoBehaviour
{
    
    [SerializeField] private bool displayBaseValue = false;
    [SerializeField] private bool displayHighlight = false;
    [SerializeField] private Color textColor;
    [SerializeField] private Color highlightHigh;
    [SerializeField] private Color highlightLow;

    private TextMeshProUGUI _text;
    private int _value = 0;
    private int _baseValue = 0;

    public int value
    {
        set
        {
            _value = value;
            UpdateText();
        }
    }

    public int baseValue
    {
        set
        {
            _baseValue = value;
            UpdateText();
        }
    }

    void UpdateText()
    {
        _text.text = "";
        if (displayHighlight)
        {
            if (_value > _baseValue)
            {
                _text.text += ("<#" + ColorUtility.ToHtmlStringRGB(highlightHigh) + ">");
            } else if (_value < _baseValue) {
                _text.text += ("<#" + ColorUtility.ToHtmlStringRGB(highlightLow) + ">");
            } else {
                _text.text += ("<#" + ColorUtility.ToHtmlStringRGB(textColor) + ">");
            }
        } else
        {
            _text.text += ("<#" + ColorUtility.ToHtmlStringRGB(textColor) + ">");
        }
        _text.text += (_value.ToString() + "</color>");
        if (displayBaseValue)
        {
            _text.text += ("<#" + ColorUtility.ToHtmlStringRGB(textColor) + ">");
            _text.text += ("/" + _baseValue.ToString() + "</color>");
        }
    }
    void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }
}
