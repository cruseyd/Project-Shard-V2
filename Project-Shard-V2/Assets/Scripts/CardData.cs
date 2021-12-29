using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class CardArray
{
    public CardData[] data;
}

[System.Serializable]
public class CardData : IComparable
{
    public string tst;
    public string impl;
    public string set;

    public string name;

    public string stype;
    public string k1;
    public string k2;
    public string k3;

    public string scolor;
    public int level;
    public string t1;
    public string t2;
    public string t3;
    public string t4;
    public string t5;

    public int power;
    public int health;
    public int defiance;

    public string basetext;
    public int var1;
    public int var2;
    public int var3;

    public string ka1;
    public string ka2;
    public string ka3;

    public string text
    {
        get
        {
            if (basetext == null) { return ""; }
            string[] bits = basetext.Split('#');
            string t = bits[0];
            for (int ii = 1; ii < bits.Length; ii++)
            {
                t += (Var(ii - 1) + bits[ii]);
            }
            return t;
        }
    }
    public string id
    {
        get
        {
            return name.Replace(" ", "");
        }
    }

    public List<Keyword> keywords
    {
        get
        {
            List<Keyword> keys = new List<Keyword>();
            if (k1 != null) { keys.Add((Keyword)Enum.Parse(typeof(Keyword), k1.ToUpper())); }
            if (k2 != null) { keys.Add((Keyword)Enum.Parse(typeof(Keyword), k2.ToUpper())); }
            if (k3 != null) { keys.Add((Keyword)Enum.Parse(typeof(Keyword), k3.ToUpper())); }
            return keys;
        }
    }

    public List<AbilityKeyword> abilityKeywords
    {
        get
        {
            List<AbilityKeyword> keys = new List<AbilityKeyword>();
            if (ka1 != null) { keys.Add((AbilityKeyword)Enum.Parse(typeof(AbilityKeyword), ka1.ToUpper())); }
            if (ka2 != null) { keys.Add((AbilityKeyword)Enum.Parse(typeof(AbilityKeyword), ka2.ToUpper())); }
            if (ka3 != null) { keys.Add((AbilityKeyword)Enum.Parse(typeof(AbilityKeyword), ka3.ToUpper())); }
            return keys;
        }
    }

    public int Var(int a_index)
    {
        switch (a_index)
        {
            case 0: return var1;
            case 1: return var2;
            case 2: return var3;
            default: Debug.LogError("CardData::Var | Error: Index out of bounds: " + a_index); return 0;
        }
    }

    public CardData() { }
    public Card.Type type
    {
        get
        {
            switch (stype)
            {
                case "ACTION":
                case "SPELL":
                    return Card.Type.ACTION;
                case "UNIT":
                case "FOLLOWER":
                    return Card.Type.FOLLOWER;
                default:
                    return Card.Type.DEFAULT;
            }
        }
    }
    public Card.Color color
    {
        get
        {
            switch (scolor)
            {
                case "RAIZ":
                case "RED":
                    return Card.Color.RED;
                case "FEN":
                case "GREEN":
                    return Card.Color.GREEN;
                case "IRI":
                case "BLUE":
                    return Card.Color.BLUE;
                case "LIS":
                case "VIOLET":
                    return Card.Color.VIOLET;
                case "GOLD":
                case "ORA":
                    return Card.Color.GOLD;
                case "INDIGO":
                case "VAEL":
                    return Card.Color.INDIGO;
                default:
                    return Card.Color.DEFAULT;
            }
        }
    }
    public Actor.StatName thresholdYield
    {
        get
        {
            switch (color)
            {
                case Card.Color.RED: return Actor.StatName.THRESHOLD_RED;
                case Card.Color.GREEN: return Actor.StatName.THRESHOLD_GRN;
                case Card.Color.BLUE: return Actor.StatName.THRESHOLD_BLU;
                case Card.Color.VIOLET: return Actor.StatName.THRESHOLD_VLT;
                case Card.Color.GOLD: return Actor.StatName.THRESHOLD_GLD;
                case Card.Color.INDIGO: return Actor.StatName.THRESHOLD_IGO;
            }
            return Actor.StatName.DEFAULT;
        }
    }
    public int GetStat(CardStats.Name a_stat)
    {
        switch (a_stat)
        {
            case CardStats.Name.LEVEL: return level;
            case CardStats.Name.POWER: return power;
            case CardStats.Name.HEALTH: return health;
            case CardStats.Name.MAX_HEALTH: return health;
            case CardStats.Name.DEFIANCE: return defiance;
            default:
                Debug.LogError("CardData::GetStat | Error: Unrecognized Stat Name: " + a_stat);
                return 0;
        }
    }
    
    public int CompareTo(object obj)
    {
        if (obj is CardData)
        {
            CardData data = obj as CardData;
            // sorting: level -> alphabetical
            int comparison = level.CompareTo(data.level);
            if (comparison == 0)
            {
                comparison = name.CompareTo(data.name);
            }
            return comparison;
        } else if (obj is string)
        {
            return id.CompareTo(obj);
        } else
        {
            Debug.LogError("CardData::CompareTo | Error: Cannot compare a CardData to a " + obj);
        }
        return -1;
    }
    
    public Card.Color Threshold(int a_index)
    {
        switch (a_index)
        {
            case 0: if (t1 != null) { return ParseAbbreviatedColor(t1); } break;
            case 1: if (t2 != null) { return ParseAbbreviatedColor(t2); } break;
            case 2: if (t3 != null) { return ParseAbbreviatedColor(t3); } break;
            case 3: if (t4 != null) { return ParseAbbreviatedColor(t4); } break;
            case 4: if (t5 != null) { return ParseAbbreviatedColor(t5); } break;
        }
        return Card.Color.DEFAULT;
    }
    public Card.Color ParseAbbreviatedColor(string a_string)
    {
        switch (a_string)
        {
            case "R": return Card.Color.RED;
            case "G": return Card.Color.GREEN;
            case "B": return Card.Color.BLUE;
            case "V": return Card.Color.VIOLET;
            case "Y": return Card.Color.GOLD;
            case "I": return Card.Color.INDIGO;
            default: return Card.Color.DEFAULT;
        }
    }

}
