using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject cellPrefab;
    [SerializeField] Transform cellParent;

    [Header("Parameters")]
    [SerializeField] Vector2Int gridSize;
    [SerializeField] float cellSize;

    List<List<GameObject>> cells = new List<List<GameObject>>();

    private void Start()
    {
        transform.position = new Vector3(-gridSize.x * cellSize / 2f, -gridSize.y * cellSize / 2f, 0);

        generateGrid();
    }
    
    void generateGrid()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            cells.Add(new List<GameObject>());
            for (int x = 0; x < gridSize.x; x++)
            {
                GameObject instance = Instantiate(cellPrefab, transform.position, Quaternion.identity, cellParent);
                instance.transform.localScale = new Vector3(cellSize, cellSize, 1);
                instance.transform.position = new Vector3(transform.position.x + x * cellSize, transform.position.y + y * cellSize, 0);
                cells[y].Add(instance);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        for(int i = 0; i < gridSize.x + 1; i++)
        {
            Vector3 start = transform.position + new Vector3(i * cellSize, 0f, 0f);
            Vector3 end = transform.position + new Vector3(i * cellSize, gridSize.y * cellSize, 0f);
            Debug.DrawLine(start, end, Color.black);
        }

        for (int i = 0; i < gridSize.y + 1; i++)
        {
            Vector3 start = transform.position + new Vector3(0f, i * cellSize, 0f);
            Vector3 end = transform.position + new Vector3(gridSize.x * cellSize, i * cellSize, 0f);
            Debug.DrawLine(start, end, Color.black);
        }

    }
}
