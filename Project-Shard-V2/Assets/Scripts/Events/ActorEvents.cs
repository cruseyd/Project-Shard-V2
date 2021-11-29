using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActorEvents
{
    private Actor _actor;

    public event Action<Actor> onStartTurn;
    public event Action<Actor> onEndTurn;
    public event Action<Actor, Card> onPlayCard;
    public event Action<Actor, Card> onDrawCard;
    public event Action<Actor, DamageData> onBeforeDealDamage;
    public event Action<Actor, DamageData, Attempt> onTryDealDamage;
    public event Action<Actor, DamageData> onDealDamage;

    public ActorEvents(Actor a_actor) { _actor = a_actor; }

    public void StartTurn() { Debug.Log("START TURN");  onStartTurn?.Invoke(_actor); }
    public void EndTurn() { Debug.Log("END TURN"); onEndTurn?.Invoke(_actor); }
    public void PlayCard(Card a_card) { onPlayCard?.Invoke(_actor, a_card); }
    public void DrawCard(Card a_card) { onDrawCard?.Invoke(_actor, a_card); }

    public void BeforeDealDamage(DamageData a_data) { onBeforeDealDamage?.Invoke(_actor, a_data); }
    public void TryDealDamage(DamageData a_data, Attempt a_attempt) { onTryDealDamage?.Invoke(_actor, a_data, a_attempt); }
    public void DealDamage(DamageData a_data) { onDealDamage?.Invoke(_actor, a_data); }
}
