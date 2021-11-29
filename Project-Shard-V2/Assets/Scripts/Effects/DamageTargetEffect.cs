using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTargetEffect : GameEffect
{
    private DamageData _data;
    private int _prevHealth;
    public DamageTargetEffect(DamageData a_data)
    {
        _data = a_data;
        _prevHealth = a_data.target.health;
    }
    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        _data.target.TakeDamage(_data);
        if (_data.target.health <= 0)
        {
            DeathEffect death = new DeathEffect(_data.target, _data);
            death.Execute(a_game);
        }
    }

    public override void Show(CardGame a_game)
    {
    }

    public override void Undo(CardGame a_game)
    {
        _data.target.SetHealth(_prevHealth);
    }
}
