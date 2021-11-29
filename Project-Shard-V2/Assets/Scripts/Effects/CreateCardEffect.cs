using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCardEffect : GameEffect
{
    private Card _card;
    private CardData _data;
    private CardZone _zone;
    public CreateCardEffect(CardData a_data, CardZone a_zone)
    {
        _data = a_data;
        _zone = a_zone;
    }
    public override void Execute(CardGame a_game)
    {
        _card = Card.Get(a_game, _data, _zone.owner);
        _zone.Add(_card);
    }

    public override void Show(CardGame a_game)
    {
        _card.Spawn(Vector3.zero, _zone.ui);
    }

    public override void Undo(CardGame a_game)
    {
        _zone.Remove(_card);
    }
}
