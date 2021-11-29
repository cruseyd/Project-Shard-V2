using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : MonoBehaviour
{
    public int L;
    void Start()
    {
        List<int> list = new List<int>();
        List<List<int>> listPerms = new List<List<int>>();
        for (int ii = 0; ii < L; ii++) { list.Add(ii); }
        ListTools.Combinations(listPerms, list, 2);
        foreach (List<int> p in listPerms)
        {
            ListTools.Print(p);
        }
    }
}
