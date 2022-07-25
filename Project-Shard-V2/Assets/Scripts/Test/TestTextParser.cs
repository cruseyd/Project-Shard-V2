using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class TestTextParser : MonoBehaviour
{
    public string input;
    public TextMeshProUGUI text;
    void Update()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
        text.text = "";
        string[] split = TextParser.SplitPascalCase(input);
        foreach (string s in split)
        {
            text.text += (s + " | ");
        }
    }
}
