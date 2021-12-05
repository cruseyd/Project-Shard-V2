using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : ITarget, IModifiable, ISource
{
    public enum StatName
    {
        DEFAULT,
        LEVEL,
        POWER,
        HEALTH,
        MAX_HEALTH,
        DEFIANCE
    }

    public enum Type
    {
        DEFAULT,
        ACTION,
        FOLLOWER
    }
    
    public enum Color
    {
        DEFAULT,
        RED,
        GREEN,
        BLUE,
        VIOLET,
        GOLD,
        INDIGO
    }

    public static Card Get(CardGame a_game, CardData a_data, Actor a_actor)
    {
        switch (a_data.type)
        {
            case Type.ACTION: return new SpellCard(a_game, a_data, a_actor);
            case Type.FOLLOWER: return new UnitCard(a_game, a_data, a_actor);
            default:
                Debug.LogError("Card::Get | Error: Unknown Card Type: " + a_data.type);
                return null;
        }
    }


    protected Dictionary<StatName, int> _stats;
    protected List<CardModifier> _modifiers;
    protected Dictionary<StatusEffect.Name, StatusEffect> _statusEffects;
    protected Dictionary<AbilityKeyword, KeyAbility> _keyAbilities;
    protected CardGame _game;
    protected bool _hasTargets;
    public int numActions;
    public SourceEvents sourceEvents { get; protected set; }
    public CardEvents events { get; private set; }
    public CardAbility ability { get; private set; }
    public List<Actor> opponents
    {
        get
        {
            return _game.Opponents(owner);
        }
    }
    public Actor owner { get; private set; }
    public CardZone zone { get; private set; }
    public CardData data { get; private set; }
    public CardUI ui { get; protected set; }
    public GameObject obj { get { return ui.gameObject; } }
    public ITargetUI targetUI { get { return ui; } }
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
    public GenericDictionary<Card.StatName, int> stats
    {
        get
        {
            GenericDictionary<Card.StatName, int> output = new GenericDictionary<StatName, int>();
            foreach (Card.StatName stat in _stats.Keys)
            {
                output[stat] = _stats[stat];
            }
            return output;
        }
    }
    public List<CardModifier> modifiers
    {
        get
        {
            List<CardModifier> mods = new List<CardModifier>();
            foreach (CardModifier mod in _modifiers)
            {
                mods.Add(mod);
            }
            return mods;
        }
    }
    //=============================================================================================
    // Boolean flags
    public bool playable
    {
        get
        {
            if (CombatManager.phase == GamePhase.Name.PRE_GAME) { return false; }
            if (owner.focus < data.level) { return false; }
            if (_game.currentPlayer != owner) { return false; }
            if (zone.type != CardZone.Type.HAND) { return false; }
            if (!_hasTargets) { return false; }
            if (owner.GetStat(Actor.StatName.FOCUS) < data.level) { return false; }
            /*
            if (owner.GetStat(Actor.StatName.THRESHOLD_RED) < RequiredThreshold(Card.Color.RED)) { return false; }
            if (owner.GetStat(Actor.StatName.THRESHOLD_GRN) < RequiredThreshold(Card.Color.GREEN)) { return false; }
            if (owner.GetStat(Actor.StatName.THRESHOLD_BLU) < RequiredThreshold(Card.Color.BLUE)) { return false; }
            if (owner.GetStat(Actor.StatName.THRESHOLD_VLT) < RequiredThreshold(Card.Color.VIOLET)) { return false; }
            if (owner.GetStat(Actor.StatName.THRESHOLD_GLD) < RequiredThreshold(Card.Color.GOLD)) { return false; }
            if (owner.GetStat(Actor.StatName.THRESHOLD_IGO) < RequiredThreshold(Card.Color.INDIGO)) { return false; }
            */
            // check if anything else prevents playing this
            Attempt attempt = new Attempt();
            events.CheckPlayable(attempt);
            return attempt.success;
        }
    }
    public bool activatable
    {
        get
        {
            if (numActions <= 0) { return false; }
            if (ability == null || !ability.canActivate) { return false; }
            if (_game.currentPlayer != owner) { return false; }
            if (zone.type != CardZone.Type.ACTIVE) { return false; }
            if (!_hasTargets) { return false; }

            // check if anything else prevents playing this
            return true;
        }
    }
    public bool needsTarget
    {
        get
        {
            return (ability != null && ability.maxTargets > 0);
        }
    }
    public bool humanControlled { get { return owner == _game.humanPlayer; } }
    public bool playedThisTurn { get { return _game.playedThisTurn.Contains(this); } }
    public bool isInPlay { get { return zone.type == CardZone.Type.ACTIVE; } }
    public bool isInHand { get { return zone.type == CardZone.Type.HAND; } }
    public bool isInDiscard { get { return zone.type == CardZone.Type.DISCARD; } }
    public bool isInDeck { get { return zone.type == CardZone.Type.DECK; } }
    //=============================================================================================
    // Basic Methods
    public Card(CardGame a_game, CardData a_data, Actor a_owner)
    {
        _game = a_game;
        owner = a_owner;
        data = a_data;
        events = new CardEvents(this);
        sourceEvents = new SourceEvents(this);

        _stats = new Dictionary<StatName, int>();
        _modifiers = new List<CardModifier>();
        _statusEffects = new Dictionary<StatusEffect.Name, StatusEffect>();

        ability = CardAbility.Get(a_game, data.id, this);
        _keyAbilities = new Dictionary<AbilityKeyword, KeyAbility>();
        foreach (AbilityKeyword key in a_data.abilityKeywords)
        {
            _keyAbilities[key] = KeyAbility.Get(a_game, key, this);
        }
        if (ability != null && ability.minTargets > 0)
        {
            _hasTargets = false;
        }
        numActions = 1;
        _hasTargets = true;

        a_game.events.onRefresh += Refresh;
 
    }
    public abstract CardUI Spawn(Vector3 a_spawnPosition, CardZoneUI a_zone);
    public virtual void Initialize()
    {
        RemoveAllStatusEffects();
        RemoveAllModifiers();
    }
    public virtual void Refresh()
    {
        _hasTargets = true;
        if (zone.type == CardZone.Type.HAND)
        {
            if (ability != null && ability.minTargets > 0)
            {
                List<List<ITarget>> targets = _game.FindTargets(this, TargetingPhase.Action.PLAY);
                if (targets.Count == 0) { _hasTargets = false; }
            }
        } else if ( zone.type == CardZone.Type.ACTIVE)
        {
            if (ability != null && ability.canActivate && ability.aMinTargets > 0)
            {
                List<List<ITarget>> targets = _game.FindTargets(this, TargetingPhase.Action.ACTIVATION);
                if (targets.Count == 0) { _hasTargets = false; }
            }
        }
    }
    public bool HasKeyword(Keyword a_key)
    {
        return data.keywords.Contains(a_key);
    }
    public bool HasKeyword(AbilityKeyword a_key)
    {
        return _keyAbilities.ContainsKey(a_key);
    }
    public int GetStat(StatName a_stat)
    {
        if (!_stats.ContainsKey(a_stat)) { return 0; }
        int value = _stats[a_stat];
        foreach (CardModifier mod in _modifiers)
        {
            if (mod is CardStatModifier)
            {
                CardStatModifier m = mod as CardStatModifier;
                if (m.stat == a_stat)
                {
                    value += m.value;
                }
            }
        }
        foreach (ActorCardStatModifier mod in owner.cardStatModifiers)
        {
            if (mod.stat == a_stat && mod.Compare(this))
            {
                value += mod.value;
            }
        }
        return value;
    }
    public void SetStat(StatName a_stat, int a_value)
    {
        if (_stats.ContainsKey(a_stat))
        {
            _stats[a_stat] = a_value;
        }
    }
    public void SetOwner(Actor a_actor)
    {
        if (owner != a_actor)
        {
            owner = a_actor;
            owner.events.onStartTurn += (_actor_) =>
            {
                this.numActions = 1;
            };
        }
    }
    public void IncrementStat(StatName a_stat, int a_value)
    {
        Debug.Assert(_stats.ContainsKey(a_stat));
        //Debug.Log("health = " + _stats[a_stat] + " | Incrementing " + a_stat + " of " + data.name + " by " + a_value);
        _stats[a_stat] += a_value;
    }
    public int RequiredThreshold(Card.Color a_color)
    {
        int n = 0;
        for (int ii = 0; ii < 5; ii++)
        {
            if (data.Threshold(ii) == a_color) { n++; }
        }
        return n;
    }
    public void Move(CardZone a_destZone)
    {
        if (zone == a_destZone) { return; }
        CardZone prevZone = zone;
        prevZone?.Remove(this);
        zone = a_destZone;
        zone.Add(this);
    }
    public void Destroy()
    {
        zone.Remove(this);
        GameObject.Destroy(ui.gameObject);
    }

    //=============================================================================================
    // IModifiable Interface
    public void RemoveModifier(Modifier a_modifier)
    {
        if (a_modifier is CardModifier)
        {
            CardModifier mod = a_modifier as CardModifier;
            if (!_modifiers.Contains(mod)) { return; }
            _modifiers.Remove(mod);
            mod.Deactivate();
        }
    }
    public void AddModifier(Modifier a_modifier)
    {
        if (a_modifier is CardModifier)
        {
            CardModifier mod = a_modifier as CardModifier;
            if (_modifiers.Contains(mod)){ return; }
            _modifiers.Add(mod);
            mod.Activate();
            
        }
    }
    public void RemoveAllModifiers()
    {
        foreach (CardModifier mod in modifiers)
        {
            RemoveModifier(mod);
        }
    }


    //=============================================================================================
    // ITarget / ISource
    
    public void MarkChosenTarget() { ui.state = ITargetUI.State.SELECTED_TARGET; }
    public void MarkValidTarget() { ui.state = ITargetUI.State.VALID_TARGET; }
    public void ClearMarks() { ui.state = ITargetUI.State.DEFAULT; }
    public void MarkSource() { ui.state = ITargetUI.State.SOURCE; }
    public void MarkTribute() { ui.state = ITargetUI.State.TRIBUTE; }

    public void AddStatusEffect(StatusEffect.Name a_name, int a_stacks)
    {
        if (_statusEffects.ContainsKey(a_name) && _statusEffects[a_name].stackable)
        {
            _statusEffects[a_name].stacks += a_stacks;
        } else
        {
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
    public new abstract string ToString();


}
