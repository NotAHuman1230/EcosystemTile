using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject animalPrefab;
    [SerializeField] Transform animalParent;

    [Header("Parameters")]
    [SerializeField] int animalAmount;

    List<GameObject> animals = new List<GameObject>();

    Vector2Int randomPosition(Texture2D _available)
    {
        Vector2Int position = new Vector2Int(Random.Range(0, _available.width), Random.Range(0, _available.height));
        while(_available.GetPixel(position.x, position.y).r >= 1f)
        {
            position = new Vector2Int(++position.x, position.y);
            if (position.x >= _available.width)
                position = new Vector2Int(0, position.y++);
                if (position.y >= _available.height)
                    position = new Vector2Int(0, 0);
        }

        return position;
    }

    public void generateAnimals(Texture2D _available)
    {
        for (int i = 0; i < animalAmount; i++)
        {
            Vector2Int position = randomPosition(_available);
            GameObject instance = Instantiate(animalPrefab, animalParent);
            instance.GetComponent<Animal>().initialiseAnimal(position);
            animals.Add(instance);
        }
    }
}
