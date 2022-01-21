using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GamePhase
{
    public enum Name
    {
        DEFAULT,
        IDLE,
        TARGETING,
        PREVIEW,
        AI_CONTROL,
        PRE_GAME,
        END_TURN
    }

    public Name name;
    private static IdlePhase _idle;
    private static PreGamePhase _preGame;
    private static EndTurnPhase _endTurn;
    public static IdlePhase idle
    {
        get
        {
            if (_idle == null)
            {
                _idle = new IdlePhase();
            }
            return _idle;
        }
    }
    public static PreGamePhase preGame
    {
        get
        {
            if (_preGame == null)
            {
                _preGame = new PreGamePhase();
            }
            return _preGame;
        }
    }
    public static EndTurnPhase endTurn
    {
        get
        {
            if (_endTurn == null)
            {
                _endTurn = new EndTurnPhase();
            }
            return _endTurn;
        }
    }
    public abstract GamePhase Enter(CardGame a_game);
    public abstract void Exit(CardGame a_game);
    public abstract GamePhase Confirm(CardGame a_game);
    public abstract GamePhase ProcessInput(CardGame a_game, CardGameInput a_input);
}

public class IdlePhase : GamePhase
{
    public IdlePhase() { name = Name.IDLE; }
    public override GamePhase Confirm(CardGame a_game)
    {
        return endTurn;
    }

    public override void Exit(CardGame a_game)
    {
    }

    public override GamePhase Enter(CardGame a_game)                               
    {
        a_game.Refresh();
        a_game.ui.SetConfirmButtonActive(true);
        a_game.ui.SetConfirmButtonText("End Player Turn");
        return null;
    }

    public override GamePhase ProcessInput(CardGame a_game, CardGameInput a_input)
    {
        if (a_input.target is CardUI)
        {
            Card card = a_input.target.targetData as Card;
            switch (a_input.type)
            {
                case CardGameInput.Type.DOUBLE_CLICK:
                    if (!card.activatable) { return null; }
                    if (card.ability.aMaxTargets == 0)
                    {
                        ActivateCard act = new ActivateCard(card, null);
                        CombatManager.ConfirmAction(act);
                    }
                    else
                    {
                        return new TargetingPhase(card, TargetingPhase.Action.ACTIVATION);
                    }
                    return null;
                case CardGameInput.Type.BEGIN_DRAG:
                    if (card.owner != a_game.humanPlayer) { return null; }
                    if (card.playable)
                    {
                        if (card.needsTarget) { return new TargetingPhase(card); }
                        else { a_game.ui.EnableDropZone(true); }
                    } else if (card is UnitCard && ((UnitCard)card).canAttack)
                    {
                        return new TargetingPhase(card);
                    }
                    return null;
                case CardGameInput.Type.CONTINUE_DRAG:
                    card.ui.trackingMouse = true;
                    if (card.owner != a_game.humanPlayer) { return null; }
                    //if (card.playable && card.needsTarget && !a_input.Hovering(DropZone.ID.TRIBUTE))
                    if (card.playable && card.needsTarget && (Input.mousePosition.y > card.ui.zone.transform.position.y))
                    {
                        return new TargetingPhase(card);
                    }
                    if (a_input.Hovering(DropZone.ID.TRIBUTE))
                    {
                        card.MarkTribute();
                    } else if (a_input.Hovering(DropZone.ID.PLAY) && card.playable && !card.needsTarget)
                    {
                        card.MarkSource();
                    } else
                    {
                        card.ClearMarks();
                    }
                    return null;
                case CardGameInput.Type.END_DRAG:
                    card.ui.trackingMouse = false;
                    if (card.owner != a_game.humanPlayer) { return null; }
                    if (a_input.Hovering(DropZone.ID.PLAY) && card.playable && !card.needsTarget)
                    {
                        PlayCard act = new PlayCard(card, null);
                        CombatManager.ConfirmAction(act);
                    } else if (a_input.Hovering(DropZone.ID.TRIBUTE) && card.owner.canTribute)
                    {
                        TributeCard act = new TributeCard(card);
                        CombatManager.ConfirmAction(act);
                    }
                    else
                    {
                        a_game.ui.EnableDropZone(false);
                        card.ui.ResetPosition();
                        return idle;
                    }
                    return null;
                case CardGameInput.Type.BEGIN_HOVER:
                    card.ui.Zoom(true);
                    return null;
                case CardGameInput.Type.END_HOVER:
                    card.ui.Zoom(false);
                    return null;
                default: return null;
            }
        }
        else if (a_input.target is ActorUI)
        {
            Actor actor = a_input.target.targetData as Actor;
            switch (a_input.type)
            {
                default: return null;
            }
        }
        return null;
    }
}
public class TargetingPhase : GamePhase
{
    public enum Action
    {
        PLAY,
        ACTIVATION
    }

    private Action _type;
    private Card _source;
    List<List<ITarget>> _validTargets;
    List<IDamageable> _validAttackTargets;
    private List<ITarget> _targets;
    public TargetingPhase(Card a_source, Action a_type = Action.PLAY)
    {
        name = Name.TARGETING;
        _source = a_source;
        _targets = new List<ITarget>();
        _type = a_type;
    }

    private bool CompareTarget (ITarget a_target)
    {
        switch (_type)
        {
            case Action.PLAY:
                return _source.ability.targets[_targets.Count].Compare(a_target);
            case Action.ACTIVATION:
                return _source.ability.aTargets[_targets.Count].Compare(a_target);
            default: return false;
        }
    }

    private void AddTarget (CardGame a_game, ITarget a_target)
    {
        Debug.Log("Trying to add target");
        if (CompareTarget(a_target))
        {
            Debug.Log("Adding target");
            _targets.Add(a_target);
            a_target.MarkChosenTarget();
            a_game.ui.FreezeTargetEffects(a_target.obj.transform);
            if (NeedsTarget())
            {
                foreach (List<ITarget> targetsList in _validTargets)
                {
                    targetsList[_targets.Count].MarkValidTarget();
                }
                //TODO: spawn new targeting effects
            } 
        }
        if (_targets.Count >= _source.ability.minTargets)
        {
            a_game.ui.SetConfirmButtonActive(true);
            a_game.ui.SetConfirmButtonText("Confirm Targets");
        }
    }
    private bool NeedsTarget()
    {
        switch (_type)
        {
            case Action.PLAY: return (_targets.Count < _source.ability.minTargets);
            case Action.ACTIVATION: return (_targets.Count < _source.ability.aMinTargets);
            default: return false;
        }
        
    }
    public override GamePhase Confirm(CardGame a_game)
    {
        switch (_type)
        {
            case Action.ACTIVATION:
                ActivateCard activate = new ActivateCard(_source, _targets);
                CombatManager.ConfirmAction(activate);
                break;
            case Action.PLAY:
                PlayCard play = new PlayCard(_source, _targets);
                CombatManager.ConfirmAction(play);
                break;
        }
        
        return idle;
    }

    public override GamePhase Enter(CardGame a_game)
    {
        _source.ui.ResetPosition();
        _source.MarkSource();
        if (_source.playable)
        {
            _validTargets = a_game.FindTargets(_source, _type);
            foreach (List<ITarget> targetList in _validTargets)
            {
                targetList[0].MarkValidTarget();
            }
        } else if (_source is UnitCard && ((UnitCard)_source).canAttack)
        {
            UnitCard unit = _source as UnitCard;
            _validAttackTargets = a_game.FindAttackTargets(unit);
            foreach (IDamageable target in _validAttackTargets)
            {
                target.MarkValidTarget();
            }
        }
        a_game.ui.SetConfirmButtonActive(false);
        a_game.ui.EnableTargetEffects(_source.ui.transform);

        return null;
    }

    public override void Exit(CardGame a_game)
    {
        a_game.ui.DisableTargetEffects();
    }

    public override GamePhase ProcessInput(CardGame a_game, CardGameInput a_input)
    {
        if (a_input.target is CardUI)
        {
            Card card = a_input.target.targetData as Card;
            switch (a_input.type)
            {
                case CardGameInput.Type.DOUBLE_CLICK:
                    AddTarget(a_game, card);
                    if (!NeedsTarget()) { return Confirm(a_game); }
                    return null;
                case CardGameInput.Type.CONTINUE_DRAG:
                    //if (a_input.Hovering(DropZone.ID.TRIBUTE)) //player needs to be able to tribute
                    if (Input.mousePosition.y < card.ui.zone.transform.position.y)
                    {
                        card.ui.trackingMouse = true;
                        return idle;
                    }
                    return null;
                case CardGameInput.Type.END_DRAG:
                    if (_source.playable)
                    {
                        Debug.Assert(NeedsTarget());
                        foreach (ITarget t in a_input.hoveredTargets)
                        {
                            if (t != _source){ AddTarget(a_game, t); break; }
                        }
                        if (!NeedsTarget())
                        {
                            return Confirm(a_game);
                        }
                        else
                        {
                            return idle;
                        }
                    } else if (_source is UnitCard && ((UnitCard)_source).canAttack)
                    {
                        UnitCard unit = _source as UnitCard;
                        foreach (ITarget t in a_input.hoveredTargets)
                        {
                            if (t is IDamageable && t != _source)
                            {
                                IDamageable d = t as IDamageable;
                                if (!_validAttackTargets.Contains(d)) { return idle; }

                                DeclareAttack declareAction = new DeclareAttack(unit, d);
                                CombatManager.ConfirmAction(declareAction);
                                return idle;
                            }
                        }
                    }
                    return idle;
                default: return null;
            }
        } else if (a_input.target is ActorUI)
        {
            Actor actor = a_input.target.targetData as Actor;
            switch (a_input.type)
            {
                case CardGameInput.Type.DOUBLE_CLICK:
                    AddTarget(a_game, actor);
                    return null;
                default: return null;
            }
        }
        if (a_input.type == CardGameInput.Type.CANCEL)
        {
            return idle;
        }
        return null;
    }
}
public class PreviewPhase : GamePhase
{
    private GameAction _action;
    public PreviewPhase(GameAction a_action)
    {
        name = Name.PREVIEW;
        _action = a_action;
    }
    public override GamePhase Confirm(CardGame a_game)
    {
        a_game.TakeAction(_action);
        a_game.UpdateCardKnowledge();
        _action.Show(a_game);
        if (a_game.currentPlayer == a_game.humanPlayer || _action is EndTurn)
        {
            GameAction startTurn = new StartTurn(a_game.currentPlayer);
            a_game.ConfirmAction(startTurn);
            return idle;
        } else
        {
            return new AIControlPhase(true);
        }
    }

    public override void Exit(CardGame a_game)
    {
        a_game.ui.DisableTargetEffects();
        a_game.Refresh();
    }

    public override GamePhase Enter(CardGame a_game)
    {
        _action.Preview(a_game);
        a_game.ui.SetConfirmButtonActive(true);
        a_game.ui.SetConfirmButtonText("Confirm");
        if (_action is EndTurn) { a_game.ui.SetConfirmButtonText("End Enemy Turn"); }
        else if (_action is StartTurn) { a_game.ui.SetConfirmButtonText("Start Enemy Turn"); }
        return null;
    }

    public override GamePhase ProcessInput(CardGame a_game, CardGameInput a_input)
    {
        return null;
    }
}

public class AIControlPhase : GamePhase
{
    private bool _inProgress = false;
    public AIControlPhase(bool a_inProgress)
    {
        name = Name.AI_CONTROL;
        _inProgress = a_inProgress;
    }
    public override GamePhase Confirm(CardGame a_game)
    {
        Actor actor = a_game.currentPlayer;
        Debug.Assert(actor != a_game.humanPlayer);
        List<GameAction> actionChain = new List<GameAction>();
        Decision decision = actor.Choose(a_game, actor, actionChain);
        return new PreviewPhase(decision.action);
    }

    public override GamePhase Enter(CardGame a_game)
    {
        a_game.Refresh();
        if (!_inProgress)
        {
            a_game.ui.SetConfirmButtonText("Start Enemy Turn");
            _inProgress = true;
            GameAction startTurn = new StartTurn(a_game.currentPlayer);
            a_game.ConfirmAction(startTurn);
            return null;
        } else
        {
            return Confirm(a_game);
        }
    }

    public override void Exit(CardGame a_game)
    {
    }

    public override GamePhase ProcessInput(CardGame a_game, CardGameInput a_input)
    {
        return null;
    }
}

public class PreGamePhase : GamePhase
{
    public PreGamePhase()
    {
        name = Name.PRE_GAME;
    }
    public override GamePhase Confirm(CardGame a_game)
    {
        GameAction action = new StartTurn(a_game.humanPlayer);
        CombatManager.ConfirmAction(action);
        return idle;
    }

    public override GamePhase Enter(CardGame a_game)
    {
        Debug.Log("Entering pregame phase");
        a_game.ui.SetConfirmButtonActive(true);
        a_game.ui.SetConfirmButtonText("Start Game");
        GameAction action = new StartGame();
        CombatManager.ConfirmAction(action);
        return null;
    }

    public override void Exit(CardGame a_game)
    {
    }

    public override GamePhase ProcessInput(CardGame a_game, CardGameInput a_input)
    {
        return null;
    }
}

public class EndTurnPhase : GamePhase
{
    public EndTurnPhase()
    {
        name = Name.END_TURN;
    }
    public override GamePhase Confirm(CardGame a_game)
    {
        int playerInfluence = a_game.currentPlayer.GetStat(Actor.StatName.INFLUENCE);
        int playerTotalDefiance = a_game.currentPlayer.GetStat(Actor.StatName.TOTAL_DEFIANCE);
        if (playerInfluence >= playerTotalDefiance)
        {
            EndTurn act = new EndTurn(a_game.currentPlayer);
            a_game.ConfirmAction(act);
            if (a_game.currentPlayer != a_game.humanPlayer)
            {
                return new AIControlPhase(false); // start AI turn
            }
        }
        else
        {
            a_game.ui.SetConfirmButtonText("Dismiss Units");
        }
        return null;
    }

    public override GamePhase Enter(CardGame a_game)
    {
        return Confirm(a_game);
    }

    public override void Exit(CardGame a_game)
    {
    }

    public override GamePhase ProcessInput(CardGame a_game, CardGameInput a_input)
    {
        if (a_input.target is CardUI)
        {
            Card card = a_input.target.targetData as Card;
            switch (a_input.type)
            {
                case CardGameInput.Type.DOUBLE_CLICK:
                    if (card is UnitCard && card.isInPlay)
                    {
                        DismissUnit dismiss = new DismissUnit((UnitCard)card);
                        a_game.ConfirmAction(dismiss);
                        return Confirm(a_game);
                    }
                    return null;
                case CardGameInput.Type.BEGIN_HOVER:
                    card.ui.Zoom(true);
                    return null;
                case CardGameInput.Type.END_HOVER:
                    card.ui.Zoom(false);
                    return null;
                default: return null;
            }
        }
        else if (a_input.target is ActorUI)
        {
            Actor actor = a_input.target.targetData as Actor;
            switch (a_input.type)
            {
                default: return null;
            }
        }
        return null;
    }
}
