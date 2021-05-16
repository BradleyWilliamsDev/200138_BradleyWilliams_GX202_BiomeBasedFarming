using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapGenerator : MonoBehaviour
{

    public enum DrawMode
    {
        NoiseMap,colourMap,Mesh
    };

    public DrawMode drawMode;

    public const int mapChunkSize = 241;
    [Range(0,6)]
    public int levelOfDetail;
    public float noiseScale;
    public float meshHeightMultiplier;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainTypes[] region;

    public void GenerateNoiseMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        // GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];

        for (int x = 0; x < mapChunkSize; x++)
        {
            for (int y = 0; y < mapChunkSize; y++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < region.Length; i++)
                {
                    if (currentHeight <= region[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = region[i].colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGen.TextureFromHeightMap(noiseMap));    
        }
        else if (drawMode == DrawMode.colourMap)
        {
            display.DrawTexture(TextureGen.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));    
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGen.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
    }

    private void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }

}

[System.Serializable]
public struct TerrainTypes
{
    public string name;
    public float height;
    public Color colour;
}
