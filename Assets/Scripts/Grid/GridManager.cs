using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] Vector2Int gridSize;
    [SerializeField] float cellSize;

    Texture2D texture;
    List<List<GameObject>> cells = new List<List<GameObject>>();

    private void Start()
    {
        texture = new Texture2D(gridSize.x, gridSize.y);
        gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);

        generateGrid();
    }
    
    void generateGrid()
    {
        
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
