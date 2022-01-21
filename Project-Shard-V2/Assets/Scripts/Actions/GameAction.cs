using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameAction
{
    /// Example Actions:
    ///     Play Card
    ///     Declare Attack
    ///     Use Ability
    ///     Mulligan
    ///     Choose Option
    ///     End Turn
    
    private Stack<GameEffect> _result;
    public abstract Actor actor { get; }
    public bool success = true;
    public bool executing = false;
    public bool previewForAI = true;
    public GameAction()
    {
        _result = new Stack<GameEffect>();
    }
    public abstract void Execute(CardGame a_game);
    public abstract bool IsValid(CardGame a_game);
    public virtual void Undo(CardGame a_game)
    {
        while (_result.Count > 0)
        {
            GameEffect effect = _result.Pop();
            effect.Undo(a_game);
        }
    }
    public void AddEffect(GameEffect a_effect)
    {
        if (executing)
        {
            _result.Push(a_effect);
        } else
        {
            Debug.Log("GameAction::AddEffect | Warning: Tried to add effect while not executing.");
        }
        
    }
    public void Show(CardGame a_game)
    {
        foreach (GameEffect effect in _result)
        {
            effect.Show(a_game);
        }
    }

    public abstract void Preview(CardGame a_game);

    public void PrintEffects()
    {
        foreach (GameEffect effect in _result)
        {
            Debug.Log(effect);
        }
    }
}
