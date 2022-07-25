using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Keyword
{
    T_SPELL,
    T_TECHNIQUE,
    E_FIRE,
    E_ICE,
    E_WATER,
    E_LIGHTNING,
    E_EARTH,
    E_WIND,
    E_POISON,
    E_LIGHT,
    E_DARK,
    E_ARCANE,
    E_MYSTIC,
    E_ELDER,
    E_SLASHING,
    E_PIERCING,
    E_CRUSHING,
    C_WARRIOR,
    C_MAGE,
    C_CLERIC,
    C_ROGUE,
    C_RANGER,
    R_HUMAN,
    R_HYROC,
    R_DRYAD,
    S_WOLF,
    S_DRAGON,
    S_SLIME,
    S_PLANT,
    S_TARANZID,
    S_FAIRY
}

public enum AbilityKeyword
{
    DEFAULT = 0,

    SWIFT = 1,
    NIMBLE,
    GUARDIAN,
    EPHEMERAL,
    PASSIVE,
    EVASIVE,
    OVERWHELM,
    FRAGILE,
    RUSH,
    ARMOR
}

public enum ActionKeyword
{
    DEFAULT,
    HEAL,
    ABILITY,
    ATTACK,
    DEFEND,
    PLAY,
    CHANNEL,
    CYCLE,
    DRAW,
    SUMMON,
    DESTROY
}
