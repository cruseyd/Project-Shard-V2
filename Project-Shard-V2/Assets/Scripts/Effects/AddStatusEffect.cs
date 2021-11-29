using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddStatusEffect : GameEffect
{
    private StatusEffect.Name _statusName;
    private ITarget _target;
    private int _stacks;

    public AddStatusEffect(StatusEffect.Name a_statusName, ITarget a_target, int a_stacks)
    {
        _statusName = a_statusName;
        _target = a_target;
        _stacks = a_stacks;
    }
    public override void Execute(CardGame a_game)
    {
        base.Execute(a_game);
        _target.AddStatusEffect(_statusName, _stacks);
    }
    public override void Show(CardGame a_game)
    {
    }

    public override void Undo(CardGame a_game)
    {
        _target.RemoveStatusEffect(_statusName, _stacks);
    }
}
