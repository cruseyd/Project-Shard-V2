using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CA_DrawSpell : CardAbility
{
    public CA_DrawSpell(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
    }

    public override void Play(List<ITarget> a_targets)
    {
        DrawCards(_source.owner, _source.data.var1);
    }
}
public class CA_BuffSpell : CardAbility
{
    public CA_BuffSpell(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        TargetQuery query = new TargetQuery(a_card);
        query.isCard = true;
        query.zones.Add(CardZone.Type.ACTIVE);
        query.isAlly = true;
        targets.Add(query);
    }
    public override void Play(List<ITarget> a_targets)
    {
        CardStatModifier mod = new CardStatModifier(Card.StatName.POWER, _source.data.var1, _game, _source, (Card)a_targets[0], Modifier.Duration.TARGET_LEAVE_PLAY);
        AddModifier(mod);
    }
}
public class CA_BuffUnit : CardAbility
{
    public CA_BuffUnit(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        a_card.events.onLeavePlay += a_card.sourceEvents.Deactivate;
    }

    public override void Play(List<ITarget> a_targets)
    {
        base.Play(a_targets);
        TargetQuery query = new TargetQuery(_source);
        query.isAlly = true;
        query.isCard = true;
        query.zones.Add(CardZone.Type.ACTIVE);
        ActorCardStatModifier mod = new ActorCardStatModifier
            (query, Card.StatName.LEVEL, 1, _game, _source, _source.owner, Modifier.Duration.SOURCE);
        AddModifier(mod);
    }
}
public class CA_DamageSpell : CardAbility
{
    public CA_DamageSpell(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        TargetQuery query = new TargetQuery(a_card);
        query.isEnemy = true;
        query.isDamageable = true;
        targets.Add(query);
    }

    public override void Play(List<ITarget> a_targets)
    {
        DamageData data = new DamageData(_source.data.var1, _source, (IDamageable)a_targets[0]);
        DamageTarget(data);
    }
}
public class CA_HealUnit : CardAbility
{
    public override bool canActivate { get { return _source.isInPlay; } }
    public CA_HealUnit(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        TargetQuery query = new TargetQuery(_source);
        query.isAlly = true;
        query.isDamageable = true;
        aTargets.Add(query);
    }

    public override void Play(List<ITarget> a_targets)
    {
        base.Play(a_targets);
    }
    public override void Activate(List<ITarget> a_targets)
    {
        base.Activate(a_targets);
        DamageData data = new DamageData(-_source.data.var1, _source, (IDamageable)a_targets[0]);
        DamageTarget(data);
    }
}
public class CA_KillSpell : CardAbility
{
    public CA_KillSpell(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        TargetQuery query = new TargetQuery(a_card);
        query.isCard = true;
        query.zones.Add(CardZone.Type.ACTIVE);
        query.isEnemy = true;
        targets.Add(query);
    }

    public override void Play(List<ITarget> a_targets)
    {
        DestroyCard((Card)a_targets[0]);
    }
}
public class CA_PoisonSpell : CardAbility
{
    public CA_PoisonSpell(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        TargetQuery query = new TargetQuery(a_card);
        query.isEnemy = true;
        query.zones.Add(CardZone.Type.ACTIVE);
        targets.Add(query);
    }
    public override void Play(List<ITarget> a_targets)
    {
        AddStatusEffect(a_targets[0], StatusEffect.Name.Poison, _source.data.var1);
    }
}