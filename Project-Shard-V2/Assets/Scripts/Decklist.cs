using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DecklistItem
{
    public string id;
    public int qty;
}
[System.Serializable]
public class Decklist
{
    public List<DecklistItem> list;
    public Decklist()
    {
        list = new List<DecklistItem>();
    }
}
