using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombatEffect : GameEffect
{
    private UnitCard _attacker;
    private UnitCard _defender;
    private int _attackerPrevHealth;
    private int _defenderPrevHealth;

    public UnitCombatEffect(UnitCard a_attacker, UnitCard a_defender)
    {
        _attacker = a_attacker;
        _defender = a_defender;
        _attackerPrevHealth = a_attacker.health;
        _defenderPrevHealth = a_defender.health;
    }
    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        DamageData attackerDamage = new DamageData(_attacker.power, _attacker, _defender);
        int defenderHealth = _defender.stats.Get(CardStats.Name.HEALTH);
        _defender.TakeDamage(attackerDamage);
        // note that the actual damage might be altered inside of TakeDamage
        int damageDealt = attackerDamage.damage;
        int overkillDamage = damageDealt - defenderHealth;
        if (_attacker.HasKeyword(AbilityKeyword.OVERWHELM) && overkillDamage > 0)
        {
            DamageData overwhelmDamage = new DamageData(overkillDamage, _attacker, _defender.owner);
            DamageTargetEffect overwhelmEffect = new DamageTargetEffect(overwhelmDamage);
            overwhelmEffect.Execute(a_game);
        }
        if (_attacker.HasKeyword(AbilityKeyword.RUSH))
        {
            if (_defender.health <= 0)
            {
                DeathEffect death = new DeathEffect(_defender, attackerDamage);
                death.Execute(a_game);
                return;
            }
            DamageData defenderDamage = new DamageData(_defender.power, _defender, _attacker);
            _attacker.TakeDamage(defenderDamage);
            if (_attacker.health <= 0)
            {
                DeathEffect death = new DeathEffect(_attacker, defenderDamage);
                death.Execute(a_game);
            }
        } else {
            DamageData defenderDamage = new DamageData(_defender.power, _defender, _attacker);
            _attacker.TakeDamage(defenderDamage);
            if (_defender.health <= 0)
            {
                DeathEffect death = new DeathEffect(_defender, attackerDamage);
                death.Execute(a_game);
            }
            if (_attacker.health <= 0)
            {
                DeathEffect death = new DeathEffect(_attacker, defenderDamage);
                death.Execute(a_game);
            }
        }
        
    }
    public override void Show(CardGame a_game)
    {
    }

    public override void Undo(CardGame a_game)
    {
        _attacker.SetHealth(_attackerPrevHealth);
        _defender.SetHealth(_defenderPrevHealth);
    }
}
