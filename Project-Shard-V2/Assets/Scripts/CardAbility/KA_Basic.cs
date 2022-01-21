using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KA_Ward : KeyAbility
{
    public KA_Ward(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        Debug.Assert(a_card is IDamageable);
        ((IDamageable)a_card).damageEvents.onBeforeTakeDamage += ModifyDamage;
    }

    private void ModifyDamage(IDamageable a_target, DamageData a_data)
    {
        if (a_data.source != null)
        {
            if (a_data.source is SpellCard)
            {
                a_data.damage--;
            }
        }
    }
}

public class KA_Armor : KeyAbility
{
    public KA_Armor(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        Debug.Assert(a_card is IDamageable);
        ((IDamageable)a_card).damageEvents.onBeforeTakeDamage += ModifyDamage;
    }
    private void ModifyDamage(IDamageable a_target, DamageData a_data)
    {
        if (a_data.source != null)
        {
            if (a_data.source is UnitCard)
            {
                a_data.damage--;
            }
        }
    }
}