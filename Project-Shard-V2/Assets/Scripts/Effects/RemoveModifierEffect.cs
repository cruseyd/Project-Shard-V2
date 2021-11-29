using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveModifierEffect : GameEffect
{
    private Modifier _mod;
    public RemoveModifierEffect(Modifier a_mod)
    {
        _mod = a_mod;
    }
    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        _mod.target.RemoveModifier(_mod);
    }
    public override void Show(CardGame a_game)
    {

    }

    public override void Undo(CardGame a_game)
    {
        _mod.target.AddModifier(_mod);
    }
}