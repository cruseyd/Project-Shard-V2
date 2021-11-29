using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : GameEffect
{
    private DamageData _fatalDamage;
    private IDamageable _target;

    public DeathEffect(IDamageable a_target, DamageData a_damage)
    {
        _fatalDamage = a_damage;
        _target = a_target;
    }
    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        _target.damageEvents.Death(_fatalDamage);
        if (_target is Card)
        {
            Card card = _target as Card;
            EntersZoneEffect moveEffect = new EntersZoneEffect(card, card.owner.discard, true);
            moveEffect.Execute(a_game);
        }
    }
    public override void Show(CardGame a_game)
    {
        
    }

    public override void Undo(CardGame a_game)
    {

    }
}
