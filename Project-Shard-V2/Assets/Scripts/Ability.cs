using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability
{
    protected CardGame _game;
    public List<TargetQuery> targets { get; protected set; }
    public virtual int minTargets { get { return maxTargets; } }
    public int maxTargets { get { return targets.Count; } }

    public Ability(CardGame a_game)
    {
        _game = a_game;
        targets = new List<TargetQuery>();
    }
    protected void DrawCards(Actor a_actor, int a_n)
    {
        for (int ii = 0; ii < a_n; ii++)
        {
            DrawCardEffect effect = new DrawCardEffect(a_actor);
            effect.Execute(_game);
        }
    }
    public void DestroyCard(Card a_target)
    {
        EntersZoneEffect effect = new EntersZoneEffect(a_target, a_target.owner.discard, true);
        effect.Execute(_game);
    }
    protected void DamageTarget(DamageData a_data)
    {
        DamageTargetEffect effect = new DamageTargetEffect(a_data);
        effect.Execute(_game);
    }

    protected void AddFocus(Actor a_actor, int a_temp, int a_max)
    {
        if (a_temp != 0)
        {
            IncrementActorStatEffect effect = new IncrementActorStatEffect(Actor.StatName.FOCUS, a_temp, a_actor);
            effect.Execute(_game);
        }
        if (a_max != 0)
        {
            IncrementActorStatEffect effect = new IncrementActorStatEffect(Actor.StatName.MAX_FOCUS, a_max, a_actor);
            effect.Execute(_game);
        }
    }
    protected void AddModifier(Modifier a_mod)
    {
        AddModifierEffect effect = new AddModifierEffect(a_mod);
        effect.Execute(_game);
    }
    protected void RemoveModifier(Modifier a_mod)
    {
        RemoveModifierEffect effect = new RemoveModifierEffect(a_mod);
        effect.Execute(_game);
    }
    protected void AddStatusEffect(ITarget a_target, StatusEffect.Name a_status, int a_stacks)
    {
        AddStatusEffect effect = new AddStatusEffect(a_status, a_target, a_stacks);
        effect.Execute(_game);
    }
    protected void RemoveStatusEffect(ITarget a_target, StatusEffect.Name a_status, int a_stacks)
    {
        RemoveStatusEffect effect = new RemoveStatusEffect(a_status, a_target, a_stacks);
        effect.Execute(_game);
    }
    protected void ChangeAOEStatModifier(AOECardStatModifier a_mod, int a_value)
    {
        ChangeAOECardStatModifierEffect effect = new ChangeAOECardStatModifierEffect(a_mod, a_value);
        effect.Execute(_game);
    }
    protected void ChangeStatModifier(CardStatModifier a_mod, int a_value)
    {
        ChangeCardStatModifierEffect effect = new ChangeCardStatModifierEffect(a_mod, a_value);
        effect.Execute(_game);
    }
    protected void AddToHand(Card a_card)
    {
        EntersZoneEffect effect = new EntersZoneEffect(a_card, a_card.owner.hand, true);
        effect.Execute(_game);
    }
    protected ITarget GetRandomTarget(TargetQuery a_query, Actor a_pov)
    {
        List<ITarget> targets = _game.VisibleTargets(a_pov);
        bool done = false;
        while (!done && targets.Count > 0)
        {
            int roll = Random.Range(0, targets.Count);
            ITarget t = targets[roll];
            if (a_query.Compare(t)) { return t; }
            else { targets.Remove(t); }
        }
        return null;
    }
}
