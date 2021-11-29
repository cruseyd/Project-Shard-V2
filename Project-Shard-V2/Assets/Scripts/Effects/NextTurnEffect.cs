using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextTurnEffect : GameEffect
{
    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        a_game.currentPlayer.events.EndTurn();
        a_game.NextTurn();
    }
    public override void Show(CardGame a_game)
    {
        foreach (Actor player in a_game.players)
        {
            player.ui.state = ITargetUI.State.SOURCE;
        }
        a_game.currentPlayer.ui.state = ITargetUI.State.DEFAULT;
    }

    public override void Undo(CardGame a_game)
    {
        a_game.PrevTurn();
    }
}
