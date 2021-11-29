using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEffect
{
    /// Sample Effects
    ///     Increment Stat
    ///     Move Card Between Zones (Draw, Discard, Put Into Play
    ///     Create / Destroy Card
    ///     Add / Remove Modifier
    ///     Add / Remove Status Effect
    public virtual void Execute(CardGame a_game)
    {
        //Debug.Log("EFFECT: " + this);
        a_game.currentAction.AddEffect(this);
    }
    public abstract void Undo(CardGame a_game);

    public abstract void Show(CardGame a_game);
}
