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

    List<Animal> animals = new List<Animal>();

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

    void mergeSortAnimals(int _start, int _end)
    {
        int mid = (_end + _start) / 2;

        if (_end - _start > 1)
        {
            mergeSortAnimals(_start, mid);
            mergeSortAnimals(mid + 1, _end);
        }
        sortAnimals(_start, mid, _end);
    }
    void sortAnimals(int _start, int _mid, int _end)
    {
        int arrOneAmount = _mid - _start + 1;
        int arrTwoAmount = _end - _mid;

        List<Animal> list = new List<Animal>();
        int i = 0;
        int j = 0;
        
        while(arrOneAmount > i && arrTwoAmount > j)
        {
            if (animals[_start + i].getAgility() > animals[_mid + j + 1].getAgility())
            {
                list.Add(animals[_start + i]);
                i++;
            }
            else
            {
                list.Add(animals[_mid + j + 1]);
                j++;
            }
        }

        while(arrOneAmount > i) { list.Add(animals[_start + i]); i++; }
        while(arrTwoAmount > j) { list.Add(animals[_mid + j + 1]); j++; }

        for (int k = 0; k < list.Count; k++) { animals[_start + k] = list[k]; }
    }

    public void reproduction(Animal _father, Animal _mother, Vector2Int _position)
    {
        GameObject instance = Instantiate(animalPrefab, animalParent);
        Animal instanceScript = instance.GetComponent<Animal>();
        instanceScript.born(_father, _mother, _position);
        animals.Add(instanceScript);
    }

    public void generateAnimals(Texture2D _available)
    {
        for (int i = 0; i < animalAmount; i++)
        {
            Vector2Int position = randomPosition(_available);
            GameObject instance = Instantiate(animalPrefab, animalParent);
            Animal instanceScript = instance.GetComponent<Animal>();
            instanceScript.initialiseAnimal(position);
            animals.Add(instanceScript);
        }
    }
    public void animalsUpdate()
    {
        mergeSortAnimals(0, animalAmount - 1);
    }
}
