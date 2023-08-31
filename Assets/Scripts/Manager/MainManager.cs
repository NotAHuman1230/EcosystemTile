using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MapManager mapManager;
    [SerializeField] AnimalManager animalManager;

    [Header("Parameters")]
    [SerializeField] float updateDelay;

    private void Start()
    {
        mapManager.generateMap();
        animalManager.generateAnimals(mapManager.waterTexture, mapManager.desertTexture);

        //StartCoroutine(delay());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            delayedUpdate();
    }

    void delayedUpdate()
    {
        mapManager.updateFood();
        animalManager.updateAnimals(mapManager.foodTexutre);
    }

    IEnumerator delay()
    {
        mapManager.updateFood();
        animalManager.updateAnimals(mapManager.foodTexutre);

        yield return new WaitForSeconds(updateDelay);
        StartCoroutine(delay());
    }
}
