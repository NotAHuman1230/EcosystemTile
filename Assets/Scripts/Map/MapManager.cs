using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ComputeShader noiseCompute;
    [SerializeField] ComputeShader visualCompute;
    [SerializeField] ComputeShader foodCompute;

    [Header("Map Parameters")]
    [SerializeField] Vector2Int gridSize;
    [SerializeField] float cellSize;

    [Header("Biome Parameters")]
    [Range(0f, 1f)][SerializeField] float waterCutoff;
    [SerializeField] float humidityStrength;
    [SerializeField] Color waterColour;
    [SerializeField] Color baseColour;
    [SerializeField] Color dryColour;

    [Header("Food Paramters")]
    [SerializeField] float foodHumidityFactor;
    [SerializeField] Vector2 foodGain;
    [SerializeField] Vector2 foodRange;


    [Header("Perlin Noise")]
    [SerializeField] int perlinCellSize;
    [SerializeField] float perlinIntensity;

    Texture2D humidityTexture;
    Texture2D foodTexutre;

    private void Start()
    {
        generateMap();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            generateMap();
    }

    RenderTexture tex2DToRenderTex(Texture2D _texture)
    {
        RenderTexture rt = new RenderTexture(_texture.width, _texture.height, 0);
        RenderTexture.active = rt;
        Graphics.Blit(_texture, rt);
        return rt;
    }
        
    Texture2D renderTexTo2D(RenderTexture _renderTexture)
    {
        Texture2D tex = new Texture2D(gridSize.x, gridSize.y);
        RenderTexture.active = _renderTexture;
        tex.filterMode = FilterMode.Point;
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
        noiseCompute.SetFloat("cutOff", waterCutoff);

        noiseCompute.SetTexture(noiseCompute.FindKernel("PerlinNoise"), "result", rw);
        noiseCompute.Dispatch(noiseCompute.FindKernel("PerlinNoise"), (int)Mathf.Ceil(gridSize.x / 8f), (int)Mathf.Ceil(gridSize.y / 8f), 1);

        return rw;
    }
    Texture2D generateVisuals(RenderTexture _humidityMap)
    {
        RenderTexture rw = new RenderTexture(gridSize.x, gridSize.y, 0);
        rw.enableRandomWrite = true;
        rw.Create();

        visualCompute.SetInts("resolution", gridSize.x, gridSize.y);
        visualCompute.SetFloat("waterCutoff", waterCutoff);
        visualCompute.SetFloat("humidityStrength", humidityStrength);

        visualCompute.SetFloats("water", waterColour.r, waterColour.g, waterColour.b);
        visualCompute.SetFloats("base", baseColour.r, baseColour.g, baseColour.b);
        visualCompute.SetFloats("dry", dryColour.r, dryColour.g, dryColour.b);

        visualCompute.SetTexture(visualCompute.FindKernel("VisualGeneration"), "humidity", _humidityMap);
        visualCompute.SetTexture(visualCompute.FindKernel("VisualGeneration"), "result", rw);

        visualCompute.Dispatch(visualCompute.FindKernel("VisualGeneration"), (int)Mathf.Ceil(gridSize.x / 8f), (int)Mathf.Ceil(gridSize.y / 8f), 1);

        return renderTexTo2D(rw);
    }

    void generateMap()
    {
        //Create data maps
        RenderTexture renderHumidity = generatePerlinNoise();

        //Convert data maps to textures
        humidityTexture = renderTexTo2D(renderHumidity);

        //Create visuals
        Sprite sprite = Sprite.Create(generateVisuals(renderHumidity), new Rect(0f, 0f, gridSize.x, gridSize.y), new Vector2(0.5f, 0.5f), 1f / cellSize);
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
    void updateFood()
    {
        foodCompute.SetFloats("resolution", gridSize.x, gridSize.y);
        foodCompute.SetFloat("deltaTime", Time.deltaTime);
        foodCompute.SetFloat("foodHumidityFactor", foodHumidityFactor);
        foodCompute.SetFloats("foodGain", foodGain.x, foodGain.y);
        foodCompute.SetFloats("foodRange", foodRange.x, foodRange.y);

        foodCompute.SetTexture(foodCompute.FindKernel("FoodUpdate"), "humidity", tex2DToRenderTex(humidityTexture));
        foodCompute.SetTexture(foodCompute.FindKernel("FoodUpdate"), "result", tex2DToRenderTex(foodTexutre));

        foodCompute.Dispatch(foodCompute.FindKernel("FoodUpdate"), (int)Mathf.Ceil(gridSize.x / 8f), (int)Mathf.Ceil(gridSize.y / 8f), 1);
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
