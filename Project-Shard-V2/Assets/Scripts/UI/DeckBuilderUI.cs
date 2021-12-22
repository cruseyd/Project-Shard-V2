using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderUI : MonoBehaviour
{
    [SerializeField] private CardZoneUI _cardPool;

    private void Awake()
    { }


    private void Start()
    {
        List<CardData> cards = GameManager.cardIndex.cards;
        foreach (CardData data in cards)
        {
            CardUI card = CardUI.Spawn(data, _cardPool);
            card.FaceUp(true);
        }
    }
    private void Update()
    {
    }
}
