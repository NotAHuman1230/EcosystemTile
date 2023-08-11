using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Animal : MonoBehaviour
{

    [Header("Parameters")]
    [SerializeField] float mutationRate;
    [SerializeField] float valueMax;
    [SerializeField] float mutationMax;
    [SerializeField] List<Gene> genes = new List<Gene>();

    //Private references
    [HideInInspector] public AnimalManager manager;

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

    public void initialiseAnimal(Vector2Int _position)
    {
        position = _position;
        hunger = 50f;

        foreach(Gene gene in genes)
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

    //Update states
    public void deciding() 
    {
        hunger -= calulatedHungerUsage();
        if (hunger <= 0f)
            Destroy(gameObject);

        if (hunger >= getGeneValue("MatingDesire"))
            searchingMate();
        else
        {
            float foodType = Random.Range(0f, 1f);
            if (foodType < getGeneValue("Carnivory"))
                searchingAnimals();
            else
                searchingPlants();
        }
    }
    void searchingMate() 
    {
    
    }
    void searchingPlants() { }
    void searchingAnimals() { }
    void huntingPlants() { }
    void huntingAnimal() { }
    void mating() { }

    private void OnDestroy()
    {
        manager.animals.Remove(this);
    }
}
