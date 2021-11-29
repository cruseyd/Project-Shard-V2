using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget
{
    public List<Actor> opponents { get; }
    public Actor owner { get; }
    public GameObject obj { get; }
    public void MarkChosenTarget();
    public void MarkValidTarget();
    public void ClearMarks();

    public void AddStatusEffect(StatusEffect.Name a_name, int a_stacks);
    public void RemoveStatusEffect(StatusEffect.Name a_name, int a_stacks);
    public int GetStatusEffect(StatusEffect.Name a_name);
    public void RemoveAllStatusEffects();
}
