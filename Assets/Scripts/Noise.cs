using UnityEngine;

public static class Noise
{
    public static float[,] Perlin(int width, int height, float scale, int seed = 1, int octaves = 8,
        float persistence = 0.5f, float lacunarity = 2f)
    {
        var heights = new float[width, height];
        var rng = new System.Random(seed);
        var octaveOffsets = new Vector2[octaves];
        for (var i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(-100000, 100000);
            float offsetY = rng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        var maxNoiseHeight = float.MinValue;
        var minNoiseHeight = float.MaxValue;

        var halfWidth = width / 2f;
        var halfHeight = height / 2f;

        if (scale <= 0f)
        {
            scale = 0.0001f;
        }

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (var i = 0; i < octaves; i++)
                {
                    var sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    var sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;
                    var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistence;
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

                heights[x, y] = noiseHeight;
            }
        }

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                heights[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heights[x, y]);
            }
        }

        return heights;
    }
}