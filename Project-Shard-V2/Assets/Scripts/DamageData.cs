using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData
{
    private int _damage;
    public int damage
    {
        get { return _damage; }
        set
        {
            if (locked)
            {
                Debug.LogError("Tried to edit DamageData after locking");
            }
            _damage = value;
        }
    }
    public ISource source { get; private set; }
    public IDamageable target { get; private set; }

    public bool locked;
    public DamageData(int a_damage, ISource a_source, IDamageable a_target)
    {
        damage = a_damage;
        source = a_source;
        target = a_target;
        locked = false;
    }
}
