using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gene
{
    public string name;
    public float energyCost;
    public float mutationRange;
    public Vector2 boundryRange;

    [HideInInspector] public float value;
}

