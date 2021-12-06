using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorCardStatModifier : ActorModifier
{
    private TargetQuery _query;
    public readonly CardStats.Name stat;
    public readonly int value;
    public ActorCardStatModifier(TargetQuery a_query, CardStats.Name a_stat, int a_value,
        CardGame a_game, ISource a_source, Actor a_target, Duration a_duration) : base(a_game, a_source, a_target, a_duration)
    {
        _query = a_query;
        stat = a_stat;
        value = a_value;
    }

    public bool Compare(Card a_card)
    {
        return _query.Compare(a_card);
    }
}
