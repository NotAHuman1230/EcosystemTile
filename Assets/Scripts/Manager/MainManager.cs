using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MapManager mapManager;
    [SerializeField] AnimalManager animalManager;
    [SerializeField] StatManager statManager;
    [SerializeField] Animal animalPrefab;

    [Header("Debug")]
    [SerializeField] float updateDelay;
    [SerializeField] bool manualUpdate;

    private void Start()
    {
        mapManager.generateMap();
        animalManager.generateAnimals(mapManager.waterTexture, mapManager.desertTexture);
        statManager.initialiseAverages(animalPrefab.getGeneList());

        StartCoroutine(delay());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && manualUpdate)
            delayedUpdate();
    }

    void delayedUpdate()
    {
        mapManager.updateFood();
        animalManager.updateAnimals(mapManager.foodTexutre);
        statManager.calculateAverages(animalManager.geneTotals, animalManager.animals.Count);
    }

    IEnumerator delay()
    {
        if (!manualUpdate)
            delayedUpdate();

        yield return new WaitForSeconds(updateDelay);

        StartCoroutine(delay());
    }
}
