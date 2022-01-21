using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public abstract class StatusEffect : Ability, ISource
{
    public enum Name
    {
        DEFAULT,
        // General status effects
        Poison = 1,
        Acumen,
        Alacrity,
        Frenzy,
        Chilled,
        Impaled,
        Frozen,
        Armor,
        Memorize,

        // Card specific status effects
        Sharpen = 1000

    }
    public List<Actor> opponents { get { return _game.Opponents(owner); } }
    public Actor owner { get; }
    public SourceEvents sourceEvents { get; private set; }
    public abstract bool stackable { get; }
    public readonly ITarget target;
    public int stacks;
    public abstract Name name { get; }

    public static StatusEffect Get(Name a_name, CardGame a_game, ITarget a_target)
    {
        string typeName = "SE_" + a_name.ToString();
        Type statusType = Type.GetType(typeName);
        ConstructorInfo constructorInfo = statusType.GetConstructor(new[] { typeof(CardGame), typeof(ITarget) });
        object[] args = { a_game, a_target };
        StatusEffect status = constructorInfo.Invoke(args) as StatusEffect;
        return status;
    }
    public static string Tooltip (Name a_name)
    {
        string text = "";
        switch (a_name)
        {
            case Name.Poison:
                text = "At the start of your turn, this takes 1 damage and loses 1 stack of POISON.";
                break;
            case Name.Acumen:
                text = "At the start of your turn, remove all stacks of ACUMEN and gain that much temporary FOCUS.";
                break;
            case Name.Alacrity:
                text = "While in hand, this card's cost is reduced by the number of stacks of ALACRITY.";
                break;
            case Name.Frenzy:
                text = "When you play an ACTION, remove 1 stack of FRENZY, take 1 damage and gain 1 temporary FOCUS.";
                break;
            case Name.Sharpen:
                return GameManager.cardIndex.Get("Sharpen").text;
            default: return "";
        }
        text = CardData.ParseSpecialWords(text);
        text = StatusEffect.ParseStatusEffects(text);
        return text;
    }
    public static string ParseStatusEffects(string a_string)
    {
        string output = a_string;
        foreach (string se in Enum.GetNames(typeof(StatusEffect.Name)))
        {
            output = output.Replace(se.ToUpper(), "<b>" + se + "</b>");
        }
        return output;
    }
    public StatusEffect(CardGame a_game, ITarget a_target) : base(a_game)
    {
        owner = a_target.owner;
        target = a_target;
        sourceEvents = new SourceEvents(this);
    }

    public abstract void Activate();
    public abstract void Deactivate();

    public void MarkSource()
    {
    }

    public void ClearMarks()
    {
    }
}
