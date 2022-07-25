using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE_Acumen : StatusEffect
{
    public override bool stackable { get { return true; } }
    public override Name name { get { return Name.ACUMEN; } }
    public SE_Acumen(CardGame a_game, ITarget a_target) : base(a_game, a_target)
    {
        Debug.Assert(a_target is Actor);
    }

    public override void Activate()
    {
        target.owner.events.onStartTurn += OnStartTurn;
    }

    public override void Deactivate()
    {
        target.owner.events.onStartTurn -= OnStartTurn;
    }

    private void OnStartTurn(Actor a_actor)
    {
        AddFocus((Actor)target, stacks, 0);
        RemoveStatusEffect(target, name, 9999);
    }
}
