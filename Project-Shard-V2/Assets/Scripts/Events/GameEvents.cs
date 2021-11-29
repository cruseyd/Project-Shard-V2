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
    public void Refresh() { onRefresh?.Invoke(); }
    public void UIRefresh() { onUIRefresh?.Invoke(); }
    public void CardPlayed(Card a_card) { onCardPlayed?.Invoke(a_card); }
    public void EnterPhase(GamePhase a_phase) { onEnterPhase?.Invoke(a_phase); }
    public void ExitPhase(GamePhase a_phase) { onExitPhase?.Invoke(a_phase); }
}
