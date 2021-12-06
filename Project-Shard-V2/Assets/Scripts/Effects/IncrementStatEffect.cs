using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementActorStatEffect : GameEffect
{
    private Actor.StatName _stat;
    private int _value;
    private int _prevValue;
    private Actor _actor;

    public IncrementActorStatEffect(Actor.StatName a_stat, int a_value, Actor a_actor)
    {
        _stat = a_stat;
        _value = a_value;
        _actor = a_actor;
        _prevValue = a_actor.GetStat(a_stat);
    }

    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        _actor.IncrementStat(_stat, _value);
    }

    public override void Show(CardGame a_game)
    {
        
    }

    public override void Undo(CardGame a_game)
    {
        _actor.SetStat(_stat, _prevValue);
    }
}
public class IncrementCardStatEffect : GameEffect
{
    private CardStats.Name _stat;
    private int _value;
    private int _prevValue;
    private Card _card;

    public IncrementCardStatEffect(CardStats.Name a_stat, int a_value, Card a_card)
    {
        _stat = a_stat;
        _value = a_value;
        _card = a_card;
        _prevValue = a_card.stats.Get(a_stat);
    }

    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        _card.stats.Increment(_stat, _value);
    }

    public override void Show(CardGame a_game)
    {

    }

    public override void Undo(CardGame a_game)
    {
        _card.stats.Set(_stat, _prevValue);
    }
}
