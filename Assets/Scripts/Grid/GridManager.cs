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

    [SerializeField] Texture2D texture;
    new SpriteRenderer renderer;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        texture = generateGrid();
        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1f / cellSize);
        renderer.sprite = sprite;
        Debug.Log(renderer.sprite.texture.GetPixel(100, 100));
    }
    
    Texture2D generateGrid()
    {
        RenderTexture rw = new RenderTexture(gridSize.x, gridSize.y, 0);
        rw.enableRandomWrite = true;
        rw.Create();

        computeShader.SetInts("resolution", gridSize.x, gridSize.y);
        computeShader.SetTexture(computeShader.FindKernel("PerlinNoise"), "result", rw);
        computeShader.Dispatch(computeShader.FindKernel("PerlinNoise"), gridSize.x / 8, gridSize.y / 8, 1);
        //Debug.Log("hi");
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
