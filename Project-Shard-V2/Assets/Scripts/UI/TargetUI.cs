using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetUI
{
    public enum State
    {
        DEFAULT,
        PLAYABLE,
        SOURCE,
        VALID_TARGET,
        SELECTED_TARGET,
        TRIBUTE
    }

    public State state { get; set; }
    public ITarget targetData { get; }
    public GameObject gameObject { get; }

    public void Refresh();
}
