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

        StartCoroutine(delayedUpdate());
    }
    private void Update()
    {
        
    }

    IEnumerator delayedUpdate()
    {
        mapManager.updateFood();
        animalManager.updateAnimals();

        yield return new WaitForSeconds(updateDelay);
        StartCoroutine(delayedUpdate());
    }
}
