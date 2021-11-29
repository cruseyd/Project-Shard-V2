using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCard : GameAction
{
    private Card _card;
    private List<ITarget> _targets;
    public override Actor actor { get { return _card.owner; } }
    public ActivateCard(Card a_card, List<ITarget> a_targets = null)
    {
        _card = a_card;
        _targets = a_targets;
    }

    public override void Execute(CardGame a_game)
    {
        _card.events.Activate();
        _card.ability?.Activate(_targets);
        _card.numActions--;
    }

    public override void Preview(CardGame a_game)
    {
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
        _card.numActions++;
    }
}
