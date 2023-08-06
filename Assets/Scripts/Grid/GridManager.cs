using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ComputeShader noiseCompute;
    [SerializeField] ComputeShader visualCompute;

    [Header("Map")]
    [SerializeField] Vector2Int gridSize;
    [SerializeField] float cellSize;
    [SerializeField] float waterFactor;

    [Header("Perlin Noise")]
    [SerializeField] int perlinCellSize;
    [SerializeField] float perlinIntensity;

    Texture2D humidityTexture;
    Texture2D temperatureTexutre;

    private void Start()
    {
        generateMap();
    }
    
    Texture2D renderTexTo2D(RenderTexture _renderTexture)
    {
        Texture2D tex = new Texture2D(gridSize.x, gridSize.y);
        RenderTexture.active = _renderTexture;
        tex.ReadPixels(new Rect(0f, 0f, _renderTexture.width, _renderTexture.height), 0, 0);
        tex.Apply();
        return tex;
    }
    RenderTexture generatePerlinNoise()
    {
        RenderTexture rw = new RenderTexture(gridSize.x, gridSize.y, 0);
        rw.enableRandomWrite = true;
        rw.Create();

        noiseCompute.SetInts("resolution", gridSize.x, gridSize.y);
        noiseCompute.SetInt("gridSize", perlinCellSize);
        noiseCompute.SetFloat("seed", Random.Range(0f, 1f));
        noiseCompute.SetFloat("intensity", perlinIntensity);

        noiseCompute.SetTexture(noiseCompute.FindKernel("PerlinNoise"), "result", rw);
        noiseCompute.Dispatch(noiseCompute.FindKernel("PerlinNoise"), (int)Mathf.Ceil(gridSize.x / 8f), (int)Mathf.Ceil(gridSize.y / 8f), 1);

        return rw;
    }
    Texture2D generateVisuals(RenderTexture _humidityMap, RenderTexture _temperatureMap)
    {
        RenderTexture rw = new RenderTexture(gridSize.x, gridSize.y, 0);
        rw.enableRandomWrite = true;
        rw.Create();

        visualCompute.SetInts("resolution", gridSize.x, gridSize.y);
        visualCompute.SetFloat("waterFactor", waterFactor);

        visualCompute.SetTexture(visualCompute.FindKernel("VisualGeneration"), "result", rw);

        visualCompute.SetTexture(visualCompute.FindKernel("VisualGeneration"), "humidity", _humidityMap);
        visualCompute.SetTexture(visualCompute.FindKernel("VisualGeneration"), "temperature", _temperatureMap);

        visualCompute.Dispatch(visualCompute.FindKernel("VisualGeneration"), (int)Mathf.Ceil(gridSize.x / 8f), (int)Mathf.Ceil(gridSize.y / 8f), 1);

        return renderTexTo2D(rw);
    }

    void generateMap()
    {
        //Create data maps
        RenderTexture renderHumidity = generatePerlinNoise();
        RenderTexture renderTemperature = generatePerlinNoise();

        //Create visuals
        Sprite sprite = Sprite.Create(generateVisuals(renderHumidity, renderTemperature), new Rect(0f, 0f, gridSize.x, gridSize.y), new Vector2(0.5f, 0.5f), 1f / cellSize);
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 originPosition = (Vector2)transform.position - ((Vector2)gridSize * cellSize / 2f);
        for (int i = 0; i < gridSize.x; i++)
        {
            Vector2 start = originPosition + new Vector2(i * cellSize, 0f);
            Vector2 end = originPosition + new Vector2(i * cellSize, gridSize.y * cellSize);
            Debug.DrawLine(start, end, Color.black);
        }

        for (int i = 0; i < gridSize.y; i++)
        {
            Vector2 start = originPosition + new Vector2(0f, i * cellSize);
            Vector2 end = originPosition + new Vector2(gridSize.x * cellSize, i * cellSize);
            Debug.DrawLine(start, end, Color.black);
        }
    }
}
