using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGame
{
    private Stack<GameAction> _actions;
    private bool _takingAction = false;
    private List<Actor> _players;
    public GameEvents events { get; private set; }
    public int turn { get; private set; }
    public CardGameUI ui {get; private set; }

    public List<Card> playedThisTurn;
    public List<Actor> players
    {
        get
        {
            List<Actor> ret = new List<Actor>();
            foreach (Actor actor in _players)
            {
                ret.Add(actor);
            }
            return ret;
        }
    }
    public GameAction confirmedAction { get; private set; }
    public GameAction currentAction { get { return _actions.Peek(); } }
    public Actor currentPlayer { get { return _players[turn]; } }
    public Actor humanPlayer { get { return _players[0]; } }
    public List<Actor> Opponents(Actor a_actor)
    {
        List<Actor> opps = new List<Actor>();
        foreach (Actor actor in _players)
        {
            if (actor != a_actor) { opps.Add(actor); }
        }
        return opps;
    }
    public Actor Player(int a_index) { return _players[a_index]; }
    public CardGame()
    {
        events = new GameEvents();
        _players = new List<Actor>();
        _players.Add(new Actor(this, true));
        _players.Add(new Actor(this, false));
        _actions = new Stack<GameAction>();
        playedThisTurn = new List<Card>();
        turn = 0;
    }

    public void SetUI(CardGameUI a_ui)
    {
        if (ui != null) { return; }
        ui = a_ui;
        int numPlayers = _players.Count;
        for (int ii = 0; ii < numPlayers; ii++)
        {
            ui.SetPlayerUI(_players[ii], ii);
        }
    }
    public float Evaluate(Actor a_actor)
    {
        float score = 0;
        score += a_actor.hand.cards.Count;
        score += 10.0f*a_actor.health;
        foreach (Card card in a_actor.active.cards)
        {
            score += card.GetStat(Card.StatName.LEVEL);
            score += card.GetStat(Card.StatName.HEALTH);
            score += card.GetStat(Card.StatName.POWER);
            foreach (StatusEffect.Name s in card.statusEffects.Keys)
            {
                score += EvaluateStatusEffect(s, card.GetStatusEffect(s));
            }
        }
        foreach (Actor opponent in a_actor.opponents)
        {
            score -= opponent.hand.cards.Count;
            score -= 10.0f*opponent.health;
            foreach (Card card in opponent.active.cards)
            {
                score -= card.GetStat(Card.StatName.LEVEL);
                score -= card.GetStat(Card.StatName.HEALTH);
                score -= card.GetStat(Card.StatName.POWER);
                foreach (StatusEffect.Name s in card.statusEffects.Keys)
                {
                    score -= EvaluateStatusEffect(s, card.GetStatusEffect(s));
                }
            }
        }
        return score;
    }
    public float EvaluateStatusEffect(StatusEffect.Name a_status, int a_stacks)
    {
        switch (a_status)
        {
            case StatusEffect.Name.Poison: return -1.0f * a_stacks;
            default: return 0.0f;
        }
    }
    public List<GameAction> ListActions(Actor a_actor)
    {
        List<GameAction> actions = new List<GameAction>();
        foreach (Card card in a_actor.hand.cards)
        {
            if (!card.playable) { continue; }
            if (card.ability == null || card.ability.minTargets == 0)
            {
                actions.Add(new PlayCard(card, null));
            }
            List<List<ITarget>> targetCombinations = FindTargets(card, TargetingPhase.Action.PLAY);
            foreach (List<ITarget> targets in targetCombinations)
            {
                actions.Add(new PlayCard(card, targets));
            }
        }
        foreach (UnitCard unit in a_actor.units)
        {
            if (!unit.canAttack) { continue; }
            List<IDamageable> defenders = FindAttackTargets(unit);
            foreach (IDamageable d in defenders)
            {
                actions.Add(new DeclareAttack(unit, d));
            }
        }
        return actions;
    }
    public List<List<ITarget>> FindTargets(Card a_card, TargetingPhase.Action a_targetType)
    {
        List<TargetQuery> queries = new List<TargetQuery>();
        int maxTargets = 0;
        switch (a_targetType)
        {
            case TargetingPhase.Action.PLAY:
                if (a_card.ability == null)
                {
                    maxTargets = 0;
                } else
                {
                    queries.AddRange(a_card.ability.targets);
                    maxTargets = a_card.ability.maxTargets;
                }
                break;
            case TargetingPhase.Action.ACTIVATION:
                Debug.Assert(a_card.ability != null);
                queries.AddRange(a_card.ability.aTargets);
                maxTargets = a_card.ability.aMaxTargets;
                break;
            default: Debug.LogError("Unrecognized TargetingPhase.Type: " + a_targetType); break;
        }
        List<List<ITarget>> targetCombinations = new List<List<ITarget>>();
        if (queries.Count == 0) { return targetCombinations; }
        List<ITarget> visibleTargets = VisibleTargets(a_card.owner);
        ListTools.Combinations(targetCombinations, visibleTargets, maxTargets);
        for (int ii = targetCombinations.Count - 1; ii >= 0; ii--)
        {
            List<ITarget> targets = targetCombinations[ii];
            for (int jj = 0; jj < maxTargets; jj++)
            {
                if (!queries[jj].Compare(targets[jj])) { targetCombinations.Remove(targets); }
                break;
            }
        }
        return targetCombinations;
    }

    public List<IDamageable> FindAttackTargets(UnitCard a_unit)
    {
        List<IDamageable> defenders = new List<IDamageable>();
        foreach (Actor opponent in a_unit.opponents)
        {
            if (opponent.units.Count == 0) { defenders.Add(opponent); }
            else
            {
                defenders.AddRange(opponent.units);
            }
        }
        return defenders;
    }

    public List<ITarget> VisibleTargets(Actor a_actor)
    {
        List<ITarget> targets = new List<ITarget>();
        foreach (Actor actor in _players)
        {
            targets.Add(actor);
            foreach (Card card in actor.active.cards)
            {
                targets.Add(card);
            }
        }
        foreach (Card card in a_actor.hand.cards)
        {
            targets.Add(card);
        }
        return targets;
    }
    public bool IsActorTurn(Actor a_actor)
    {
        return (a_actor == _players[turn]);
    }
    public void SetTurn(Actor a_actor)
    {
        if (!_players.Contains(a_actor)) { return; }
        turn = _players.IndexOf(a_actor);
        playedThisTurn.Clear();
    }
    public void NextTurn()
    {
        turn++;
        if (turn >= _players.Count)
        {
            turn = 0;
        }
        playedThisTurn.Clear();
    }
    public void PrevTurn()
    {
        turn--;
        if (turn < 0)
        {
            turn = _players.Count - 1;
        }
    }
    public bool TakeAction(GameAction a_action)
    {
        if (!a_action.IsValid(this)) { return false; }

        _actions.Push(a_action);
        a_action.executing = true;
        //Debug.Log("ACTION: " + a_action + " | Tree depth: " + _actions.Count);
        a_action.Execute(this);
        events.Refresh();
        a_action.executing = false;
        return a_action.success;
    }

    public GameAction UndoAction()
    {
        GameAction action = _actions.Pop();
        action?.Undo(this);
        events.Refresh();
        return action;
    }

    public GameAction UndoAllActions()
    {
        GameAction action = null;
        while (_actions.Count > 0)
        {
            action = UndoAction();
        }
        return action;
    }

    public void ConfirmAction()
    {
        GameAction action = UndoAllActions();
        Debug.Log("ConfirmAction: " + action);
        CardGameManager.ConfirmAction(action);
    }
    public void ConfirmAction(GameAction a_action)
    {
        _actions.Clear();
        CardGameManager.ConfirmAction(a_action);
    }

    public void Print()
    {
        string output = "Game State:\n";
        for (int pi = 0; pi < _players.Count; pi++)
        {
            Actor player = _players[pi];
            output += "\tPlayer " + pi + "\n";
            output += "\t\thealth: " + player.GetStat(Actor.StatName.HEALTH) + "/" + player.GetStat(Actor.StatName.MAX_HEALTH);
            output += " | resource: " + player.GetStat(Actor.StatName.FOCUS) + "/" + player.GetStat(Actor.StatName.MAX_FOCUS);
            output += " | deck: " + players[pi].deck.cards.Count + "\n";
            output += "\t\tHand: \n";
            foreach (Card card in players[pi].hand.cards)
            {
                output += "\t\t\t" + card.ToString() + "\n";
            }
            output += "\t\tActive: \n";
            foreach (Card card in players[pi].active.cards)
            {
                output += "\t\t\t" + card.ToString() + "\n";
            }
            output += "\t\tDiscard: \n";
            foreach (Card card in players[pi].discard.cards)
            {
                output += "\t\t\t" + card.ToString() + "\n";
            }
        }
        Debug.Log(output);
    }
    public void Refresh()
    {
        events.Refresh();
        events.UIRefresh();
    }
}
