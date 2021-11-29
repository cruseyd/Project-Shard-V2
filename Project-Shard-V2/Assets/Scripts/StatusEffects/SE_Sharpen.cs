using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE_Sharpen : StatusEffect
{
    public override bool stackable { get { return true; } }
    public override Name name { get { return Name.Sharpen; } }
    public SE_Sharpen(CardGame a_game, ITarget a_target) : base(a_game, a_target)
    {

    }

    public override void Activate()
    {
        target.owner.events.onBeforeDealDamage += ModifyDamage;
        target.owner.events.onEndTurn += EndTurn;
    }

    public override void Deactivate()
    {
        target.owner.events.onBeforeDealDamage -= ModifyDamage;
        target.owner.events.onEndTurn -= EndTurn;
    }

    private void ModifyDamage(Actor a_actor, DamageData a_data)
    {
        if (a_data.source is Card && ((Card)a_data.source).HasKeyword(Keyword.SLASHING))
        {
            a_data.damage += stacks;
        }
    }

    private void EndTurn(Actor a_actor)
    {
        RemoveStatusEffect(target, Name.Sharpen, 9999);
    }
}
