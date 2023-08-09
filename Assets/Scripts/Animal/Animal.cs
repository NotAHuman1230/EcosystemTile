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

    //Base
    Vector2Int position;
    float hunger;

    //Genes
    float speed;
    float agility;
    float eyeSight;
    float offense;
    float defense;
    float matingDesire;

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
}
