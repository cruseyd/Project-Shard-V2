using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardEvents
{
    private Card _card;

    public event Action<Card, Attempt> onCheckPlayable;
    public event Action<Card, Attempt> onCheckCanAttack;
    public event Action<Card> onPlay;
    public event Action<Card> onActivate;
    public event Action<Card, IDamageable> onAttack;
    public event Action<Card, IDamageable> onDeclareAttack;
    public event Action<Card, Card> onDefend;
    public event Action<Card> onDie;
    public event Action<Card, DamageData> onTakeDamage;
    public event Action<Card, DamageData> onDealDamage;
    public event Action<Card> onDraw;
    public event Action<Card> onLeavePlay;
    public event Action<Card> onEnterPlay;
    public event Action<Card> onCycle;
    public event Action<Card> onLeaveHand;
    public CardEvents(Card a_card) { _card = a_card; }
    public void CheckPlayable(Attempt a_attempt) { onCheckPlayable?.Invoke(_card, a_attempt); }
    public void CheckCanAttack(Attempt a_attempt) { onCheckCanAttack?.Invoke(_card, a_attempt); }
    public void Play() { onPlay?.Invoke(_card); }
    public void Activate() { onActivate?.Invoke(_card); }
    public void Attack(IDamageable a_target) { onAttack?.Invoke(_card, a_target); }
    public void DeclareAttack(IDamageable a_target) { onDeclareAttack?.Invoke(_card, a_target); }
    public void Defend(Card a_attacker) { onDefend?.Invoke(_card, a_attacker); }
    public void Draw() { onDraw?.Invoke(_card); }
    public void LeavePlay() { onLeavePlay?.Invoke(_card); }
    public void EnterPlay() { onEnterPlay?.Invoke(_card); }
    public void Cycle() { onCycle?.Invoke(_card); }
    public void LeaveHand() { onLeaveHand?.Invoke(_card); }
}
