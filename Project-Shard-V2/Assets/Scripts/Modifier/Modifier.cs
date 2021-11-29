using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Modifier
{
    public enum Duration
    {
        TARGET_LEAVE_PLAY,
        SOURCE,
        PERMANENT
    }

    protected CardGame _game;

    public readonly ISource source;
    public readonly IModifiable target;
    public readonly Duration duration;

    public Modifier(CardGame a_game, ISource a_source, IModifiable a_target, Duration a_duration)
    {
        _game = a_game;
        source = a_source;
        target = a_target;
        duration = a_duration;
    }

    public virtual void Activate()
    {
        if (duration == Duration.SOURCE)
        {
            Debug.Assert(source != null);
            source.sourceEvents.onDeactivate += OnSourceDeactivate;
        }
    }

    public virtual void Deactivate()
    {
        if (duration == Duration.SOURCE)
        {
            Debug.Assert(source != null);
            source.sourceEvents.onDeactivate -= OnSourceDeactivate;
        }
    }

    public virtual void OnSourceDeactivate(ISource a_source)
    {
        RemoveModifierEffect effect = new RemoveModifierEffect(this);
        effect.Execute(_game);
        a_source.sourceEvents.onDeactivate -= OnSourceDeactivate;
    }
}
