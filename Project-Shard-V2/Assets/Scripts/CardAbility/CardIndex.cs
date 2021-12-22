using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CardIndex
{
    private List<CardData> _cards;

    public List<CardData> cards
    {
        get
        {
            List<CardData> allCards = new List<CardData>();
            foreach (CardData data in _cards)
            {
                allCards.Add(data);
            }
            Debug.Log("returning " + allCards.Count + " cards");
            return allCards;
        }
    }
    public CardIndex()
    {
        _cards = new List<CardData>();

        string path = Application.dataPath + "/Resources/Cards";
        List<string> sets = new List<string>();
        sets.Add("test.json");
        sets.Add("set1.json");
        foreach (string set in sets)
        {
            string json = File.ReadAllText(path + "\\" + set);

            json = "{\"data\":" + json + "}";
            CardArray cardArray = JsonUtility.FromJson<CardArray>(json);
            Array.Sort(cardArray.data);

            for (int ii = 0; ii < cardArray.data.Length; ii++)
            {
                _cards.Add(cardArray.data[ii]);
                //Debug.Log("Added Card: " + cardArray.data[ii].name);
            }
        }
        Debug.Log("Added " + _cards.Count + " cards");
    }

    public CardData Get(string a_id)
    {
        foreach (CardData data in _cards)
        {
            if (data.id == a_id) { return data; }
        }
        Debug.Log("CardIndex::Get | Warning: Card not found: " + a_id);
        return null;
    }

    
}
