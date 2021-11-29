using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCard : Card, IDamageable
{
    public DamageableEvents damageEvents { get; private set; }
    public int health { get { return GetStat(StatName.HEALTH); } }
    public int maxHealth { get { return GetStat(StatName.MAX_HEALTH); } }
    public int power { get { return GetStat(StatName.POWER); } }
    public bool canAttack
    {
        get
        {
            if (!_game.IsActorTurn(owner)) { return false; }
            if (zone.type != CardZone.Type.ACTIVE) { return false; }
            if (numActions <= 0) { return false; }
            if (playedThisTurn)
            {
                if (!HasKeyword(AbilityKeyword.SWIFT)) { return false; }
            }
            Attempt attempt = new Attempt();
            events.CheckCanAttack(attempt);
            return attempt.success;
        }
    }
    public UnitCard(CardGame a_game, CardData a_data, Actor a_actor) : base(a_game, a_data, a_actor)
    {
        Initialize();
        damageEvents = new DamageableEvents(this);
        
    }
    public override CardUI Spawn(Vector3 a_spawnPosition, CardZoneUI a_zone)
    {
        GameObject cardGO = GameObject.Instantiate(CardGameParams.GetCardPrefab(Card.Type.FOLLOWER),
            a_spawnPosition, Quaternion.identity) as GameObject;
        ui = cardGO.GetComponent<CardUI>();
        ui.Initialize(this);
        ui.Move(a_zone, false, false);
        _game.events.onUIRefresh += ui.Refresh;
        events.onLeavePlay += (_card_) =>
        {
            ResetCardEffect effect = new ResetCardEffect(_card_);
            effect.Execute(_game);
        };
        return cardGO.GetComponent<CardUI>();
    }
    public override void Refresh()
    {
        base.Refresh();
    }
    public override string ToString()
    {
        string output = data.name;
        output += " | level: " + GetStat(StatName.LEVEL);
        output += " | power: " + GetStat(StatName.POWER);
        output += " | health: " + GetStat(StatName.HEALTH) + "/" + GetStat(StatName.MAX_HEALTH);
        output += " | defiance: " + GetStat(StatName.DEFIANCE);
        return output;
    }
    public override void Initialize()
    {
        base.Initialize();
        _stats[StatName.HEALTH] = data.health;
        _stats[StatName.MAX_HEALTH] = data.health;
        _stats[StatName.POWER] = data.power;
        _stats[StatName.LEVEL] = data.level;
        _stats[StatName.DEFIANCE] = data.defiance;
    }

   
    //=============================================================================================
    // IDamageable
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
        a_damageData.locked = true; //if the data is altered after this, undo wont work
        
        if (a_damageData.damage > 0)
        {
            a_damageData.source.sourceEvents.DealDamage(a_damageData);
            damageEvents.TakeDamage(a_damageData);
            a_damageData.source.owner.events.DealDamage(a_damageData);
        } else if (a_damageData.damage < 0)
        {
            a_damageData.source.sourceEvents.Heal(a_damageData);
            damageEvents.Heal(a_damageData);
        }
    }
    
    public void SetHealth(int a_value)
    {
        SetStat(StatName.HEALTH, a_value);
    }
}
