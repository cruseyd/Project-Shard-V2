using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class KeyAbility : Ability
{
    public static KeyAbility Get(CardGame a_game, AbilityKeyword a_key, Card a_card)
    {
        string typeName = "KA_" + TextUtils.AllCapsToPascal(a_key.ToString());
        Type keyAbilityType = Type.GetType(typeName);
        if (keyAbilityType == null)
        {
            return null;
        }
        ConstructorInfo constructorInfo = keyAbilityType.GetConstructor(new[] { typeof(CardGame), typeof(Card) });
        object[] args = { a_game, a_card };
        KeyAbility ability = constructorInfo.Invoke(args) as KeyAbility;

        return ability;

        /*
        switch (a_key)
        {
            case AbilityKeyword.SWIFT: return new KA_Swift(a_game, a_card);
            default:
                return null;
        }
        */
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
