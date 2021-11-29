using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility : Ability
{
    public static CardAbility Get(CardGame a_game, string a_id, Card a_card)
    {
        switch (a_id)
        {
            case "BUFF_UNIT": return new CA_BuffUnit(a_game, a_card);
            case "HEAL_UNIT": return new CA_HealUnit(a_game, a_card);

            case "KILL_SPELL": return new CA_KillSpell(a_game, a_card);
            case "DAMAGE_SPELL": return new CA_DamageSpell(a_game, a_card);
            case "DRAW_SPELL": return new CA_DrawSpell(a_game, a_card);
            case "BUFF_SPELL": return new CA_BuffSpell(a_game, a_card);
            case "POISON_SPELL": return new CA_PoisonSpell(a_game, a_card);

            // Set 1 - Raiz
            case "SLASH": return new CA_Slash(a_game, a_card);
            case "WILD_SWING": return new CA_WildSwing(a_game, a_card);
            case "SKY_SUNDER": return new CA_SkySunder(a_game, a_card);
            case "FLURRY": return new CA_Flurry(a_game, a_card);
            case "SEVER": return new CA_Sever(a_game, a_card);
            case "BLITZ": return new CA_Blitz(a_game, a_card);
            case "SHARPEN": return new CA_Sharpen(a_game, a_card);
            default:
                return null;
        }
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
