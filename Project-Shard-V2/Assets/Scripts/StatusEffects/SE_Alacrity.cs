using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SE_Alacrity : StatusEffect
{
    public override bool stackable { get { return true; } }
    public override Name name { get { return Name.Alacrity; } }

    private CardStatModifier _mod;
    public SE_Alacrity(CardGame a_game, ITarget a_target) : base(a_game, a_target)
    {
        Debug.Assert(a_target is Card);
        _mod = new CardStatModifier(CardStats.Name.LEVEL, 0, a_game, null, (Card)a_target, Modifier.Duration.PERMANENT);
    }
    public override void Activate()
    {
        AddModifier(_mod);
        ChangeStatModifier(_mod, -stacks);
        ((Card)target).events.onLeaveHand += OnLeaveHand;
    }

    public override void Deactivate()
    {
        RemoveModifier(_mod);
        ((Card)target).events.onLeaveHand -= OnLeaveHand;
    }

    public void OnLeaveHand(Card a_card)
    {
        RemoveStatusEffect(target, Name.Alacrity, 9999);
    }
}
/*
public class SE_Alacrity : StatusEffect
{
    public override bool stackable { get { return true; } }
    public override Name name { get { return Name.Alacrity; } }

    private TargetQuery _query;
    private AOECardStatModifier _mod;
    public SE_Alacrity(CardGame a_game, ITarget a_target) : base(a_game, a_target)
    {
        Debug.Assert(a_target is Actor);
        _query = new TargetQuery(null);
        _query.isCard = true;
        _query.zones.Add(CardZone.Type.HAND);
        _query.cardTypes.Add(Card.Type.ACTION);
        _mod = new AOECardStatModifier(_query, CardStats.Name.LEVEL, -stacks, a_game, null, (Actor)a_target, Modifier.Duration.PERMANENT);
    }

    public override void Activate()
    {
        AddModifier(_mod);
        ChangeAOEStatModifier(_mod, -stacks);
        target.owner.events.onPlayCard += OnPlayCard;
        target.owner.events.onEndTurn += OnEndTurn;
        
    }

    public override void Deactivate()
    {
        RemoveModifier(_mod);
        target.owner.events.onPlayCard -= OnPlayCard;
        target.owner.events.onEndTurn -= OnEndTurn;
    }

    private void OnPlayCard(Actor a_actor, Card a_card)
    {
        Debug.Assert(a_actor == target);
        if (_mod.Compare(a_card))
        {
            RemoveStatusEffect(target, Name.Alacrity, 1);
            ChangeAOEStatModifier(_mod, stacks);
        }
    }

    private void OnEndTurn(Actor a_actor)
    {
        RemoveStatusEffect(target, Name.Alacrity, 9999);
    }
}
*/
