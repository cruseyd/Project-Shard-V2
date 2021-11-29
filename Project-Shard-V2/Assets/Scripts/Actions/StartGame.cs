using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : GameAction
{
    public override Actor actor { get { return null; } }
    public StartGame()
    {
        previewForAI = false;
    }
    public override void Execute(CardGame a_game)
    {
        foreach (Actor player in a_game.players)
        {
            for (int ii = 0; ii < CardGameParams.playerHandSize; ii++)
            {
                DrawCardEffect effect = new DrawCardEffect(player);
                effect.Execute(a_game);
            }
        }
    }

    public override bool IsValid(CardGame a_game)
    {
        return true;
    }

    public override void Preview(CardGame a_game)
    {
    }
}
