using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CA_Slash : CardAbility
{
    public CA_Slash(CardGame a_game, Card a_card) : base(a_game, a_card)
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

public class CA_WildSwing : CardAbility
{
    public CA_WildSwing(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        TargetQuery query = new TargetQuery(a_card);
        query.isEnemy = true;
        query.isDamageable = true;
        targets.Add(query);
    }

    public override void Play(List<ITarget> a_targets)
    {
        DamageData dataMain = new DamageData(_source.data.var1, _source, (IDamageable)a_targets[0]);
        DamageTarget(dataMain);

        TargetQuery query = new TargetQuery(_source);
        query.isEnemy = true;
        query.isDamageable = true;
        ITarget splashTarget = GetRandomTarget(query, _source.owner);
        if (splashTarget != null)
        {
            DamageData dataSplash = new DamageData(_source.data.var2, _source, (IDamageable)splashTarget);
            DamageTarget(dataSplash);
        }
    }
}

public class CA_SkySunder : CardAbility
{
    public CA_SkySunder(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        TargetQuery query = new TargetQuery(a_card);
        query.isEnemy = true;
        query.cardTypes.Add(Card.Type.FOLLOWER);
        targets.Add(query);
    }

    public override void Play(List<ITarget> a_targets)
    {
        DamageData data = new DamageData(_source.data.var1, _source, (IDamageable)a_targets[0]);
        DamageTarget(data);
        foreach (Card card in a_targets[0].owner.active.cards)
        {
            if (card is UnitCard)
            {
                DamageData d = new DamageData(_source.data.var2, _source, (IDamageable)card);
                DamageTarget(d);
            }
        }
    }
}

public class CA_Flurry : CardAbility
{
    public CA_Flurry(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        TargetQuery query = new TargetQuery(a_card);
        query.isEnemy = true;
        query.isDamageable = true;
        targets.Add(query);
        _source.owner.events.onPlayCard += PlayCardHandler;
    }

    public override void Play(List<ITarget> a_targets)
    {
        DamageData data = new DamageData(_source.data.var1, _source, (IDamageable)a_targets[0]);
        DamageTarget(data);
    }

    public void PlayCardHandler(Actor a_actor, Card a_card)
    {
        if (a_card == _source) { return; }
        if (a_card.HasKeyword(Keyword.T_TECHNIQUE) && _source.playedThisTurn && _source.zone.type == CardZone.Type.DISCARD)
        {
            AddToHand(_source);
        }
    }
}

public class CA_Sever : CardAbility
{
    public CA_Sever(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        TargetQuery query = new TargetQuery(a_card);
        query.cardTypes.Add(Card.Type.FOLLOWER);
        query.isEnemy = true;
        targets.Add(query);
    }

    public override void Play(List<ITarget> a_targets)
    {
        int damage = _source.owner.GetStat(Actor.StatName.THRESHOLD_RED);
        DamageData data = new DamageData(damage, _source, (IDamageable)a_targets[0]);
        DamageTarget(data);
    }
}

public class CA_Blitz : CardAbility
{
    public CA_Blitz(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        _source.events.onCycle += CycleHandler;
    }

    public override void Play(List<ITarget> a_targets)
    {
        base.Play(a_targets);
        DrawCards(_source.owner, 1);
        int focusGain = 0;
        List<Card> handCards = _source.owner.hand.cards;
        foreach (Card card in handCards)
        {
            if (card.HasKeyword(Keyword.T_TECHNIQUE))
            {
                focusGain++;
            }
        }
        AddFocus(_source.owner, focusGain, 0);
    }

    private void CycleHandler(Card a_card)
    {
        AddStatusEffect(_source.owner, StatusEffect.Name.Acumen, _source.data.var2);
    }
}

public class CA_Sharpen : CardAbility
{
    public CA_Sharpen(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
    }

    public override void Play(List<ITarget> a_targets)
    {
        base.Play(a_targets);
        AddStatusEffect(_source.owner, StatusEffect.Name.Sharpen, 1);
    }
}

public class CA_Relentless : CardAbility
{
    public CA_Relentless(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        TargetQuery query = new TargetQuery(a_card);
        query.isEnemy = true;
        query.isDamageable = true;
        targets.Add(query);
    }

    public override void Play(List<ITarget> a_targets)
    {
        base.Play(a_targets);
        DamageData data = new DamageData(_source.data.var1, _source, (IDamageable)a_targets[0]);
        DamageTarget(data);
        List<Card> handCards = _source.owner.hand.cards;
        List<Card> actions = new List<Card>();
        foreach (Card card in handCards)
        {
            if (card.data.type == Card.Type.ACTION)
            {
                actions.Add(card);
            }
        }
        int roll = Random.Range(0, actions.Count);
        AddStatusEffect(actions[roll], StatusEffect.Name.Alacrity, _source.data.var2);
    }
}

public class CA_SeeingRed : CardAbility
{
    public CA_SeeingRed(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
    }

    public override void Play(List<ITarget> a_targets)
    {
        base.Play(a_targets);
        AddStatusEffect(_source.owner, StatusEffect.Name.Frenzy, _source.data.var1);
        DrawCards(_source.owner, 1);
    }
}

public class CA_IntotheFray : CardAbility
{
    public CA_IntotheFray(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        _source.events.onCycle += CycleHandler;
    }

    public override void Play(List<ITarget> a_targets)
    {
        base.Play(a_targets);
        AddStatusEffect(_source.owner, StatusEffect.Name.Frenzy, _source.data.var1);
    }

    private void CycleHandler(Card a_card)
    {
        AddStatusEffect(_source.owner, StatusEffect.Name.Frenzy, _source.data.var2);
    }
}

public class CA_FeralThrash : CardAbility
{

    private CardStatModifier _mod;
    public CA_FeralThrash(CardGame a_game, Card a_card) : base(a_game, a_card)
    {
        TargetQuery query = new TargetQuery(a_card);
        query.isEnemy = true;
        query.isDamageable = true;
        targets.Add(query);
        _mod = new CardStatModifier(CardStats.Name.LEVEL, 0, a_game, a_card, a_card, Modifier.Duration.PERMANENT);
        a_game.events.onRefresh += OnRefresh;
    }

    public override void Play(List<ITarget> a_targets)
    {
        base.Play(a_targets);
        DamageData data = new DamageData(_source.data.var1, _source, (IDamageable)a_targets[0]);
        DamageTarget(data);
    }

    private void OnRefresh()
    {
        if (_source.isInHand)
        {
            _source.AddModifier(_mod); //adding modifier manually;
            int numFrenzy = _source.owner.GetStatusEffect(StatusEffect.Name.Frenzy);
            int discount = numFrenzy * _source.data.var2;
            Debug.Log("Feral Thrash Discount: " + discount);
            ChangeStatModifier(_mod, -discount);
        }
    }
}