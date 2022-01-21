using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAOECardStatModifierEffect : GameEffect
{
    private int _prevValue;
    private int _newValue;
    private AOECardStatModifier _mod;
    public ChangeAOECardStatModifierEffect(AOECardStatModifier a_mod, int a_value)
    {
        _mod = a_mod;
        _prevValue = a_mod.value;
        _newValue = a_value;
    }
    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        _mod.value = _newValue;
    }
    public override void Show(CardGame a_game)
    {

    }

    public override void Undo(CardGame a_game)
    {
        _mod.value = _prevValue;
    }
}
