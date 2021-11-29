using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE_Acumen : StatusEffect
{
    public override bool stackable { get { return true; } }
    public override Name name { get { return Name.Acumen; } }
    public SE_Acumen(CardGame a_game, ITarget a_target) : base(a_game, a_target)
    {
        Debug.Assert(a_target is IDamageable);
    }

    public override void Activate()
    {
    }

    public override void Deactivate()
    {
    }

    private void OnStartTurn(Actor a_actor)
    {
    }
}
