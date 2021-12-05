using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : CardZone
{
    public Deck(CardGame a_game, Actor a_owner, Decklist a_list) : base(a_game, a_owner, CardZone.Type.DECK)
    {
        SetList(a_list);
    }

    public void SetList(Decklist a_list)
    {
        foreach (Card card in cards)
        {
            card.Destroy();
        }
        if (a_list != null)
        {
            foreach (DecklistItem item in a_list.list)
            {
                CardData data = CombatManager.cardIndex.Get(item.id);
                for (int ii = 0; ii < item.qty; ii++)
                {
                    Card card = Card.Get(_game, data, owner);
                    card.Move(this);
                }
            }
        }
    }

    public void SpawnCards()
    {
        foreach (Card card in cards)
        {
            card.SetOwner(owner);
            card.Spawn(ui.transform.position, ui);
            card.ui.FaceUp(false);
        }
    }
    public override void Add(Card a_card)
    {
        Shuffle(a_card);
    }
    public void Insert(Card a_card, int a_index)
    {
        if (cards.Contains(a_card))
        { cards.Remove(a_card); }
        cards.Insert(a_index, a_card);
    }
    public void InsertTop(Card a_card)
    {
        Insert(a_card, 0);
    }
    public void Shuffle(Card a_card)
    {
        if (cards.Contains(a_card)) { return; }
        int index = Random.Range(0, cards.Count);
        cards.Insert(index, a_card);
    }

    public void Shuffle()
    {
        for (int ii = 0; ii < cards.Count; ii++)
        {
            int a = Random.Range(0, cards.Count);
            int b = Random.Range(0, cards.Count);
            if (a == b) { continue; }
            Card tmp = cards[a];
            cards[a] = cards[b];
            cards[b] = tmp;
        }
    }

    public Card Draw()
    {
        if (cards.Count == 0) { return null; }
        return cards[0];
    }
}
