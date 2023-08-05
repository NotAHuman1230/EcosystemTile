using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ComputeShader computeShader;

    [Header("Parameters")]
    [SerializeField] Vector2Int gridSize;
    [SerializeField] float cellSize;

    [Header("Perlin Noise")]
    [SerializeField] int perlinCellSize;
    [SerializeField] float perlinIntensity;

    Texture2D texture;

    private void Start()
    {
        texture = generateGrid();
        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1f / cellSize);
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
    
    Texture2D generateGrid()
    {
        RenderTexture rw = new RenderTexture(gridSize.x, gridSize.y, 0);
        rw.enableRandomWrite = true;
        rw.Create();

        computeShader.SetInts("resolution", gridSize.x, gridSize.y);
        computeShader.SetInt("gridSize", perlinCellSize);
        computeShader.SetFloat("seed", Random.Range(0f, 1f));
        computeShader.SetFloat("intensity", perlinIntensity);

        computeShader.SetTexture(computeShader.FindKernel("PerlinNoise"), "result", rw);
        computeShader.Dispatch(computeShader.FindKernel("PerlinNoise"), (int)Mathf.Ceil(gridSize.x / 8f), (int)Mathf.Ceil(gridSize.y / 8f), 1);

        Texture2D tex = new Texture2D(gridSize.x, gridSize.y);
        RenderTexture.active = rw;
        tex.ReadPixels(new Rect(0f, 0f, rw.width, rw.height), 0, 0);
        tex.Apply();
        return tex;
    }

    private void OnDrawGizmosSelected()
    {
        
    }
}
