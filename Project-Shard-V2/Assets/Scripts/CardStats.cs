using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStats
{
    public enum Name
    {
        DEFAULT,
        LEVEL,
        POWER,
        HEALTH,
        MAX_HEALTH,
        DEFIANCE
    }

    private Dictionary<Name, int> _stats;
    private List<CardModifier> _modifiers;
    private Card _card;

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

    public CardStats(Card a_card = null)
    {
        _card = a_card;
        _stats = new Dictionary<Name, int>();
        _modifiers = new List<CardModifier>();
    }
    public CardStats(CardStats a_stats)
    {
        _card = a_stats._card;
        _stats = new Dictionary<Name, int>();
        _modifiers = a_stats.modifiers;

        foreach (Name stat in a_stats._stats.Keys)
        {
            _stats[stat] = a_stats._stats[stat];
        }
    }
    public int Get(Name a_name)
    {
        if (!_stats.ContainsKey(a_name)) { return 0; }
        int value = _stats[a_name];
        foreach (CardModifier mod in _modifiers)
        {
            if (mod is CardStatModifier)
            {
                CardStatModifier m = mod as CardStatModifier;
                if (m.stat == a_name)
                {
                    value += m.value;
                }
            }
        }
        if (_card != null)
        {
            foreach (ActorCardStatModifier mod in _card.owner.cardStatModifiers)
            {
                if (mod.stat == a_name && mod.Compare(_card))
                {
                    value += mod.value;
                }
            }
        }
        return value;
    }
    public void Set(Name a_name, int a_value)
    {
        _stats[a_name] = a_value;
    }

    public void Increment(Name a_name, int a_delta)
    {
        if (_stats.ContainsKey(a_name))
        {
            _stats[a_name] += a_delta;
        }
    }
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
            if (_modifiers.Contains(mod)) { return; }
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
}
