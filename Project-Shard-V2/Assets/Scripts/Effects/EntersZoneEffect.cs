using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntersZoneEffect : GameEffect
{
    private Card _card;
    private CardZone _destZone;
    private CardZone _prevZone;
    private bool _destFaceUp;
    private bool _useAnimationQueue;
    public EntersZoneEffect(Card a_card, CardZone a_zone, bool a_faceUp, bool a_useQueue = true)
    {
        _card = a_card;
        _destZone = a_zone;
        _prevZone = a_card.zone;
        _destFaceUp = a_faceUp;
        _useAnimationQueue = a_useQueue;
    }
    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        if (_prevZone.type != _destZone.type)
        {
            if (_prevZone.type == CardZone.Type.HAND) { _card.events.LeaveHand(); }
            if (_prevZone.type == CardZone.Type.ACTIVE) { _card.events.LeavePlay(); }
            if (_destZone.type == CardZone.Type.ACTIVE) { _card.events.EnterPlay(); }
            if (_prevZone.type == CardZone.Type.DECK && _destZone.type == CardZone.Type.DECK) { _card.events.Draw(); }
        }
        _card.Move(_destZone);
    }
    public override void Show(CardGame a_game)
    {
        _card.ui.Move(_destZone.ui, _destFaceUp, _useAnimationQueue);
    }

    public override void Undo(CardGame a_game)
    {
        _card.Move(_prevZone);
    }
}
