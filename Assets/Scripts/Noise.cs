using UnityEngine;

public static class Noise
{
    public static float[,] Perlin(int width, int height, float scale, int seed = 1, int octaves = 8,
        float persistence = 0.5f, float lacunarity = 2f, Vector2 offset = default)
    {
        var heights = new float[width, height];
        var rng = new System.Random(seed);
        var octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;

        for (var i = 0; i < octaves; i++)
        {
            var offsetX = rng.Next(-100000, 100000);
            var offsetY = rng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
            maxPossibleHeight = 1 / (1 - persistence);
            amplitude *= persistence;
        }
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (var i = 0; i < octaves; i++)
                {
                    var sampleX = (x + offset.x) / scale * frequency + octaveOffsets[i].x;
                    var sampleY = (y + offset.y) / scale * frequency + octaveOffsets[i].y;
                    var perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                heights[x, y] = noiseHeight;
            }
        }
        return Normalize(heights, maxPossibleHeight);
    }

    private static float[,] Normalize(float[,] heights, float maxHeight)
    {
        for (var y = 0; y < heights.GetLength(1); y++)
        {
            for (var x = 0; x < heights.GetLength(0); x++)
            {
                heights[x, y] /= maxHeight;
            }
        }
        return heights;
    }
}