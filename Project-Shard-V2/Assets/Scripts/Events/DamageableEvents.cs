using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DamageableEvents
{
    private IDamageable _target;

    public event Action<IDamageable, DamageData> onBeforeTakeDamage;
    public event Action<IDamageable, DamageData> onTakeDamage;
    public event Action<IDamageable, DamageData> onTakeOverkillDamage;
    public event Action<IDamageable, DamageData> onBeforeTakeCounterAttackDamage;
    public event Action<IDamageable, DamageData> onTakeCounterAttackDamage;
    public event Action<IDamageable, DamageData> onDeath;
    public event Action<IDamageable, DamageData> onHeal;

    public DamageableEvents(IDamageable a_target) { _target = a_target; }

    public void TakeDamage(DamageData a_data) { onTakeDamage?.Invoke(_target, a_data); }
    public void TakeOverkillDamage(DamageData a_data) { onTakeOverkillDamage?.Invoke(_target, a_data); }
    public void TakeCounterAttackDamage(DamageData a_data) { onTakeCounterAttackDamage?.Invoke(_target, a_data); }
    public void BeforeTakeCounterAttackDamage(DamageData a_data) { onBeforeTakeCounterAttackDamage?.Invoke(_target, a_data); }
    public void Heal(DamageData a_data) { onHeal?.Invoke(_target, a_data); }
    public void Death(DamageData a_data) { onDeath?.Invoke(_target, a_data); }
    public void BeforeTakeDamage(DamageData a_data) { onBeforeTakeDamage?.Invoke(_target, a_data); }
}
