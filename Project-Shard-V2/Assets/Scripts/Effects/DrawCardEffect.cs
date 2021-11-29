using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : GameEffect
{
    private Actor _player;
    private Card _drawnCard;
    private bool _cycleDeck;

    public DrawCardEffect(Actor a_player)
    {
        _player = a_player;
        _cycleDeck = false;
    }
    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        if (_player.deck.cards.Count == 0)
        {
            List<Card> discardPile = _player.discard.cards;
            Debug.Log("Deck is empty, shuffling " + discardPile.Count + " cards.");
            for (int ii = discardPile.Count - 1; ii >= 0; ii--)
            {
                EntersZoneEffect insertEffect = new EntersZoneEffect(discardPile[ii], _player.deck, false, false);
                insertEffect.Execute(a_game);
            }
            _player.deck.Shuffle();
            IncrementActorStatEffect addResourceEffect = new IncrementActorStatEffect(Actor.StatName.MAX_FOCUS, 1, _player);
            addResourceEffect.Execute(a_game);
        }
        _drawnCard = _player.Draw();
        EntersZoneEffect drawEffect = new EntersZoneEffect(_drawnCard, _player.hand, true);
        drawEffect.Execute(a_game);
    }

    public override void Show(CardGame a_game)
    {
        //_drawnCard.ui.Move(_player.ui.zones[CardZone.Type.HAND], _player.playerControlled);
        //_drawnCard.ui.Move(_player.ui.zones[CardZone.Type.HAND], true);
    }

    public override void Undo(CardGame a_game)
    {
        //Debug.Assert(_player.hand.cards.Contains(_drawnCard));

        //_player.hand.Remove(_drawnCard);
        //_player.deck.InsertTop(_drawnCard);
    }
}
