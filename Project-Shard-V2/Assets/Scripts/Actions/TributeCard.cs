using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TributeCard : GameAction
{
    private Card _tribute;
    public override Actor actor { get { return _tribute.owner; } }

    public TributeCard(Card a_card) { _tribute = a_card; }
    public override void Execute(CardGame a_game)
    {
        IncrementActorStatEffect incrementEffect
            = new IncrementActorStatEffect(_tribute.data.thresholdYield, 1, _tribute.owner);
        incrementEffect.Execute(a_game);
        EntersZoneEffect discardEffect
            = new EntersZoneEffect(_tribute, _tribute.owner.discard, true);
        discardEffect.Execute(a_game);
        actor.canTribute = false;
    }

    public override bool IsValid(CardGame a_game)
    {
        return a_game.IsActorTurn(_tribute.owner);
    }

    public override void Preview(CardGame a_game)
    {
    }
    public override void Undo(CardGame a_game)
    {
        base.Undo(a_game);
        actor.canTribute = true;
    }
}
