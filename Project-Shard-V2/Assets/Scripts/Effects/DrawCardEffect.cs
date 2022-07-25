using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : GameEffect
{
    private Actor _player;
    private Card _drawnCard;

    public DrawCardEffect(Actor a_player)
    {
        _player = a_player;
    }
    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        if (_player.deck.cards.Count == 0)
        {
            List<Card> discardPile = _player.discard.cards;
            for (int ii = discardPile.Count - 1; ii >= 0; ii--)
            {
                EntersZoneEffect insertEffect = new EntersZoneEffect(discardPile[ii], _player.deck, false, false);
                insertEffect.Execute(a_game);
            }
            _player.deck.Shuffle();
            if (_player.GetStat(Actor.StatName.MAX_FOCUS) < CardGameParams.playerMaxResource)
            {
                IncrementActorStatEffect addResourceEffect = new IncrementActorStatEffect(Actor.StatName.MAX_FOCUS, 1, _player);
                addResourceEffect.Execute(a_game);
            }
            if (_player.GetStat(Actor.StatName.INFLUENCE) < CardGameParams.playerMaxInfluence)
            {
                IncrementActorStatEffect addInfluenceEffect = new IncrementActorStatEffect(Actor.StatName.INFLUENCE, 1, _player);
                addInfluenceEffect.Execute(a_game);
            }
        }
        _drawnCard = _player.Draw();
        EntersZoneEffect drawEffect = new EntersZoneEffect(_drawnCard, _player.hand, true);
        drawEffect.Execute(a_game);
    }

    public override void Show(CardGame a_game)
    {
    }

    public override void Undo(CardGame a_game)
    {
        // Undo is unnecessary. Handled by EntersZone effects.
    }
}
