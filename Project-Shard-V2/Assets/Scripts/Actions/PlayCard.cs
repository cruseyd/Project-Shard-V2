using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCard : GameAction
{
    private Card _card;
    private List<ITarget> _targets;
    public override Actor actor { get { return _card.owner; } }
    public PlayCard(Card a_card, List<ITarget> a_targets = null)
    {
        _card = a_card;
        _targets = a_targets;
    }

    public override void Execute(CardGame a_game)
    {
        IncrementActorStatEffect payResourceEffect = 
            new IncrementActorStatEffect(Actor.StatName.FOCUS, -_card.stats.Get(CardStats.Name.LEVEL), _card.owner);
        payResourceEffect.Execute(a_game);
        //IncrementActorStatEffect addThresholdEffect = new IncrementActorStatEffect(_card.data.threshold, 1, _card.owner);
        //addThresholdEffect.Execute(a_game);
        switch (_card.data.type)
        {
            case Card.Type.ACTION:
                EntersZoneEffect discardEffect = new EntersZoneEffect(_card, _card.owner.discard, true);
                discardEffect.Execute(a_game);
                break;
            case Card.Type.FOLLOWER:
                EntersZoneEffect enterPlayEffect = new EntersZoneEffect(_card, _card.owner.active, true);
                enterPlayEffect.Execute(a_game);
                break;
            default:
                Debug.LogError("PlayCard::Execute | Error: Unrecognized Card Type: " + _card.data.type);
                break;
        }
        _card.events.Play();
        _card.owner.events.PlayCard(_card);
        _card.ability?.Play(_targets);
        a_game.playedThisTurn.Add(_card);
    }

    public override void Preview(CardGame a_game)
    {
        _card.ui.Move(a_game.ui.preview, true);
        _card.ui.state = ITargetUI.State.SOURCE;
        if (_targets != null && _targets.Count > 0)
        {
            foreach (ITarget t in _targets)
            {
                AnimationQueue.Add(new ShowTargetAnim(a_game, _card.ui.transform, t.obj.transform));
                if (t is Card)
                {
                    ((Card)t).ui.state = ITargetUI.State.SELECTED_TARGET;
                }
            }
        }
    }

    public override bool IsValid(CardGame a_game)
    {
        return a_game.IsActorTurn(actor);
    }

    public override void Undo(CardGame a_game)
    {
        base.Undo(a_game);
        a_game.playedThisTurn.Remove(_card);
    }
}
