using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DismissUnit : GameAction
{
    private UnitCard _card;
    public override Actor actor { get { return _card.owner; } }
    public DismissUnit(UnitCard a_card)
    {
        _card = a_card;
    }

    public override void Execute(CardGame a_game)
    {
        EntersZoneEffect leavePlayEffect = new EntersZoneEffect(_card, _card.owner.discard, true);
        leavePlayEffect.Execute(a_game);
    }

    public override void Preview(CardGame a_game)
    {
    }

    public override bool IsValid(CardGame a_game)
    {
        return a_game.IsActorTurn(actor);
    }
}
