using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class DecklistItem
{
    public string id;
    public int qty;

    public DecklistItem(string a_id, int a_qty)
    {
        id = a_id;
        qty = a_qty;
    }
}

public class DecklistArray
{
    public Decklist[] data;
}

[System.Serializable]
public class Decklist
{
    private static List<Decklist> _decks;
    public static List<Decklist> decks
    {
        get
        {
            LoadDecks();
            return _decks;
        }
    }

    public string name;
    public List<DecklistItem> list;
    public Decklist()
    {
        list = new List<DecklistItem>();
    }

    public static void LoadDecks()
    {
        if (_decks != null) { return; }
        _decks = new List<Decklist>();
        string fileName = Application.dataPath + "/Resources/decks.json";
        string json = File.ReadAllText(fileName);
        DecklistArray decksArray = JsonUtility.FromJson<DecklistArray>(json);
        foreach (Decklist list in decksArray.data)
        {
            _decks.Add(list);
        }
    }

    public static void SaveDeck(Decklist a_decklist)
    {
        for (int ii = _decks.Count-1; ii >= 0; ii--)
        {
            if (_decks[ii].name == a_decklist.name)
            {
                _decks.RemoveAt(ii);
            }
        }
        _decks.Add(a_decklist);
        DecklistArray decklistArray = new DecklistArray();
        decklistArray.data = _decks.ToArray();
        string fileName = Application.dataPath + "/Resources/decks.json";
        string json = JsonUtility.ToJson(decklistArray);
        File.WriteAllText(fileName, json);

    }

    public static Decklist Get(string a_name)
    {
        foreach (Decklist list in decks)
        {
            if (list.name == a_name) { return list; }
        }
        return null;
    }

    public static List<string> GetDeckNames()
    {
        List<string> decknames = new List<string>();
        foreach (Decklist list in decks)
        {
            decknames.Add(list.name);
        }
        return decknames;
    }
}

