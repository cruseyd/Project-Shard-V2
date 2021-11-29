using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardModifier : Modifier
{
    new public readonly Card target;

    public CardModifier(CardGame a_game, ISource a_source, Card a_target, Duration a_duration)
    : base(a_game, a_source, a_target, a_duration)
    {
        target = a_target;
    }

    public override void Activate()
    {
        base.Activate();
        if (duration == Duration.TARGET_LEAVE_PLAY)
        {
            target.events.onLeavePlay += OnTargetLeavesPlay;
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        if (duration == Duration.TARGET_LEAVE_PLAY)
        {
            target.events.onLeavePlay -= OnTargetLeavesPlay;
        }
    }

    private void OnTargetLeavesPlay(Card a_target)
    {
        RemoveModifierEffect effect = new RemoveModifierEffect(this);
        effect.Execute(_game);
        a_target.events.onLeavePlay -= OnTargetLeavesPlay;
    }
}
