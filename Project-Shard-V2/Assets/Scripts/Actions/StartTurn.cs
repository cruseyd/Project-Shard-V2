using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTurn : GameAction
{
    private Actor _actor;
    public override Actor actor { get { return _actor; } }
    public StartTurn(Actor a_actor)
    {
        _actor = a_actor;
        previewForAI = false;
    }

    public override void Execute(CardGame a_game)
    {
        SetTurnEffect setTurnEffect = new SetTurnEffect(a_game, _actor);
        setTurnEffect.Execute(a_game);
        int maxFocus = _actor.GetStat(Actor.StatName.MAX_FOCUS);
        int focus = _actor.GetStat(Actor.StatName.FOCUS);
        IncrementActorStatEffect incrementResource = new IncrementActorStatEffect(Actor.StatName.FOCUS, maxFocus - focus, _actor);
        incrementResource.Execute(a_game);
    }

    public override void Preview(CardGame a_game)
    {
    }
    public override bool IsValid(CardGame a_game)
    {
        return true;
    }
}
