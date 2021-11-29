using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListTools
{
    public static void Permutations<T>(List<List<T>> a_permutations, List<T> a_list, int a_depth)
    {
        Permutations(a_permutations, a_list, 0, a_list.Count - 1, a_depth);
    }
    public static void Permutations<T>(List<List<T>> a_permutations, List<T> a_list, int l, int r, int d)
    {
        if (l == d)
        {
            List<T> p = new List<T>();
            for (int ii = 0; ii < d; ii++)
            {
                p.Add(a_list[ii]);
            }
            a_permutations.Add(p);
        } else
        {
            for (int ii = l; ii <= r; ii++)
            {
                Swap<T>(a_list, l, ii);
                Permutations(a_permutations, a_list, l + 1, r, d);
                Swap<T>(a_list, l, ii);
            }
        }
    }

    public static void Combinations<T>(List<List<T>> a_combinations, List<T> a_elements, int a_n)
    {
        int L = a_elements.Count;
        int[] combo = new int[a_n];
        for (int ii = 0; ii < Mathf.Pow(L, a_n); ii++)
        {
            List<T> list = new List<T>();
            for (int jj = 0; jj < a_n; jj++)
            {
                list.Add(a_elements[combo[jj]]);
            }
            a_combinations.Add(list);
            int index = 0;
            while (index < a_n)
            {
                if (combo[index] < L-1)
                {
                    combo[index]++;
                    break;
                } else
                {
                    combo[index] = 0;
                }
                index++;
            }
        }
    }

    public static void Swap<T>(List<T> a_list, int a, int b)
    {
        if (a == b) { return; }
        else
        {
            T tmp = a_list[a];
            a_list[a] = a_list[b];
            a_list[b] = tmp;
        }
    }

    public static void Print<T>(List<T> a_list)
    {
        string s = "";
        foreach (T element in a_list)
        {
            s += (element + ", ");
        }
        Debug.Log(s);
    }
}
