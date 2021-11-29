using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeclareAttack : GameAction
{
    private UnitCard _attacker;
    private IDamageable _defender;
    public override Actor actor { get { return _attacker.owner; } }
    public DeclareAttack(UnitCard a_attacker, IDamageable a_defender)
    {
        _attacker = a_attacker;
        _defender = a_defender;
    }

    public override void Execute(CardGame a_game)
    {
        _attacker.events.DeclareAttack(_defender);
        if (_defender is UnitCard)
        {
            UnitCombatEffect combatEffect = new UnitCombatEffect(_attacker, (UnitCard)_defender);
            combatEffect.Execute(a_game);
            ((UnitCard)_defender).events.Defend(_attacker);
        } else
        {
            DamageData attackerData = new DamageData(_attacker.power, _attacker, _defender);
            DamageTargetEffect attackEffect = new DamageTargetEffect(attackerData);
            attackEffect.Execute(a_game);
        }

        _attacker.events.Attack(_defender);
        _attacker.numActions--;
    }

    public override bool IsValid(CardGame a_game)
    {
        return _attacker.canAttack;
    }

    public override void Preview(CardGame a_game)
    {
        AnimationQueue.Add(new ShowTargetAnim(a_game, _attacker.ui.transform, _defender.targetUI.gameObject.transform));
    }

    public override void Undo(CardGame a_game)
    {
        base.Undo(a_game);
        _attacker.numActions++;
    }
}
