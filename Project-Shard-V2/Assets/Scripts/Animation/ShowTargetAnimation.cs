using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTargetAnim : CardGameAnimation
{
    private Transform _source;
    private Transform _target;
    private CardGame _game;

    public ShowTargetAnim(CardGame a_game, Transform a_source, Transform a_target)
    {
        _source = a_source;
        _target = a_target;
        _game = a_game;
    }
    public override IEnumerator PlayNow()
    {
        _game.ui.EnableTargetEffects(_source);
        _game.ui.FreezeTargetEffects(_target);
        yield return new WaitForSeconds(CardGameParams.cardAnimationRate);
    }
}
