using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardZone
{
    public enum Type
    {
        DEFAULT,
        DECK,
        HAND,
        DISCARD,
        ACTIVE,
        PREVIEW
    }

    protected CardGame _game;

    public CardZoneUI ui { get; protected set; }
    public readonly Type type;
    public readonly Actor owner;
    public List<Card> cards { get; private set; }

    public CardZone(CardGame a_game, Actor a_owner, Type a_type)
    {
        _game = a_game;
        type = a_type;
        owner = a_owner;
        cards = new List<Card>();
    }

    public void SetUI(CardZoneUI a_ui)
    {
        if (ui != null) { return; }
        ui = a_ui;
    }

    public virtual void Add(Card a_card)
    {
        if (cards.Contains(a_card)) { return; }
        cards.Add(a_card);
    }
    public virtual void Remove(Card a_card)
    {
        cards.Remove(a_card);
    }
}
