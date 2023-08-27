using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum Behaviour { dangerous, safe, mating }
public class Animal : MonoBehaviour
{

    [Header("Parameters")]
    [SerializeField] float mutationRate;
    [SerializeField] float valueMax;
    [SerializeField] float mutationMax;
    [SerializeField] int maxSightRange;
    [SerializeField] List<Gene> genes = new List<Gene>();

    //Public variables
    [HideInInspector] public AnimalManager manager;
    [HideInInspector] public Behaviour behaviour;

    //Base
    Vector2Int position;
    float hunger;

    float calulatedHungerUsage()
    {
        float hungerCost = 0f;
        foreach (Gene gene in genes)
            hungerCost += gene.value * gene.energyCost;

        return hungerCost;
    }
    void mutate()
    {
        int index = Random.Range(0, genes.Count);
        genes[index].value += Random.Range(-mutationMax, mutationMax);
    }

    public float getGeneValue(string _name)
    {
        foreach(Gene gene in genes)
        {
            if (gene.name == _name)
                return gene.value;
        }

        Debug.LogError("Gene value not found!");
        return 0f;
    }
    Vector2Int findClosestCell(Vector2Int _position, int _range)
    {
        int middle = ((_range - 3) / 2) + 1;
        float closestDistance = Mathf.Infinity;
        Vector2Int closestVector = Vector2Int.zero;

        for (int y = 0; y < _range; y++)
            for (int x = 0; x < _range; x++)
            {
                float distance = Vector2Int.Distance(_position, new Vector2Int(x + middle, y + middle));
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestVector = new Vector2Int(x, y);
                }
            }

        return closestVector;
    }
    public float[,] generateCellChances(List<Animal>[,] _surroundings, int _range)
    {
        float[,] chances = {{0, 0, 0 }, {0, 0, 0 }, {0, 0, 0 }};

        for (int y = 0; y < _surroundings.GetLength(0); y++)
            for (int x = 0; x < _surroundings.GetLength(1); x++)
            {
                float effect = 0f;

                for (int i = 0; i < _surroundings[y, x].Count; i++)
                {
                    if (_surroundings[y, x][i].behaviour == Behaviour.dangerous)
                        effect -= getGeneValue("PredatorAversion");
                    else if (_surroundings[y, x][i].behaviour == Behaviour.safe && behaviour == Behaviour.safe)
                        effect -= getGeneValue("CrowdAversion");
                    else if (_surroundings[y, x][i].behaviour == Behaviour.mating && behaviour == Behaviour.mating)
                        effect += getGeneValue("DesireBenefit");
                    else if(_surroundings[y, x][i].behaviour == Behaviour.safe && behaviour == Behaviour.dangerous)
                        effect += getGeneValue("DesireBenefit");
                }

                Vector2Int pos = new Vector2Int(x, y);
                Vector2Int closestCell = findClosestCell(pos, _range);
                float distance = Vector2Int.Distance(pos, closestCell);
                int furthestPoint = (_range - 3) / 2;
                float deteriation = distance / Vector2Int.Distance(new Vector2Int(0, 0), new Vector2Int(furthestPoint, furthestPoint));
                effect *= (1f - deteriation);

                chances[closestCell.y, closestCell.x] += effect;
            }

        return chances;
    }
    

    public void initialiseAnimal(Vector2Int _position)
    {
        position = _position;
        hunger = 50f;

        foreach (Gene gene in genes)
            gene.value = Random.Range(0f, valueMax);
    }
    public void born(Animal _father, Animal _mother, Vector2 _position)
    {
        for (int i = 0; i < genes.Count; i++)
        {
            float geneAttribution = Random.Range(0f, 1f);
            genes[i].value = geneAttribution > 0.5f ? _father.genes[i].value : _mother.genes[i].value;
        }

        float mutationChance = Random.Range(0f, 1f);
        if(mutationChance < mutationRate) mutate();
    }

    //State control
    public void pickBehaviour()
    {
        hunger -= calulatedHungerUsage();
        if (hunger <= 0f)
            Destroy(gameObject);

        if (hunger >= getGeneValue("MatingDesire") * 100f)
            behaviour = Behaviour.mating;
        else
        {
            float foodType = Random.Range(0f, 1f);
            if (foodType < getGeneValue("Carnivory"))
                behaviour = Behaviour.dangerous;
            else
                behaviour = Behaviour.safe;
        }
    }
    public void searching() 
    {
        int range = Mathf.RoundToInt(maxSightRange * getGeneValue("Sight"));
        float[,] chances = generateCellChances(manager.getSurroundings(position, range), range);

        for (int y = 0; y < chances.GetLength(0); y++)
            for (int x = 0; x < chances.GetLength(1); x++)
                Debug.Log(chances[y, x]);

    }
    void hunting() { }
    void mating() { }

    private void OnDestroy()
    {
        manager.death(this, position);
    }
}
