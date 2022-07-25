using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KA_Armor : KeyAbility
{

    public KA_Armor(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        Debug.Assert(a_card is IDamageable);
    }
    private void ModifyDamage(IDamageable a_target, DamageData a_data)
    {
        a_data.damage--;
    }

    public override void Enable()
    {
        base.Enable();
        ((IDamageable)_source).damageEvents.onBeforeTakeDamage += ModifyDamage;
    }
    public override void Disable()
    {
        base.Disable();
        ((IDamageable)_source).damageEvents.onBeforeTakeDamage -= ModifyDamage;
    }
}
public class KA_Guardian : KeyAbility
{

    public KA_Guardian(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        Debug.Assert(a_card is IDamageable);
    }
    public override void Enable()
    {
        base.Enable();
        _game.events.onModifyDefenders += ModifyDefendersHandler;
    }
    private void ModifyDefendersHandler(Card a_attacker, Actor a_target, List<IDamageable> a_defenders)
    {
        if (_source.isInPlay && a_target == _source.owner)
        {
            for (int ii = a_defenders.Count-1; ii >= 0; ii--)
            {
                Debug.Assert(a_defenders[ii] is Card);
                Card d = a_defenders[ii] as Card;
                if (!d.HasKeyword(AbilityKeyword.GUARDIAN))
                {
                    a_defenders.RemoveAt(ii);
                    Debug.Log("Removing devender " + d.data.name + " which does not have GUARDIAN");
                }
            }
        }
    }

    public override void Disable()
    {
        base.Disable();
        _game.events.onModifyDefenders -= ModifyDefendersHandler;
    }
}

public class KA_Evasive : KeyAbility
{

    public KA_Evasive(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        Debug.Assert(a_card is IDamageable);
    }
    public override void Enable()
    {
        base.Enable();
        _game.events.onModifyDefenders += ModifyDefendersHandler;
    }
    private void ModifyDefendersHandler(Card a_attacker, Actor a_target, List<IDamageable> a_defenders)
    {
        if (_source.isInPlay && a_target == _source.owner)
        {
            for (int ii = a_defenders.Count - 1; ii >= 0; ii--)
            {
                Debug.Assert(a_defenders[ii] is Card);
                Card d = a_defenders[ii] as Card;
                if (!d.HasKeyword(AbilityKeyword.EVASIVE))
                {
                    a_defenders.Remove((IDamageable)_source);
                    break;
                }
            }
        }
    }
    public override void Disable()
    {
        base.Disable();
        _game.events.onModifyDefenders -= ModifyDefendersHandler;
    }
}
public class KA_Nimble : KeyAbility
{

    public KA_Nimble(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        Debug.Assert(a_card is IDamageable);
    }
    public override void Enable()
    {
        base.Enable();
        _game.events.onOverrideDefenders += OverrideDefendersHandler;
        _game.events.onBypassDefenders += BypassDefendersHandler;
    }
    private void OverrideDefendersHandler(Card a_attacker, Actor a_target, List<IDamageable> a_defenders)
    {
        if (a_attacker == _source)
        {
            a_defenders.Clear();
            a_defenders.AddRange(a_target.units);
        }
    }
    private void BypassDefendersHandler(Card a_attacker, Actor a_target, List<IDamageable> a_defenders)
    {
        if (a_attacker == _source)
        {
            bool nimbleBlocker = false;
            foreach (IDamageable d in a_defenders)
            {
                if (d is Card && ((Card)d).HasKeyword(AbilityKeyword.NIMBLE))
                {
                    nimbleBlocker = true;
                }
            }
            if (!nimbleBlocker && !a_defenders.Contains(a_target))
            {
                a_defenders.Add(a_target);
            }
        }
    }

    public override void Disable()
    {
        base.Disable();
        _game.events.onOverrideDefenders -= OverrideDefendersHandler;
        _game.events.onBypassDefenders -= BypassDefendersHandler;
    }
}
public class KA_Overwhelm : KeyAbility
{

    public KA_Overwhelm(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        Debug.Assert(a_card is IDamageable);
    }
    public override void Enable()
    {
        base.Enable();
        _source.sourceEvents.onDealOverkillDamage += OverkillDamageHandler;
    }

    private void OverkillDamageHandler(ISource a_source, DamageData a_damageData)
    {
        DamageData overkillData = new DamageData(a_damageData.damage, a_damageData.source, a_damageData.target.owner);
        DamageTarget(overkillData);
    }

    public override void Disable()
    {
        base.Disable();
        _source.sourceEvents.onDealOverkillDamage -= OverkillDamageHandler;
    }
}
public class KA_Rush : KeyAbility
{

    public KA_Rush(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        Debug.Assert(a_card is UnitCard);
    }
    public override void Enable()
    {
        base.Enable();
        ((UnitCard)_source).damageEvents.onBeforeTakeCounterAttackDamage += CounterAttackHandler;
    }

    private void CounterAttackHandler(IDamageable a_target, DamageData a_data)
    {
        Debug.Assert(a_data.source is UnitCard);
        if (((UnitCard)a_data.source).health <= 0)
        {
            a_data.damage = 0;
        }
    }

    public override void Disable()
    {
        base.Disable();
        ((UnitCard)_source).damageEvents.onBeforeTakeCounterAttackDamage -= CounterAttackHandler;
    }
}
public class KA_Swift : KeyAbility
{

    public KA_Swift(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        Debug.Log("trying to give Swift to " + a_card.data.name);
        Debug.Assert(a_card is UnitCard);
    }
    public override void Enable()
    {
        base.Enable();
        ((UnitCard)_source).events.onEnterPlay += EnterPlayHandler;
    }

    private void EnterPlayHandler(Card a_card)
    {
        a_card.numActions = 1;
    }

    public override void Disable()
    {
        base.Disable();
        ((UnitCard)_source).events.onEnterPlay -= EnterPlayHandler;
    }
}