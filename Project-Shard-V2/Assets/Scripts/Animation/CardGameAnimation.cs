using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardGameAnimation
{
    public abstract IEnumerator PlayNow();

    public IEnumerator Play()
    {
        yield return PlayNow();
        yield return AnimationQueue.Finish();
    }
}
