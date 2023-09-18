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
        starved = 0;
        eaten = 0;

        resetAverages();

        for (int i = 0; i < _animals.Count; i++)
        {
            if (_animals[i].behaviour == Behaviour.starved) starved++;
            else if (_animals[i].behaviour == Behaviour.eaten) eaten++;

            for (int n = 0; n < stats.Count; n++)
            {
                stats[n].average += _animals[i].getGeneList()[n].value;
            }
        }

        divideAverages(_animals.Count);
    }

    void resetAverages()
    {
        foreach(GeneStats geneStat in stats)
            geneStat.average = 0f;
    }
    void divideAverages(int _count)
    {
        foreach (GeneStats geneStat in stats)
            geneStat.average /= _count;
    }
}
