using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class Gene
{
    public string name;
    public float valueMax;
    public float mutationMax;
    public float energyCost;

    [HideInInspector] public float value;

    public Gene(string _name, float _value, float _valueMax, float _mutationMax, float _energyCost)
    {
        name = _name;
        value = _value;
        valueMax = _valueMax;
        mutationMax = _mutationMax;
        energyCost = _energyCost;
    }
}

