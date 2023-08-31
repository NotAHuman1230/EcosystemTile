using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum Behaviour { dangerous, safe, mating }
public class Animal : MonoBehaviour
{
    [Header("Diet")]
    [SerializeField] float plantEnergy;
    [SerializeField] float meatEnergy;

    [Header("Genes")]
    [SerializeField] float mutationRate;
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
        genes[index].value += Random.Range(-genes[index].mutationRange, genes[index].mutationRange);
        genes[index].value = Mathf.Clamp(genes[index].value, genes[index].boundryRange.x, genes[index].boundryRange.y);
    }

    public Gene getGene(string _name)
    {
        foreach(Gene gene in genes)
        {
            if (gene.name == _name)
                return gene;
        }

        Debug.LogError("Gene value not found!");
        return genes[0];
    }
    public float[,] generateCellChances(List<Animal>[,] _surroundings)
    {
        float[,] chances = {{0, 0, 0 }, {0, 0, 0 }, {0, 0, 0 }};
        
        for (int y = 0; y < _surroundings.GetLength(0); y++)
            for (int x = 0; x < _surroundings.GetLength(1); x++)
            {
                float effect = 0f;

                for (int i = 0; i < _surroundings[y, x].Count; i++)
                {
                    if (_surroundings[y, x][i].behaviour == Behaviour.dangerous)
                        effect -= getGene("PredatorAversion").value;
                    else if (_surroundings[y, x][i].behaviour == Behaviour.safe && behaviour == Behaviour.safe)
                        effect -= getGene("CrowdAversion").value;
                    else if (_surroundings[y, x][i].behaviour == Behaviour.mating && behaviour == Behaviour.mating)
                        effect += getGene("DesireBenefit").value;
                    else if(_surroundings[y, x][i].behaviour == Behaviour.safe && behaviour == Behaviour.dangerous)
                        effect += getGene("DesireBenefit").value;
                }

                chances[y, x] = effect;
            }

        return chances;
    }
    Vector2Int generateDirection(float[,] _chances)
    {
        float smallest = Mathf.Infinity;
        float biggest = -Mathf.Infinity;
        bool isNegative = true;
        for (int y = 0; y < _chances.GetLength(0); y++)
            for (int x = 0; x < _chances.GetLength(1); x++)
            {
                if(biggest < _chances[y, x])
                    biggest = _chances[y, x];
                else if (smallest > _chances[y, x])
                    smallest = _chances[y, x];

                if (_chances[y, x] >= 0)
                    isNegative = false;
            }

        float total = 0f;
        for (int y = 0; y < _chances.GetLength(0); y++)
            for (int x = 0; x < _chances.GetLength(1); x++)
            {
                if (isNegative)
                    _chances[y, x] = -biggest + _chances[y, x];
                else
                    _chances[y, x] = _chances[y, x] - smallest;
            }

        

        float chance = Random.Range(0f, total);
        for (int y = 0; y < _chances.GetLength(0); y++)
            for (int x = 0; x < _chances.GetLength(1); x++)
            {
                chance -= _chances[y, x];
                if (chance <= 0f)
                    return new Vector2Int(y, x) - new Vector2Int(1, 1);
            }

        Debug.LogError("Choosing direction has led to uncertainty!!!");
        return new Vector2Int(0, 0);
    }

    public void initialiseAnimal(Vector2Int _position)
    {
        position = _position;
        hunger = 50f;

        foreach (Gene gene in genes)
            gene.value = Random.Range(gene.boundryRange.x, gene.boundryRange.y);
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

        if (hunger >= getGene("MatingDesire").value * (100f / getGene("MatingDesire").boundryRange.y))
            behaviour = Behaviour.mating;
        else
        {
            Gene carnivory = getGene("Carnivory");
            float foodType = Random.Range(carnivory.boundryRange.x, carnivory.boundryRange.y);
            if (foodType < carnivory.value)
                behaviour = Behaviour.dangerous;
            else
                behaviour = Behaviour.safe;
        }
    }
    public void searching() 
    {
        float[,] chances = generateCellChances(manager.getSurroundings(position));

        Vector2Int direction = generateDirection(chances);
        position += direction;

        switch (behaviour)
        {
            case Behaviour.dangerous:
                huntingMeat();
                break;
            case Behaviour.safe:
                huntingPlants();
                break;
            case Behaviour.mating:
                mating();
                break;
            default:
                Debug.LogError("Behaviour not found!");
                break;
        }
    }
    void huntingMeat() 
    {
        if (manager.animalCells[position.y, position.x].Count == 0)
            return;

        int victimIndex = Random.Range(0, manager.animalCells[position.y, position.x].Count);
        Animal victim = manager.animalCells[position.y, position.x][victimIndex];

        float huntSuccess = Random.Range(0f, getGene("Offence").value + victim.getGene("Defence").value);
        if(huntSuccess < getGene("Offence").value)
        {
            hunger = Mathf.Clamp(hunger + meatEnergy, 0f, 100f);
            Destroy(victim.gameObject);
        }

    }
    void huntingPlants() 
    {
        if (manager.food.GetPixel(position.x, position.y).r == 0f)
            return;

        hunger = Mathf.Clamp(hunger + plantEnergy, 0f, 100f);
    }
    void mating() 
    {
        List<Animal> matePossibilites = new List<Animal>();
        foreach (Animal animal in manager.animalCells[position.y, position.x])
            if (animal.behaviour == Behaviour.mating)
                matePossibilites.Add(animal);
        if (matePossibilites.Count == 0)
            return;

        Animal mate = matePossibilites[Random.Range(0, matePossibilites.Count)];
        manager.reproduction(this, mate, position);
    }

    private void OnDestroy()
    {
        manager.death(this, position);
    }
}
