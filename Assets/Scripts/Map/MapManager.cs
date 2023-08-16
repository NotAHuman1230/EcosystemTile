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

    [Header("Water Parameters")]
    [SerializeField] Color waterColour;
    [SerializeField] float waterCutOff;
    [SerializeField] float perlinWaterIntensity;
    [SerializeField] int perlinWaterCellSize;

    [Header("Ground Parameters")]
    [SerializeField] Color baseColour;
    [SerializeField] Color desertColour;
    [SerializeField] float desertCutOff;
    [SerializeField] float perlinDesertIntensity;
    [SerializeField] int perlinDesertCellSize;

    [Header("Food Paramters")]
    [SerializeField] Vector2 foodGain;
    [SerializeField] Vector2 foodRange;

    [HideInInspector] public Texture2D waterTexture;
    [HideInInspector] public Texture2D desertTexture;
    [HideInInspector] public Texture2D foodTexutre;

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
    Texture2D renderTexTo2D(RenderTexture _renderTexture)
    {
        Texture2D tex = new Texture2D(gridSize.x, gridSize.y);
        RenderTexture.active = _renderTexture;
        tex.filterMode = FilterMode.Point;
        tex.ReadPixels(new Rect(0f, 0f, _renderTexture.width, _renderTexture.height), 0, 0);
        tex.Apply();
        return tex;
    }

    RenderTexture generatePerlinNoise(float _cutOff, int _cellSize, float _intensity)
    {
        RenderTexture rt = createRenderTexture(gridSize.x, gridSize.y);

        noiseCompute.SetInts("resolution", gridSize.x, gridSize.y);
        noiseCompute.SetInt("gridSize", _cellSize);
        noiseCompute.SetFloat("seed", Random.Range(0f, 1f));
        noiseCompute.SetFloat("intensity", _intensity);
        noiseCompute.SetFloat("cutOff", _cutOff);

        noiseCompute.SetTexture(noiseCompute.FindKernel("PerlinNoise"), "result", rt);
        noiseCompute.Dispatch(noiseCompute.FindKernel("PerlinNoise"), (int)Mathf.Ceil(gridSize.x / 8f), (int)Mathf.Ceil(gridSize.y / 8f), 1);

        return rt;
    }
    Texture2D generateVisuals(Texture2D _waterMap, Texture2D _desertMap)
    {
        RenderTexture rt = createRenderTexture(gridSize.x, gridSize.y);

        visualCompute.SetInts("resolution", gridSize.x, gridSize.y);

        visualCompute.SetFloats("water", waterColour.r, waterColour.g, waterColour.b);
        visualCompute.SetFloats("base", baseColour.r, baseColour.g, baseColour.b);
        visualCompute.SetFloats("dry", desertColour.r, desertColour.g, desertColour.b);

        visualCompute.SetTexture(visualCompute.FindKernel("VisualGeneration"), "waterTexture", _waterMap);
        visualCompute.SetTexture(visualCompute.FindKernel("VisualGeneration"), "desertTexture", _desertMap);
        visualCompute.SetTexture(visualCompute.FindKernel("VisualGeneration"), "result", rt);

        visualCompute.Dispatch(visualCompute.FindKernel("VisualGeneration"), (int)Mathf.Ceil(gridSize.x / 8f), (int)Mathf.Ceil(gridSize.y / 8f), 1);

        return renderTexTo2D(rt);
    }
    Texture2D initialiseFood(Texture2D _waterMap, Texture2D _desertMap)
    {
        RenderTexture rt = createRenderTexture(gridSize.x, gridSize.y);

        foodCompute.SetInts("resolution", gridSize.x, gridSize.y);
        foodCompute.SetFloat("seed", Random.Range(0f, 10f));
        foodCompute.SetFloats("foodRange", foodRange.x / 100f, foodRange.y / 100f);

        foodCompute.SetTexture(foodCompute.FindKernel("FoodInitialisation"), "water", _waterMap);
        foodCompute.SetTexture(foodCompute.FindKernel("FoodInitialisation"), "desert", _desertMap);
        foodCompute.SetTexture(foodCompute.FindKernel("FoodInitialisation"), "result", rt);
        foodCompute.Dispatch(foodCompute.FindKernel("FoodInitialisation"), (int)Mathf.Ceil(gridSize.x / 8f), (int)Mathf.Ceil(gridSize.y / 8f), 1);

        return renderTexTo2D(rt);
    }

    public void generateMap()
    {
        //Create data maps
        waterTexture = renderTexTo2D(generatePerlinNoise(waterCutOff, perlinWaterCellSize, perlinWaterIntensity));
        desertTexture = renderTexTo2D(generatePerlinNoise(desertCutOff, perlinDesertCellSize, perlinDesertIntensity));
        foodTexutre = initialiseFood(waterTexture, desertTexture);

        //Create visuals
        Sprite sprite = Sprite.Create(generateVisuals(waterTexture, desertTexture), new Rect(0f, 0f, gridSize.x, gridSize.y), new Vector2(0.5f, 0.5f), 1f / cellSize);
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
    public void updateFood()
    {
        RenderTexture rt = createRenderTexture(gridSize.x, gridSize.y);

        foodCompute.SetInts("resolution", gridSize.x, gridSize.y);
        foodCompute.SetFloats("foodGain", foodGain.x / 100f, foodGain.y / 100f);
        foodCompute.SetFloats("foodRange", foodRange.x / 100f, foodRange.y / 100f);

        foodCompute.SetTexture(foodCompute.FindKernel("FoodUpdate"), "water", waterTexture);
        foodCompute.SetTexture(foodCompute.FindKernel("FoodUpdate"), "desert", desertTexture);
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
