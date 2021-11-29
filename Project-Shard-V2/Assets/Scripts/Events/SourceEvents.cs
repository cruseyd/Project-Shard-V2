using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SourceEvents
{
    private ISource _source;

    public event Action<ISource, DamageData> onDealDamage;
    public event Action<ISource, DamageData> onHeal;
    public event Action<ISource> onActivate;
    public event Action<ISource> onDeactivate;

    public SourceEvents(ISource a_source) { _source = a_source; }

    public void DealDamage(DamageData a_data) { onDealDamage?.Invoke(_source, a_data); }
    public void Heal(DamageData a_data) { onHeal?.Invoke(_source, a_data); }
    public void Activate() { onActivate?.Invoke(_source); }
    public void Deactivate() { onDeactivate?.Invoke(_source); }
    public void Activate(ISource a_source) { onActivate?.Invoke(_source); } //overload for matching delagate signatures
    public void Deactivate(ISource a_source) { onDeactivate?.Invoke(_source); } //overload for matching delagate signatures
}
