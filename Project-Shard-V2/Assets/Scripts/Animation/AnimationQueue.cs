using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationQueue : MonoBehaviour
{
    private static AnimationQueue _instance;
    private Queue<CardGameAnimation> _sequencedAnimations;
    private bool _animating = false;

    void Awake()
    {
        if (_instance == null) { _instance = this; }
        _sequencedAnimations = new Queue<CardGameAnimation>();
    }

    void Update()
    {
        if (!_animating)
        {
            PlayNext();
        }
    }
    public static IEnumerator Finish()
    {
        _instance._animating = false;
        if (_instance._sequencedAnimations.Count == 0)
        {
            CombatManager.OrganizeAll();
        }
        yield return null;
    }
    public static void PlayNext()
    {
        if (_instance._sequencedAnimations.Count > 0)
        {
            _instance._animating = true;
            CardGameAnimation anim = _instance._sequencedAnimations.Dequeue();
            _instance.StartCoroutine(anim.Play());
        }
    }
    public static void PlayNow(CardGameAnimation a_anim)
    {
        _instance.StartCoroutine(a_anim.PlayNow());
    }
    public static void Add(CardGameAnimation a_anim)
    {
        if (_instance._sequencedAnimations.Contains(a_anim)) { return; }
        else if (a_anim is MoveCardAnim)
        {
            MoveCardAnim anim = a_anim as MoveCardAnim;
            // TODO: replace existing motion with the new one
        }
        _instance._sequencedAnimations.Enqueue(a_anim);
    }
}
