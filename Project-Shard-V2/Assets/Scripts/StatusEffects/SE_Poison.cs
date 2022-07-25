using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE_Poison : StatusEffect
{
    public override bool stackable { get { return true; } }
    public override Name name { get { return Name.POISON; } }
    private IDamageable damageable { get { return target as IDamageable; } }
    public SE_Poison(CardGame a_game, ITarget a_target) : base(a_game, a_target)
    {
        Debug.Assert(a_target is IDamageable);
    }

    public override void Activate()
    {
        damageable.owner.events.onStartTurn += OnStartTurn;
    }

    public override void Deactivate()
    {
        damageable.owner.events.onStartTurn -= OnStartTurn;
    }

    private void OnStartTurn(Actor a_actor)
    {
        DamageData data = new DamageData(1, this, damageable);
        DamageTarget(data);
        RemoveStatusEffect(damageable, name, 1);
    }
}
