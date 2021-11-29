using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResetCardEffect : GameEffect
{
    private GenericDictionary<Card.StatName, int> _prevStats;
    private GenericDictionary<StatusEffect.Name, int> _prevStatusEffects;
    private List<CardModifier> _prevModifiers;
    private Card _card;
    public ResetCardEffect(Card a_card)
    {
        _card = a_card;
        _prevStats = _card.stats;
        _prevStatusEffects = _card.statusEffects;
        _prevModifiers = _card.modifiers;
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
        foreach (Card.StatName stat in _prevStats.Keys)
        {
            _card.SetStat(stat, _prevStats[stat]);
        }
        foreach (StatusEffect.Name status in _prevStatusEffects.Keys)
        {
            _card.AddStatusEffect(status, _prevStatusEffects[status]);
        }
        foreach (CardModifier mod in _prevModifiers)
        {
            _card.AddModifier(mod);
        }
    }
}
