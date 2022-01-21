using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE_Frenzy : StatusEffect
{
    public override bool stackable { get { return true; } }
    public override Name name { get { return Name.Frenzy; } }
    public SE_Frenzy(CardGame a_game, ITarget a_target) : base(a_game, a_target)
    {
        Debug.Assert(a_target is Actor);
    }

    public override void Activate()
    {
        ((Actor)target).events.onPlayCard += OnPlayCard;
    }

    public override void Deactivate()
    {
        ((Actor)target).events.onPlayCard -= OnPlayCard;
    }

    private void OnPlayCard(Actor a_actor, Card a_card)
    {
        if (a_card.data.type == Card.Type.ACTION)
        {
            DamageData damage = new DamageData(1, this, (IDamageable)target);
            DamageTarget(damage);
            AddFocus((Actor)target, 1, 0);
            RemoveStatusEffect(target, Name.Frenzy, 1);
        }
    }
}
