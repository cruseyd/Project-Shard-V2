using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStatModifier : ActorModifier
{
    public readonly Actor.StatName stat;
    public readonly int value;

    public ActorStatModifier(Actor.StatName a_stat, int a_value, CardGame a_game, ISource a_source, Actor a_target, Duration a_duration)
    : base(a_game, a_source, a_target, a_duration)
    {
        stat = a_stat;
        value = a_value;
    }
}
