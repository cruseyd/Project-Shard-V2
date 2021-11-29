using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISource
{
    public List<Actor> opponents { get; }
    public Actor owner { get; }
    public SourceEvents sourceEvents { get; }
    public void MarkSource();
    public void ClearMarks();
}
