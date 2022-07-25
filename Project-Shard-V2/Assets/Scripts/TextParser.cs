using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class TextParser
{
    private static TextParser _instance;

    private List<CardData> _cards;
    private List<CardStats.Name> _cardStats;
    private List<Actor.StatName> _actorStats;
    private List<StatusEffect.Name> _statusEffects;
    private List<AbilityKeyword> _abilityKeywords;
    private List<ActionKeyword> _actionKeywords;
    private List<Keyword> _keywords;

    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = new TextParser();
        }
    }
    public TextParser()
    {
        _cards = new List<CardData>();
        _cardStats = new List<CardStats.Name>();
        _actorStats = new List<Actor.StatName>();
        _statusEffects = new List<StatusEffect.Name>();
        _abilityKeywords = new List<AbilityKeyword>();
        _actionKeywords = new List<ActionKeyword>();
        _keywords = new List<Keyword>();
    }
    public static string Parse(string input)
    {
        string output = input;
        output = ParseCards(output);
        output = ParseStatusEffects(output);
        output = ParseCardStats(output);
        output = ParseActorStats(output);
        output = ParseAbilityKeywords(output);
        output = ParseActionKeywords(output);
        output = ParseKeywords(output);
        output = ParseColors(output);
        output = ParseCardTypes(output);
        return output;
    }
    public static string ParseCards(string input)
    {
        Initialize();
        _instance._cards.Clear();
        string[] cardNameSplit = input.Split('&');
        string output = cardNameSplit[0];
        for (int ii = 1; ii < cardNameSplit.Length; ii += 2)
        {
            string card = cardNameSplit[ii];
            _instance._cards.Add(GameManager.cardIndex.Get(card.Replace(" ", "")));
            output += ("<color=#FF00FF><b>" + card + "</b></color>");
            if (ii + 1 < cardNameSplit.Length)
            {
                output += cardNameSplit[ii + 1];
            }
        }
        return output;
    }
    public static string ParseStatusEffects(string input)
    {
        Initialize();
        string output = input;
        _instance._statusEffects.Clear();
        foreach (StatusEffect.Name item in Enum.GetValues(typeof(StatusEffect.Name)))
        {
            string s = item.ToString();
            if (output.Contains(s.ToUpper())) { _instance._statusEffects.Add(item); }
            output = output.Replace(s, "<color=yellow>" + Capitalize(s.Replace("_", " ")) + "</color>");
        }
        return output;
    }
    public static string ParseActorStats(string input)
    {
        Initialize();
        string output = input;
        _instance._actorStats.Clear();
        foreach (Actor.StatName item in Enum.GetValues(typeof(Actor.StatName)))
        {
            string s = item.ToString();
            if (output.Contains(s.ToUpper())) { _instance._actorStats.Add(item); }
            output = output.Replace(s, "<color=yellow>" + Capitalize(s.Replace("_"," ")) + "</color>");
        }
        return output;
    }
    public static string ParseCardTypes(string input)
    {
        string output = input;
        foreach (Card.Type item in Enum.GetValues(typeof(Card.Type)))
        {
            string s = item.ToString();
            output = output.Replace(s, "<color=yellow>" + Capitalize(s.Replace("_", " ")) + "</color>");
        }
        return output;
    }
    public static string ParseCardStats(string input)
    {
        Initialize();
        string output = input;
        _instance._cardStats.Clear();
        foreach (CardStats.Name item in Enum.GetValues(typeof(CardStats.Name)))
        {
            string s = item.ToString();
            if (output.Contains(s.ToUpper())) { _instance._cardStats.Add(item); }
            output = output.Replace(s, "<color=yellow>" + Capitalize(s.Replace("_", " ")) + "</color>");
        }
        return output;
    }
    public static string ParseAbilityKeywords(string input)
    {
        Initialize();
        string output = input;
        _instance._abilityKeywords.Clear();
        foreach (AbilityKeyword item in Enum.GetValues(typeof(AbilityKeyword)))
        {
            string s = item.ToString();
            if (output.Contains(s.ToUpper())) { _instance._abilityKeywords.Add(item); }
            output = output.Replace(s, "<color=yellow>" + Capitalize(s.Replace("_", " ")) + "</color>");
        }
        return output;
    }
    public static string ParseActionKeywords(string input)
    {
        Initialize();
        string output = "";
        _instance._actionKeywords.Clear();
        string[] words = input.Split(' ');
        for (int ii = 0; ii < words.Length; ii++)
        {
            if (words[ii].Length > 0 && words[ii][0] == '$')
            {
                foreach (ActionKeyword item in Enum.GetValues(typeof(ActionKeyword)))
                {
                    string s = item.ToString();
                    if (words[ii].ToUpper().Contains(s))
                    {
                        _instance._actionKeywords.Add(item);
                        words[ii] = ("<color=yellow>" + words[ii].Substring(1) + "</color>");
                        break;
                    }
                }
            }
            if (ii > 0) { output += " "; }
            output += words[ii];
        }
        return output;
    }
    public static string ParseKeywords(string input)
    {
        Initialize();
        string output = "";
        _instance._keywords.Clear();
        string[] words = input.Split(' ');
        for (int ii = 0; ii < words.Length; ii++)
        {
            if (words[ii].Length <= 0) { continue; }
            Keyword key = ParseTextAsKeyword(words[ii]);
            if ((int)key > 0)
            {
                _instance._keywords.Add(key);
                words[ii] = ("<color=yellow>" + Capitalize(words[ii]) + "</color>");
            } 
            if (ii > 0) { output += " "; }
            output += words[ii];
        }
        return output;
    }
    public static string ParseColors(string input)
    {
        Initialize();
        string output = input;
        foreach (Card.Color item in Enum.GetValues(typeof(Card.Color)))
        {
            string s = item.ToString();
            output = output.Replace(s, "<color=yellow>" + Capitalize(s.Replace("_", " ")) + "</color>");
        }
        return output;
    }
    public static string Capitalize(string input)
    {
        return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
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
    public static string[] SplitPascalCase(string a_input)
    {
        return Regex.Replace(a_input, "([A-Z])", " $1", RegexOptions.Compiled).Split(' ');
    }
    public static Keyword ParseTextAsKeyword(string a_string)
    {
        string baseString = a_string.ToUpper();
        string[] keyStrings = {
            "T_" + baseString,
            "E_" + baseString,
            "C_" + baseString,
            "R_" + baseString,
            "S_" + baseString
        };
        foreach (Keyword item in Enum.GetValues(typeof(Keyword)))
        {
            string ks = item.ToString();
            foreach (string s in keyStrings)
            {
                if (s.Contains(ks))
                {
                    return item;
                }
            }
        }
        return (Keyword)(-1);
    }
}
