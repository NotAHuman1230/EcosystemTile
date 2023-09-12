using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneStats
{
    public string name;
    public float average;

    public GeneStats(string _name) { name = _name; average = 0f; }
}
public class StatManager : MonoBehaviour
{
    [SerializeField] List<GeneStats> stats = new List<GeneStats>();

    public void initialiseAverages(List<Gene> _genes)
    {
        stats.Clear();
        for (int i = 0; i < _genes.Count; i++)
            stats.Add(new GeneStats(_genes[i].name));
    }
    public void calculateAverages(List<float> _totals, int population)
    {
        for (int i = 0; i < _totals.Count; i++)
            stats[i].average = _totals[i] / population;
    }
}
