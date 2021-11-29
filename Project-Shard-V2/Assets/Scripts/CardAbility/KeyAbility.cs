using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyAbility : Ability
{
    public static KeyAbility Get(CardGame a_game, AbilityKeyword a_key, Card a_card)
    {
        switch (a_key)
        {
            case AbilityKeyword.SWIFT: return new KA_Swift(a_game, a_card);
            default:
                return null;
        }
    }
    public List<TargetQuery> aTargets { get; protected set; }
    public virtual int aMinTargets { get { return maxTargets; } }
    public int aMaxTargets { get { return aTargets.Count; } }
    public virtual bool canActivate { get { return false; } }

    protected Card _source;
    public KeyAbility(CardGame a_game, Card a_card) : base(a_game)
    {
        aTargets = new List<TargetQuery>();
        _source = a_card;
    }

    public virtual void Play(List<ITarget> a_targets) { }

    public virtual void Activate(List<ITarget> a_targets) { }
}
