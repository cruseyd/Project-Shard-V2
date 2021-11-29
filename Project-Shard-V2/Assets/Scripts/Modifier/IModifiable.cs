using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifiable
{
    void RemoveModifier(Modifier a_modifier);
    void AddModifier(Modifier a_modifier);
    void RemoveAllModifiers();
}
