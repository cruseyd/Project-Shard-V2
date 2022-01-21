using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetQuery
{
    private ISource _source;

    // Card criteria
    public bool isCard = false;
    public List<CardZone.Type> zones;
    public List<Card.Type> cardTypes;
    public List<Keyword> keywords;
    public List<AbilityKeyword> abilityKeywords;

    // Actor criteria
    public bool isActor = false;
    public bool isOwner = false;
    public bool isOpponent = false;

    // Misc criteria
    public bool isEnemy = false;
    public bool isAlly = false;
    public bool isDamageable = false;

    public List<ITarget> exclude;

    public TargetQuery(ISource a_source)
    {
        _source = a_source;

        cardTypes = new List<Card.Type>();
        zones = new List<CardZone.Type>();
        exclude = new List<ITarget>();
        keywords = new List<Keyword>();
        abilityKeywords = new List<AbilityKeyword>();
    }

    public bool Compare(ITarget a_target)
    {
        // Misc checks
        if (_source != null)
        {
            if (isEnemy && (a_target.owner == _source.owner)) { return false; }
            if (isAlly && (a_target.owner != _source.owner)) { return false; }
        }
        if (exclude.Contains(a_target)) { return false; }
        
        if (isDamageable && !(a_target is IDamageable)) { return false; }

        if (a_target is Card)
        {
            Card card = a_target as Card;

            // Non card checks
            if (isActor  || isOwner || isOpponent) { return false; }

            // Card checks
            foreach (Keyword key in keywords)
            {
                if (!card.HasKeyword(key)) { return false; }
            }
            foreach (Card.Type type in cardTypes)
            {
                if (card.data.type == type) { break; }
                return false;
            }
            foreach (CardZone.Type type in zones)
            {
                if (card.zone.type == type) { break; }
                return false;
            }
            foreach (AbilityKeyword key in abilityKeywords)
            {
                if (card.HasKeyword(key)) { break; }
                return false;
            }
            return true;
        } else if (a_target is Actor)
        {
            Actor actor = a_target as Actor;

            // Non actor checks
            if (isCard) { return false; }
            if (cardTypes.Count > 0) { return false; }
            if (zones.Count > 0) { return false; }

            // Actor checks
            if (_source != null)
            {
                if (isOwner && (actor != _source.owner)) { return false; }
                if (isOpponent && (!_source.opponents.Contains(actor))) { return false; }
            }
            return true;
        } else
        {
            return false;
        }
    }
}
