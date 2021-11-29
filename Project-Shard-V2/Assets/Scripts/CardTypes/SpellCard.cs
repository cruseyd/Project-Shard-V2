using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : Card
{
    public SpellCard(CardGame a_game, CardData a_data, Actor a_actor) : base(a_game, a_data, a_actor)
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();
        _stats[StatName.LEVEL] = data.level;
    }

    public override CardUI Spawn(Vector3 a_spawnPosition, CardZoneUI a_zone)
    {
        GameObject cardGO = GameObject.Instantiate(CardGameParams.GetCardPrefab(Card.Type.ACTION),
            a_spawnPosition, Quaternion.identity);
        ui = cardGO.GetComponent<CardUI>();
        ui.Initialize(this);
        _game.events.onUIRefresh += ui.Refresh;
        events.onLeavePlay += (_card_) =>
        {
            ResetCardEffect effect = new ResetCardEffect(_card_);
            effect.Execute(_game);
        };
        ui.Move(a_zone, false, false);
        return cardGO.GetComponent<CardUI>();
    }

    public override string ToString()
    {
        return data.name + " | level: " + GetStat(StatName.LEVEL);
    }
}
