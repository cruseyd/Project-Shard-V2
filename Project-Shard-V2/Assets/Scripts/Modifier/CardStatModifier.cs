using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStatModifier : CardModifier
{
    public readonly CardStats.Name stat;
    public int value;

    public CardStatModifier(CardStats.Name a_stat, int a_value, CardGame a_game, ISource a_source, Card a_target, Duration a_duration)
    : base(a_game, a_source, a_target, a_duration)
    { 
        stat = a_stat;
        value = a_value;
    }
}
