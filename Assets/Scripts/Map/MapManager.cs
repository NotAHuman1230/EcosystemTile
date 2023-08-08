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
    [SerializeField] Vector2 foodHumidityFactor;
    [SerializeField] Vector2 foodGain;
    [SerializeField] Vector2 foodRange;


    [Header("Perlin Noise")]
    [SerializeField] int perlinCellSize;
    [SerializeField] float perlinIntensity;

    Texture2D humidityTexture;
    [SerializeField] Texture2D foodTexutre;

    private void Start()
    {
        generateMap();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            generateMap();
        else if (Input.GetKeyDown(KeyCode.Space))
            updateFood();
    }

    RenderTexture createRenderTexture(int _width, int _height)
    {
        RenderTexture rt = new RenderTexture(_width, _height, 0);
        rt.enableRandomWrite = true;
        rt.Create();
        return rt;
    }

    RenderTexture tex2DToRenderTex(Texture2D _texture)
    {
        RenderTexture rt = new RenderTexture(_texture.width, _texture.height, 0);
        rt.enableRandomWrite = true;
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
        RenderTexture rt = createRenderTexture(gridSize.x, gridSize.y);

        noiseCompute.SetInts("resolution", gridSize.x, gridSize.y);
        noiseCompute.SetInt("gridSize", perlinCellSize);
        noiseCompute.SetFloat("seed", Random.Range(0f, 1f));
        noiseCompute.SetFloat("intensity", perlinIntensity);
        noiseCompute.SetFloat("cutOff", waterCutoff);

        noiseCompute.SetTexture(noiseCompute.FindKernel("PerlinNoise"), "result", rt);
        noiseCompute.Dispatch(noiseCompute.FindKernel("PerlinNoise"), (int)Mathf.Ceil(gridSize.x / 8f), (int)Mathf.Ceil(gridSize.y / 8f), 1);

        return rt;
    }
    Texture2D generateVisuals(RenderTexture _humidityMap)
    {
        RenderTexture rt = createRenderTexture(gridSize.x, gridSize.y);

        visualCompute.SetInts("resolution", gridSize.x, gridSize.y);
        visualCompute.SetFloat("waterCutoff", waterCutoff);
        visualCompute.SetFloat("humidityStrength", humidityStrength);

        visualCompute.SetFloats("water", waterColour.r, waterColour.g, waterColour.b);
        visualCompute.SetFloats("base", baseColour.r, baseColour.g, baseColour.b);
        visualCompute.SetFloats("dry", dryColour.r, dryColour.g, dryColour.b);

        visualCompute.SetTexture(visualCompute.FindKernel("VisualGeneration"), "humidity", _humidityMap);
        visualCompute.SetTexture(visualCompute.FindKernel("VisualGeneration"), "result", rt);

        visualCompute.Dispatch(visualCompute.FindKernel("VisualGeneration"), (int)Mathf.Ceil(gridSize.x / 8f), (int)Mathf.Ceil(gridSize.y / 8f), 1);

        return renderTexTo2D(rt);
    }
    Texture2D initialiseFood(RenderTexture _humidityMap)
    {
        RenderTexture rt = new RenderTexture(gridSize.x, gridSize.y, 0);
        rt.enableRandomWrite = true;
        rt.Create();
        foodCompute.SetInts("resolution", gridSize.x, gridSize.y);
        foodCompute.SetFloat("seed", Random.Range(0f, 10f));
        foodCompute.SetFloats("foodHumidityFactor", foodHumidityFactor.x, foodHumidityFactor.y);
        foodCompute.SetFloats("foodRange", foodRange.x / 100f, foodRange.y / 100f);

        foodCompute.SetTexture(foodCompute.FindKernel("FoodInitialisation"), "humidity", _humidityMap);
        foodCompute.SetTexture(foodCompute.FindKernel("FoodInitialisation"), "result", rt);
        foodCompute.Dispatch(foodCompute.FindKernel("FoodInitialisation"), (int)Mathf.Ceil(gridSize.x / 8f), (int)Mathf.Ceil(gridSize.y / 8f), 1);

        return renderTexTo2D(rt);
    }

    void generateMap()
    {
        //Create data maps
        RenderTexture renderHumidity = generatePerlinNoise();

        //Convert data maps to textures
        humidityTexture = renderTexTo2D(renderHumidity);
        foodTexutre = initialiseFood(renderHumidity);

        //Create visuals
        Sprite sprite = Sprite.Create(generateVisuals(renderHumidity), new Rect(0f, 0f, gridSize.x, gridSize.y), new Vector2(0.5f, 0.5f), 1f / cellSize);
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
    void updateFood()
    {
        RenderTexture rt = tex2DToRenderTex(foodTexutre);

        foodCompute.SetInts("resolution", gridSize.x, gridSize.y);
        foodCompute.SetFloats("foodHumidityFactor", foodHumidityFactor.x, foodHumidityFactor.y);
        foodCompute.SetFloats("foodGain", foodGain.x / 100f, foodGain.y / 100f);
        foodCompute.SetFloats("foodRange", foodRange.x / 100f, foodRange.y / 100f);

        foodCompute.SetTexture(foodCompute.FindKernel("FoodUpdate"), "humidity", tex2DToRenderTex(humidityTexture));
        foodCompute.SetTexture(foodCompute.FindKernel("FoodUpdate"), "result", rt);

        foodCompute.Dispatch(foodCompute.FindKernel("FoodUpdate"), (int)Mathf.Ceil(gridSize.x / 8f), (int)Mathf.Ceil(gridSize.y / 8f), 1);
    
        foodTexutre = renderTexTo2D(rt);
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
