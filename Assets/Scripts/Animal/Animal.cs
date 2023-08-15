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

    //TODO:
    //Enum for searching rather than sepearate methods
    //Get surroundings should involve stealth

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
    
    }
    void hunting() { }
    void mating() { }

    private void OnDestroy()
    {
        manager.death(this, position);
    }
}
