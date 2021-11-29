using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decision
{
    public GameAction action;
    public float score;
    public Decision(GameAction a_action, float a_score) { action = a_action; score = a_score; }
}
public class Actor : ITarget, IModifiable, IDamageable
{
    public enum StatName
    {
        DEFAULT,
        HEALTH,
        MAX_HEALTH,
        FOCUS,
        MAX_FOCUS,
        INFLUENCE,
        TOTAL_DEFIANCE,
        THRESHOLD_RED,
        THRESHOLD_GRN,
        THRESHOLD_BLU,
        THRESHOLD_VLT,
        THRESHOLD_GLD,
        THRESHOLD_IGO
    }

    private static GameObject _prefab;
    private Dictionary<CardZone.Type, CardZone> _zones;
    private Dictionary<StatName, int> _stats;
    private List<ActorModifier> _modifiers;
    private Dictionary<StatusEffect.Name, StatusEffect> _statusEffects;
    private CardGame _game;

    public bool canTribute;
    public List<ActorCardStatModifier> cardStatModifiers
    {
        get
        {
            List<ActorCardStatModifier> mods = new List<ActorCardStatModifier>();
            foreach (ActorModifier mod in _modifiers)
            {
                if (mod is ActorCardStatModifier)
                {
                    mods.Add((ActorCardStatModifier)mod);
                }
            }
            return mods;
        }
    }
    public ActorEvents events { get; private set; }
    public DamageableEvents damageEvents { get; private set; }
    public bool playerControlled { get; private set; }
    public ITargetUI targetUI { get { return ui; } }
    public ActorUI ui { get; private set; }
    public GameObject obj { get { return ui.gameObject; } }
    public Deck deck { get { return (Deck)_zones[CardZone.Type.DECK]; } }
    public CardZone hand { get { return _zones[CardZone.Type.HAND]; } }
    public CardZone discard { get { return _zones[CardZone.Type.DISCARD]; } }
    public CardZone active { get { return _zones[CardZone.Type.ACTIVE]; } }
    public Actor owner { get { return this; } }
    public List<Actor> opponents
    {
        get
        {
            return _game.Opponents(this);
        }
    }
    public List<UnitCard> units
    {
        get
        {
            List<UnitCard> u = new List<UnitCard>();
            foreach (Card card in active.cards)
            {
                if (card is UnitCard) { u.Add((UnitCard)card); }
            }
            return u;
        }
    }
    public int health { get { return GetStat(StatName.HEALTH); } }
    public int maxHealth { get { return GetStat(StatName.MAX_HEALTH); } }
    public int focus { get { return GetStat(StatName.FOCUS); } }
    public int maxFocus { get { return GetStat(StatName.MAX_FOCUS); } }
    public GenericDictionary<StatusEffect.Name, int> statusEffects
    {
        get
        {
            GenericDictionary<StatusEffect.Name, int> output = new GenericDictionary<StatusEffect.Name, int>();
            foreach (StatusEffect.Name s in _statusEffects.Keys)
            {
                output[s] = GetStatusEffect(s);
            }
            return output;
        }
    }
    public bool needsToDismiss
    {
        get
        {
            return GetStat(StatName.TOTAL_DEFIANCE) > GetStat(StatName.INFLUENCE);
        }
    }
    public ActorUI Spawn(Transform a_position)
    {
        if (_prefab == null)
        {
            _prefab = Resources.Load("Prefabs/Player") as GameObject;
        }

        //HACK: these are here because CardGameParams must first be defined. Should not be tied to graphics spawn.
        _stats[StatName.HEALTH] = CardGameParams.playerStartingHealth;
        _stats[StatName.MAX_HEALTH] = CardGameParams.playerStartingHealth;
        _stats[StatName.FOCUS] = CardGameParams.playerStartingResource;
        _stats[StatName.MAX_FOCUS] = CardGameParams.playerStartingResource;
        _stats[StatName.INFLUENCE] = CardGameParams.playerStartingInfluence;
        _stats[StatName.TOTAL_DEFIANCE] = 0;

        _stats[StatName.THRESHOLD_RED] = 0;
        _stats[StatName.THRESHOLD_GRN] = 0;
        _stats[StatName.THRESHOLD_BLU] = 0;
        _stats[StatName.THRESHOLD_VLT] = 0;
        _stats[StatName.THRESHOLD_GLD] = 0;
        _stats[StatName.THRESHOLD_IGO] = 0;

        GameObject actorGO = GameObject.Instantiate(_prefab, a_position) as GameObject;
        ActorUI a_ui = actorGO.GetComponent<ActorUI>();
        a_ui.Initialize(this);
        ui = a_ui;
        _game.events.onRefresh += Refresh;
        _game.events.onUIRefresh += a_ui.Refresh;
        return a_ui;
    }

    public Actor(CardGame a_game, bool a_playerControlled)
    {
        playerControlled = a_playerControlled;
        _game = a_game;
        _zones = new Dictionary<CardZone.Type, CardZone>();
        _stats = new Dictionary<StatName, int>();
        _modifiers = new List<ActorModifier>();
        _statusEffects = new Dictionary<StatusEffect.Name, StatusEffect>();

        _zones[CardZone.Type.DECK] = new Deck(a_game, this, null);
        _zones[CardZone.Type.HAND] = new CardZone(a_game, this, CardZone.Type.HAND);
        _zones[CardZone.Type.DISCARD] = new CardZone(a_game, this, CardZone.Type.DISCARD);
        _zones[CardZone.Type.ACTIVE] = new CardZone(a_game, this, CardZone.Type.ACTIVE);

        events = new ActorEvents(this);
        damageEvents = new DamageableEvents(this);
        canTribute = true;
        events.onStartTurn += (_actor_) =>
        {
            canTribute = true;
        };
    }

    public void Refresh()
    {
        int totalDefiance = 0;
        foreach (UnitCard unit in units)
        {
            totalDefiance += unit.GetStat(Card.StatName.DEFIANCE);
        }
        SetStat(StatName.TOTAL_DEFIANCE, totalDefiance);
    }
    public void SetDeck(Decklist a_decklist)
    {
        deck.SetList(a_decklist);
        deck.SpawnCards();
    }
    //=============================================================================================
    // Accessors
    public int GetStat(StatName a_stat)
    {
        Debug.Assert(_stats.ContainsKey(a_stat));
        int value = _stats[a_stat];
        foreach (ActorModifier mod in _modifiers)
        {
            if (mod is ActorStatModifier)
            {
                ActorStatModifier m = mod as ActorStatModifier;
                if (m.stat == a_stat)
                {
                    value += m.value;
                }
            }
        }
        return value;
    }
    public void SetStat(StatName a_stat, int a_value)
    {
        Debug.Assert(_stats.ContainsKey(a_stat));
        _stats[a_stat] = a_value;
    }
    public void IncrementStat(StatName a_stat, int a_value)
    {
        Debug.Assert(_stats.ContainsKey(a_stat));
        _stats[a_stat] += a_value;
    }
    public List<Card> GetCards(CardZone.Type a_zoneName)
    {
        Debug.Assert(_zones.ContainsKey(a_zoneName));
        return _zones[a_zoneName].cards;
    }

    public CardZone GetZone(CardZone.Type a_zoneName)
    {
        if (!_zones.ContainsKey(a_zoneName)) { return null; }
        return _zones[a_zoneName];
    }

    //=============================================================================================
    // Game Actions
    public Card Draw()
    {
        Card drawn = deck.Draw();
        if (drawn == null) { return null; }
        drawn.Move(hand);
        return drawn;
    }
    public void Discard(Card a_card)
    {
        a_card.Move(discard);
    }
    public Decision Choose(CardGame a_game, Actor a_actor, Stack<Decision> a_actions)
    {
        List<GameAction> actions = a_game.ListActions(a_actor);
        float score = a_game.Evaluate(a_actor);
        Decision decision = new Decision(new EndTurn(a_actor), score);
        foreach (GameAction action in actions)
        {
            a_game.TakeAction(action);
            Decision branchScore = Choose(a_game, a_actor, a_actions);
            if (branchScore.score > score) { decision = new Decision(action, branchScore.score); }
            a_game.UndoAction();
        }
        a_actions.Push(decision);
        return decision;
    }
    public Decision Choose(CardGame a_game, Actor a_actor, List<GameAction> a_actions)
    {
        List<GameAction> actions = a_game.ListActions(a_actor);
        float score = a_game.Evaluate(a_actor);
        if (actions.Count == 0)
        {
            string output = "";
            foreach (GameAction action in a_actions)
            {
                output += (" | " + action.ToString());
            }
            output += (" | score: " + score);
            Debug.Log(output);
            a_game.Print();
        }
        Decision decision = ChooseDismissal(a_game);
        foreach (GameAction action in actions)
        {
            float testScore_0 = a_game.Evaluate(this);
            a_game.TakeAction(action);
            a_actions.Add(action);
            Decision branchScore = Choose(a_game, a_actor, a_actions);
            if (branchScore.score > score)
            {
                decision = new Decision(action, branchScore.score);
                score = branchScore.score;
            }
            a_game.UndoAction();
            float testScore_1 = a_game.Evaluate(this);
            if (testScore_0 != testScore_1)
            {
                Debug.Log("Error reversing action: " + action.ToString());
            }
            a_actions.Remove(action);
        }
        
        return decision;
    }
    public Decision ChooseDismissal(CardGame a_game)
    {
        if (!needsToDismiss)
        {
            return new Decision(new EndTurn(this), a_game.Evaluate(this));
        }
        List<DismissUnit> actions = new List<DismissUnit>();
        foreach (UnitCard unit in units)
        {
            actions.Add(new DismissUnit(unit));
        }
        float score = Mathf.NegativeInfinity;
        Decision decision = null;
        foreach (GameAction action in actions)
        {
            a_game.TakeAction(action);
            Decision branchScore = ChooseDismissal(a_game);
            if (branchScore.score > score)
            {
                decision = new Decision(action, branchScore.score);
                score = branchScore.score;
            }
            a_game.UndoAction();
        }
        return decision;
    }
    //=============================================================================================
    // IDamageable Interface
    public void TakeDamage(DamageData a_damageData)
    {
        a_damageData.source.owner.events.BeforeDealDamage(a_damageData);
        if (a_damageData.damage < 0)
        {
            // prevent overhealing
            int maxHeal = health - maxHealth;
            a_damageData.damage = Mathf.Clamp(a_damageData.damage, maxHeal, 0);
        }

        IncrementStat(StatName.HEALTH, -a_damageData.damage);
        a_damageData.locked = true; //if the data is changed after this, undo will not work
        
        if (a_damageData.damage > 0)
        {
            a_damageData.source.sourceEvents.DealDamage(a_damageData);
            damageEvents.TakeDamage(a_damageData);
            a_damageData.source.owner.events.DealDamage(a_damageData);
        }
        else if (a_damageData.damage < 0)
        {
            a_damageData.source.sourceEvents.Heal(a_damageData);
            damageEvents.Heal(a_damageData);
        }
    }
    public void SetHealth (int a_value)
    {
        SetStat(StatName.HEALTH, a_value);
    }

    //=============================================================================================
    // IModifiable Interface
    public void RemoveModifier(Modifier a_modifier)
    {
        if (a_modifier is ActorModifier)
        {
            ActorModifier mod = a_modifier as ActorModifier;
            if (!_modifiers.Contains(mod)) { return; }
            Debug.Log("Actor::RemoveModifier");
            _modifiers.Remove(mod);
            mod.Deactivate();
        }
    }
    public void AddModifier(Modifier a_modifier)
    {
        if (a_modifier is ActorModifier)
        {
            ActorModifier mod = a_modifier as ActorModifier;
            if (_modifiers.Contains(mod)) { return; }
            Debug.Log("Actor::AddModifier");
            _modifiers.Add(mod);
            mod.Activate();
        }
    }
    public void RemoveAllModifiers()
    {
        foreach (ActorModifier mod in _modifiers)
        {
            if (mod.source != null)
            {
                _modifiers.Remove(mod);
                mod.Deactivate();
            }
        }
    }
    //=============================================================================================
    // ITarget / ISource
    public void MarkChosenTarget() {}
    public void MarkValidTarget() { ui.state = ITargetUI.State.VALID_TARGET; }
    public void MarkSource() { }
    public void ClearMarks() { ui.state = ITargetUI.State.DEFAULT; }
    public void AddStatusEffect(StatusEffect.Name a_name, int a_stacks)
    {
        if (_statusEffects.ContainsKey(a_name) && _statusEffects[a_name].stackable)
        {
            _statusEffects[a_name].stacks += a_stacks;
        }
        else
        {
            StatusEffect statusEffect = StatusEffect.Get(a_name, _game, this);
            if (statusEffect == null) { Debug.LogError("Actor::AddStatusEffect | Could not find StatusEffect " + a_name); }
            _statusEffects[a_name] = StatusEffect.Get(a_name, _game, this);
            _statusEffects[a_name].stacks = a_stacks;
            _statusEffects[a_name].Activate();
        }
    }

    public void RemoveStatusEffect(StatusEffect.Name a_name, int a_stacks)
    {
        StatusEffect effect = _statusEffects[a_name];
        if (effect != null)
        {
            effect.stacks -= a_stacks;
            if (effect.stacks <= 0)
            {
                effect.Deactivate();
                _statusEffects.Remove(a_name);
            }
        }
    }

    public void RemoveAllStatusEffects()
    {
        GenericDictionary<StatusEffect.Name, int> currentStatus = statusEffects;
        foreach (StatusEffect.Name s in currentStatus.Keys)
        {
            RemoveStatusEffect(s, currentStatus[s]);
        }
    }
    public int GetStatusEffect(StatusEffect.Name a_name)
    {
        if (_statusEffects.ContainsKey(a_name)) { return _statusEffects[a_name].stacks; }
        else { return 0; }
    }

    //=============================================================================================
}
