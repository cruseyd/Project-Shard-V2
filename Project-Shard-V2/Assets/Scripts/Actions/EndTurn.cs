using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurn : GameAction
{
    private Actor _actor;
    public override Actor actor
    {
        get
        {
            return _actor;
        }
    }
    public EndTurn(Actor a_actor)
    {
        _actor = a_actor;
    }

    public override void Execute(CardGame a_game)
    {
        List<Card> handCards = _actor.hand.cards;
        for (int ii = handCards.Count-1; ii >= 0; ii--)
        {
            Card card = handCards[ii];
            EntersZoneEffect cycleEffect = new EntersZoneEffect(card, _actor.discard, true, false);
            cycleEffect.Execute(a_game);
            card.events.Cycle();
        }
        for (int ii = 0; ii < CardGameParams.playerHandSize; ii++)
        {
            DrawCardEffect drawEffect = new DrawCardEffect(_actor);
            drawEffect.Execute(a_game);
        }

        NextTurnEffect turnEffect = new NextTurnEffect();
        turnEffect.Execute(a_game);
    }

    public override void Preview(CardGame a_game)
    {
    }
    public override bool IsValid(CardGame a_game)
    {
        return a_game.IsActorTurn(_actor);
    }
}
