using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTurnEffect : GameEffect
{
    private Actor _actor;
    private Actor _prevActor;
    public SetTurnEffect(CardGame a_game, Actor a_actor)
    {
        _actor = a_actor;
        _prevActor = a_game.currentPlayer;
    }
    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        a_game.SetTurn(_actor);
        _actor.events.StartTurn();
    }
    public override void Show(CardGame a_game)
    {
        foreach (Actor player in a_game.players)
        {
            player.ui.state = ITargetUI.State.DEFAULT;
        }
        a_game.currentPlayer.ui.state = ITargetUI.State.SOURCE;
    }

    public override void Undo(CardGame a_game)
    {
        a_game.SetTurn(_prevActor);
    }
}
