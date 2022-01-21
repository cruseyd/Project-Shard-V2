using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGame
{
    private Stack<GameAction> _actions;
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
            score += card.stats.Get(CardStats.Name.LEVEL);
            score += card.stats.Get(CardStats.Name.HEALTH);
            score += card.stats.Get(CardStats.Name.POWER);
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
                score -= card.stats.Get(CardStats.Name.LEVEL);
                score -= card.stats.Get(CardStats.Name.HEALTH);
                score -= card.stats.Get(CardStats.Name.POWER);
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
            if (!card.Known(a_actor)) { continue; }
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
            List<Card> enemyEvasive = opponent.GetCards(CardZone.Type.ACTIVE, AbilityKeyword.EVASIVE);
            List<Card> enemyGuardian = opponent.GetCards(CardZone.Type.ACTIVE, AbilityKeyword.GUARDIAN);
            List<UnitCard> enemyUnits = opponent.units;
            List<Card> enemyNonEvasive = new List<Card>();
            foreach (Card card in enemyUnits)
            {
                if (!card.HasKeyword(AbilityKeyword.EVASIVE))
                {
                    enemyNonEvasive.Add(card);
                }
            }
            if (enemyUnits.Count == 0)
            {
                defenders.Add(opponent);
            } else
            {
                if (a_unit.HasKeyword(AbilityKeyword.EVASIVE))
                {
                    if (enemyEvasive.Count == 0) { defenders.Add(opponent); }
                    defenders.AddRange(enemyUnits);
                }
                else
                {
                    if (enemyGuardian.Count > 0)
                    {
                        foreach (Card card in enemyGuardian)
                        {
                            defenders.Add(card as UnitCard);
                        }
                    }
                    else if (enemyNonEvasive.Count > 0)
                    {
                        foreach (Card card in enemyNonEvasive)
                        {
                            defenders.Add(card as UnitCard);
                        }
                    } else
                    {
                        defenders.AddRange(enemyUnits);
                    }
                }
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
        a_action.Execute(this);
        events.Refresh();
        a_action.executing = false;
        return a_action.success;
    }

    public GameAction UndoAction()
    {
        GameAction action = _actions.Pop();
        action?.Undo(this);
        //events.Refresh();
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

    public void ConfirmAction(GameAction a_action)
    {
        _actions.Clear();
        CombatManager.ConfirmAction(a_action);
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
                output = PrintCard(output, card, player);
            }
            output += "\t\tActive: \n";
            foreach (Card card in players[pi].active.cards)
            {
                output = PrintCard(output, card, player);
            }
            output += "\t\tDiscard: \n";
            foreach (Card card in players[pi].discard.cards)
            {
                output = PrintCard(output, card, player);
            }
        }
        Debug.Log(output);
    }
    public string PrintCard(string output, Card card, Actor player)
    {
        output += "\t\t\t" + card.ToString();
        if (card.Known(player)) { output += " (known to owner) "; }
        else { output += " (unknown to owner) "; }
        output += "\n";
        foreach (CardStatModifier modifier in card.stats.modifiers)
        {
            output += "\t\t\t\t modifier: " + modifier.stat.ToString();
            if (modifier.value > 0)
            {
                output += " +" + modifier.value;
            } else
            {
                output += " -" + Mathf.Abs(modifier.value);
            }
            output += "\n";
        }
        return output;
    }
    public void Refresh()
    {
        //events.Refresh();
        events.UIRefresh();
    }

    public void UpdateCardKnowledge()
    {
        foreach (Actor player in _players)
        {
            foreach (Card card in player.hand.cards)
            {
                card.SetKnown(player, true);
            }
            foreach (Card card in player.active.cards)
            {
                card.SetKnown(player, true);
                foreach (Actor opp in player.opponents)
                {
                    card.SetKnown(opp, true);
                }
            }
            foreach (Card card in player.discard.cards)
            {
                card.SetKnown(player, true);
                foreach (Actor opp in player.opponents)
                {
                    card.SetKnown(opp, true);
                }
            }
            foreach (Card card in player.deck.cards)
            {
                card.SetKnown(player, false);
                foreach (Actor opp in player.opponents)
                {
                    card.SetKnown(opp, false);
                }
            }
        }
    }

    public void FixCardPlacement()
    {
        foreach (Actor player in players)
        {
            foreach (Card card in player.hand.cards)
            {
                if (card.ui.zone == ui.preview) { continue; }
                if (card.ui.zone != player.hand.ui)
                {
                    card.ui.Move(player.hand.ui, card.ui.faceUp, false);
                }
            }
            player.hand.ui.Organize();
            foreach (Card card in player.active.cards)
            {
                if (card.ui.zone == ui.preview) { continue; }
                if (card.ui.zone != player.active.ui)
                {
                    card.ui.Move(player.active.ui, card.ui.faceUp, false);
                }
            }
            player.active.ui.Organize();
            foreach (Card card in player.discard.cards)
            {
                if (card.ui.zone == ui.preview) { continue; }
                if (card.ui.zone != player.discard.ui)
                {
                    card.ui.Move(player.discard.ui, card.ui.faceUp, false);
                }
            }
            player.discard.ui.Organize();
            foreach (Card card in player.deck.cards)
            {
                if (card.ui.zone == ui.preview) { continue; }
                if (card.ui.zone != player.deck.ui)
                {
                    card.ui.Move(player.deck.ui, false, false);
                }
            }
            player.deck.ui.Organize();
        }
    }

}
