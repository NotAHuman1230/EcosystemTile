using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class Gene
{
    public string name;
    public float energyCost;

    [HideInInspector] public float value;
}

