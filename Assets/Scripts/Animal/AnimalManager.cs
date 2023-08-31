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
            if (animals[_start + i].getGene("Agility").value > animals[_mid + j + 1].getGene("Agility").value)
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
                Vector2Int pos = new Vector2Int(_position.x + x, _position.y + y);
                Vector2Int gridPos = new Vector2Int(x, y) + new Vector2Int(1, 1);

                if (pos.y >= animalCells.GetLength(0) || pos.x >= animalCells.GetLength(1)) continue;
                else if(pos.y < 0 || pos.x < 0) continue;

                surroundings[gridPos.y, gridPos.x] = new List<Animal>();
                for (int i = 0; i < animalCells[pos.y, pos.x].Count; i++)
                {
                    Gene stealth = animalCells[pos.y, pos.x][i].getGene("Stealth");
                    if (animalCells[pos.y, pos.x][i].behaviour != Behaviour.dangerous || Random.Range(stealth.boundryRange.x, stealth.boundryRange.y * 1.25f) >= stealth.value)
                        surroundings[gridPos.y, gridPos.x].Add(animalCells[pos.y, pos.x][i]);
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

        foreach (Animal animal in animals)
            animal.searching();
    }
}
