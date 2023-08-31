using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject animalPrefab;
    [SerializeField] Transform animalParent;

    [Header("Parameters")]
    [SerializeField] int animalAmount;

    List<Animal> animals = new List<Animal>();
    [HideInInspector] public List<Animal>[,] animalCells;
    [HideInInspector] public Texture2D water;
    [HideInInspector] public Texture2D desert;
    [HideInInspector] public Texture2D food;

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
            if (animals[_start + i].getGeneValue("Agility") > animals[_mid + j + 1].getGeneValue("Agility"))
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

    public List<Animal>[,] getSurroundings(Vector2Int _position)
    {
        List<Animal>[,] surroundings = new List<Animal>[3, 3];
        for (int y = -1; y < 2; y++)
            for (int x = -1; x < 2; x++)
            {
                if (y + _position.y >= water.height || x + _position.x >= water.width)
                    continue;
                else if(y + _position.y < 0 || x + _position.x < 0)
                    continue;

                surroundings[y + 1, x + 1] = new List<Animal>();
                for (int i = 0; i < animalCells[_position.y + y, _position.x + x].Count; i++)
                {
                    if(surroundings[y, x][i].behaviour != Behaviour.dangerous || Random.Range(0f, 1.5f) >= surroundings[y, x][i].getGeneValue("Stealth") && surroundings[y, x][i].behaviour == Behaviour.dangerous)
                        surroundings[y, x].Add(animalCells[_position.y + y, _position.x + x][i]);
                }
            }

        return surroundings;
    }

    public void reproduction(Animal _father, Animal _mother, Vector2Int _position)
    {
        GameObject instance = Instantiate(animalPrefab, animalParent);
        Animal instanceScript = instance.GetComponent<Animal>();
        instanceScript.born(_father, _mother, _position);
        animals.Add(instanceScript);
        animalCells[_position.y, _position.x].Add(instanceScript);
    }
    public void death(Animal _self, Vector2Int _positon)
    {
        animals.Remove(_self);
        animalCells[_positon.y, _positon.x].Remove(_self);
    }

    public void generateAnimals(Texture2D _water, Texture2D _desert)
    {
        water = _water;
        desert = _desert;

        animalCells = new List<Animal>[water.height, water.width];
        for (int y = 0; y < water.height; y++)
            for (int x = 0; x < water.width; x++)
                animalCells[y, x] = new List<Animal>();

        for (int i = 0; i < animalAmount; i++)
        {
            Vector2Int position = randomPosition(_water);
            GameObject instance = Instantiate(animalPrefab, animalParent);
            Animal instanceScript = instance.GetComponent<Animal>();
            instanceScript.initialiseAnimal(position);
            instanceScript.manager = this;
            animals.Add(instanceScript);
            animalCells[position.y, position.x].Add(instanceScript);
        }
    }
    public void updateAnimals(Texture2D _food)
    {
        Debug.Log("Population: " + animals.Count);

        food = _food;

        foreach (Animal animal in animals)
            animal.pickBehaviour();

        if(animals.Count > 1)
            mergeSortAnimals(0, animals.Count - 1);

        //Todo bug in animal behaviour methods
        //Index out of range
        //Out of map range when finding chances
        //Negatives chances are wrong way round
        //Do complete redo of chances where rather than normal distance, use cell distance
        foreach (Animal animal in animals)
            animal.searching();
    }
}
