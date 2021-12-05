using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class CardAbility : Ability
{
    public static CardAbility Get(CardGame a_game, string a_id, Card a_card)
    {
        
        string typeName = "CA_" + a_id;
        Type cardAbilityType = Type.GetType(typeName);
        if (cardAbilityType == null)
        {
            return null;
        }
        ConstructorInfo constructorInfo = cardAbilityType.GetConstructor(new[] { typeof(CardGame), typeof(Card) });
        object[] args = { a_game, a_card };
        CardAbility ability = constructorInfo.Invoke(args) as CardAbility;

        return ability;
    }
    public List<TargetQuery> aTargets { get; protected set; }
    public virtual int aMinTargets { get { return maxTargets; } }
    public int aMaxTargets { get { return aTargets.Count; } }
    public virtual bool canActivate { get { return false; } }

    protected Card _source;
    public CardAbility(CardGame a_game, Card a_card) : base(a_game)
    {
        aTargets = new List<TargetQuery>();
        _source = a_card;
    }

    public virtual void Play(List<ITarget> a_targets) { }
    public virtual void Activate(List<ITarget> a_targets) { }
}
