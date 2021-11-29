using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorModifier : Modifier
{
    new public readonly Actor target;

    public ActorModifier(CardGame a_game, ISource a_source, Actor a_target, Duration a_duration)
    : base(a_game, a_source, a_target, a_duration)
    {
        target = a_target;
    }
}
