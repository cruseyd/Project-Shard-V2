using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextUtils
{
    public static string Capitalize(string a_input)
    {
        return a_input[0].ToString().ToUpper() + a_input.Substring(1).ToLower();
    }
    public static string AllCapsToPascal(string a_input)
    {
        string[] words = a_input.Split('_');
        string output = "";
        foreach (string word in words)
        {
            output += Capitalize(word);
        }
        return output;
    }
}
