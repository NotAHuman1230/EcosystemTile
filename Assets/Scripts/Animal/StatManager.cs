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
    [SerializeField] int population;
    [SerializeField] int starved;
    [SerializeField] int eaten;
    [SerializeField] List<GeneStats> stats = new List<GeneStats>();

    public void initialiseStats(List<Gene> _genes)
    {
        stats.Clear();
        for (int i = 0; i < _genes.Count; i++)
            stats.Add(new GeneStats(_genes[i].name));
    }
    public void calculateStats(List<Animal> _animals, int _population)
    {
        population = _population;

        for (int i = 0; i < stats.Count; i++)
        {
            float average = 0f;

            for (int n = 0; n < _animals.Count; n++)
            {
                average += _animals[n].getGeneList()[i].value;
            }

            stats[i].average = average / _animals.Count;
        }
    }
}
