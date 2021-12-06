using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResetCardEffect : GameEffect
{
    private CardStats _prevStats;
    private GenericDictionary<StatusEffect.Name, int> _prevStatusEffects;
    private Card _card;
    public ResetCardEffect(Card a_card)
    {
        _card = a_card;
        _prevStats = new CardStats(a_card.stats);
        _prevStatusEffects = _card.statusEffects;
    }
    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        _card.Initialize();
    }
    public override void Show(CardGame a_game)
    {
    }

    public override void Undo(CardGame a_game)
    {
        _card.stats = _prevStats;
        foreach (StatusEffect.Name status in _prevStatusEffects.Keys)
        {
            _card.AddStatusEffect(status, _prevStatusEffects[status]);
        }
    }
}
