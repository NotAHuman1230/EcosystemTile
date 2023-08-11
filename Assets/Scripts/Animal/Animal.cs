using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    [Header("Mutation Parameters")]
    [SerializeField] float mutationRate;

    [Header("Gene Parameters")]
    [SerializeField] float speedMax;
    [SerializeField] float speedMutationMax;
    [SerializeField] float agilityMax;
    [SerializeField] float agilityMutationMax;
    [SerializeField] float eyeSightMax;
    [SerializeField] float eyeSightMutationMax;
    [SerializeField] float offenseMax;
    [SerializeField] float offenseMutationMax;
    [SerializeField] float defenseMax;
    [SerializeField] float defenseMutationMax;
    [SerializeField] float matingDesireMax;
    [SerializeField] float matingDesireMutationMax;

    //Private references
    [HideInInspector] public AnimalManager manager;

    //Base
    Vector2Int position;
    float hunger;

    //Genes
    [HideInInspector] public float speed;
    [HideInInspector] public float agility;
    [HideInInspector] public float eyeSight;
    [HideInInspector] public float offense;
    [HideInInspector] public float defense;
    [HideInInspector] public float matingDesire;

    void mutate()
    {
        int type = Random.Range(0, 5);
        switch (type)
        {
            case 0: speed += Random.Range(-speedMutationMax, speedMutationMax); speed = Mathf.Clamp(speed, 0f, Mathf.Infinity); break;
            case 1: agility += Random.Range(-agilityMutationMax, agilityMutationMax); agility = Mathf.Clamp(agility, 0f, Mathf.Infinity); break;
            case 2: eyeSight += Random.Range(-eyeSightMutationMax, eyeSightMutationMax); eyeSight = Mathf.Clamp(eyeSight, 0f, Mathf.Infinity); break;
            case 3: offense += Random.Range(-offenseMutationMax, offenseMutationMax); offense = Mathf.Clamp(offense, 0f, Mathf.Infinity); break;
            case 4: defense += Random.Range(-defenseMutationMax, defenseMutationMax); defense = Mathf.Clamp(defense, 0f, Mathf.Infinity); break;
            case 5: matingDesire += Random.Range(-matingDesireMutationMax, matingDesireMutationMax); matingDesire = Mathf.Clamp(matingDesire, 0f, Mathf.Infinity); break;
        }
    }

    public float getAgility() { return agility; }

    public void stateUpdate()
    {

    }
    public void initialiseAnimal(Vector2Int _position)
    {
        position = _position;
        hunger = 50f;

        speed = Random.Range(0f, speedMax);
        agility = Random.Range(0f, agilityMax);
        eyeSight = Random.Range(0f, eyeSightMax);
        offense = Random.Range(0f, offenseMax);
        defense = Random.Range(0f, defenseMax);
        matingDesire = Random.Range(0f, matingDesireMax);
    }
    public void born(Animal _father, Animal _mother, Vector2 _position)
    {
        float geneAttribution = Random.Range(0f, 1f);
        agility = geneAttribution > 0.5f ? _mother.agility : _father.agility;

        geneAttribution = Random.Range(0f, 1f);
        speed = geneAttribution > 0.5f ? _mother.speed : _father.speed;

        geneAttribution = Random.Range(0f, 1f);
        eyeSight = geneAttribution > 0.5f ? _mother.eyeSight : _father.eyeSight;

        geneAttribution = Random.Range(0f, 1f);
        offense = geneAttribution > 0.5f ? _mother.offense : _father.offense;

        geneAttribution = Random.Range(0f, 1f);
        defense = geneAttribution > 0.5f ? _mother.defense : _father.defense;

        geneAttribution = Random.Range(0f, 1f);
        matingDesire = geneAttribution > 0.5f ? _mother.matingDesire : _father.matingDesire;

        geneAttribution = Random.Range(0f, 1f);
        if(geneAttribution < mutationRate) mutate();
    }

    //Update states
    void deciding() { }
    void searchingFood() { }
    void searchingMate() { }
    void huntingPlants() { }
    void huntingAnimal() { }
    void mating() { }

}
