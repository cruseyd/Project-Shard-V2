using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderUI : MonoBehaviour
{
    [SerializeField] private Transform _cardPool;
    private List<CardZoneUI> _cardPoolRows;

    private void Awake()
    {
        _cardPoolRows = new List<CardZoneUI>();
        CardZoneUI[] cardZones = _cardPool.GetComponentsInChildren<CardZoneUI>();
        Debug.Log("Found " + cardZones.Length + " CardZoneUI");
        foreach (CardZoneUI ui in cardZones)
        {
            ui.Organize();
            _cardPoolRows.Add(ui);
        }
    }

    private void Update()
    {
        foreach (CardZoneUI ui in _cardPoolRows)
        {
           // ui.Organize();
        }
    }
}
