using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum NormalizeMode
    {
        Local, Global
    };

    public static float[,] GenerateMapData(int mapChunkSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        float[,] noiseMap = new float[mapChunkSize, mapChunkSize];

        System.Random prng = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0f;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);
            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapChunkSize / 2f;
        float halfHeight = mapChunkSize / 2f;

        for (int i = 0; i < mapChunkSize; i++)
        {
            for (int j = 0; j < mapChunkSize; j++)
            {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int x = 0; x < octaves; x++)
                {
                    float sampleJ = (j - halfWidth + octaveOffsets[x].x) / scale * frequency;
                    float sampleI = (i - halfHeight + octaveOffsets[x].y) / scale * frequency;

                    float perlinNoiseValue = Mathf.PerlinNoise(sampleJ, sampleI) * 2 - 1;
                    noiseHeight += perlinNoiseValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[j, i] = noiseHeight;
            }
        }

        for (int i = 0; i < mapChunkSize; i++)
        {
            for (int j = 0; j < mapChunkSize; j++)
            {
                if (normalizeMode == NormalizeMode.Local)
                {
                    noiseMap[j, i] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[j, i]);
                }
                else
                {
                    float normalizedHeight = (noiseMap[j,i] + 1) / (maxPossibleHeight);
                    noiseMap [j,i] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }
        return noiseMap;
    }
}
