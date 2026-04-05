using UnityEngine;

public static class Noise
{
    public static float[,] Perlin(int width, int height, float scale, int seed = 1, int octaves = 8,
        float persistence = 0.5f, float lacunarity = 2f, Vector3 offset = default)
    {
        var heights = new float[width, height];
        var rng = new System.Random(seed);
        var octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        var halfWidth = width / 2f;
        var halfHeight = height / 2f;
        for (var i = 0; i < octaves; i++)
        {
            var offsetX = rng.Next(-100000, 100000) + offset.x;
            var offsetY = rng.Next(-100000, 100000) - offset.z;
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
                    var sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    var sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;
                    var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                heights[x, y] = noiseHeight;
            }
        }

        return Normalize(heights, maxPossibleHeight);
    }

    public static float[,] SimplePerlin(int width, int height, float scale, Vector3 offset = default)
    {
        var heights = new float[width, height];
        var halfWidth = width / 2f;
        var halfHeight = height / 2f;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var sampleX = (x - halfWidth + offset.x) / scale;
                var sampleY = (y - halfHeight - offset.z) / scale;
                heights[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
            }
        }
        return heights;
    }

    private static float[,] Normalize(float[,] heights, float maxHeight)
    {
        for (var y = 0; y < heights.GetLength(0); y++)
        {
            for (var x = 0; x < heights.GetLength(1); x++)
            {
                heights[x, y] /= maxHeight;
            }
        }
        return heights;
    }
}