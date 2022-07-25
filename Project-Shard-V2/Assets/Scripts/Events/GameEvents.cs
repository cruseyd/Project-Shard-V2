using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents
{
    public event Action onRefresh;
    public event Action onUIRefresh;
    public event Action<Card> onCardPlayed;
    public event Action<GamePhase> onEnterPhase;
    public event Action<GamePhase> onExitPhase;
    public event Action onConfirmAction;
    public event Action<UnitCard, Actor, List<IDamageable>> onModifyDefenders;
    public event Action<UnitCard, Actor, List<IDamageable>> onOverrideDefenders;
    public event Action<UnitCard, Actor, List<IDamageable>> onBypassDefenders;
    public void Refresh() { onRefresh?.Invoke(); }
    public void UIRefresh() { onUIRefresh?.Invoke(); }
    public void CardPlayed(Card a_card) { onCardPlayed?.Invoke(a_card); }
    public void EnterPhase(GamePhase a_phase) { onEnterPhase?.Invoke(a_phase); }
    public void ExitPhase(GamePhase a_phase) { onExitPhase?.Invoke(a_phase); }
    public void ModifyDefenders(UnitCard attacker, Actor actor, List<IDamageable> defenders)
    { onModifyDefenders?.Invoke(attacker, actor, defenders); }
    public void OverrideDefenders(UnitCard attacker, Actor actor, List<IDamageable> defenders)
    { onOverrideDefenders?.Invoke(attacker, actor, defenders); }
    public void BypassDefenders(UnitCard attacker, Actor actor, List<IDamageable> defenders)
    { onBypassDefenders?.Invoke(attacker, actor, defenders); }
    public void ConfirmAction() { onConfirmAction?.Invoke(); }
}
