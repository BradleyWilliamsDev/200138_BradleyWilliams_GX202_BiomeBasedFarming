using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateMapData(int mapChunkSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapChunkSize, mapChunkSize];

        System.Random prng = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000,100000) + offset.x;
            float offsetY = prng.Next(-100000,100000) + offset.y;

            octaveOffsets[i] = new Vector2(offsetX,offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapChunkSize/2f;
        float halfHeight = mapChunkSize /2f;

        for (int i = 0; i < mapChunkSize; i++)
        {
            for (int j = 0; j < mapChunkSize; j++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int x = 0; x < octaves; x++)
                {
                    float sampleJ = (j - halfWidth) / scale * frequency + octaveOffsets[x].x;
                    float sampleI = (i - halfHeight) / scale * frequency + octaveOffsets[x].y;

                    float perlinNoiseValue = Mathf.PerlinNoise(sampleJ, sampleI) * 2 - 1;
                    noiseHeight += perlinNoiseValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[j, i] = noiseHeight;
            }
        }

        for (int i = 0; i < mapChunkSize; i++)
        {
            for (int j = 0; j < mapChunkSize; j++)
            {
                noiseMap[j,i] = Mathf.InverseLerp(minNoiseHeight,maxNoiseHeight,noiseMap[j,i]);
            }
        }
        return noiseMap;
    }
}
