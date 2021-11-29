using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter
{
    public ISource source { get; private set; }
    public List<ITarget> targets { get; set; }
    public Queue<TargetQuery> queries { get; private set; }
    public int minTargets { get; private set; }
    public Targeter(ISource a_source, List<TargetQuery> a_queries, int a_minTargets)
    {
        targets = new List<ITarget>();
        queries = new Queue<TargetQuery>();
        source = a_source;
        foreach (TargetQuery query in a_queries)
        {
            queries.Enqueue(query);
        }
        minTargets = a_minTargets;
    }

    public bool Add(ITarget a_target)
    {
        TargetQuery query = queries.Peek();
        if (query.Compare(a_target))
        {
            targets.Add(a_target);
            return true;
        }
        return false;
    }
}
