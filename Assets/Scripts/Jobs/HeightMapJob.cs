using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Jobs
{
    [BurstCompile]
    public struct HeightMapJob : IJobFor
    {
        [ReadOnly] public int Width;
        [ReadOnly] public int Height;
        [ReadOnly] public float Scale;
        [ReadOnly] public int Octaves;
        [ReadOnly] public float Persistence;
        [ReadOnly] public float Lacunarity;
        [ReadOnly] public float2 Offset;
        [ReadOnly] public NativeArray<float2> OctaveOffsets;

        [WriteOnly] public NativeArray<float> HeightMap;

        public void Execute(int index)
        {
            var x = index % Width;
            var y = index / Width;

            var halfWidth = Width / 2f;
            var halfHeight = Height / 2f;

            var amplitude = 1f;
            var frequency = 1f;
            var noiseHeight = 0f;

            for (var i = 0; i < Octaves; i++)
            {
                var sampleX = (x - halfWidth + OctaveOffsets[i].x + Offset.x) / Scale * frequency;
                var sampleY = (y - halfHeight + OctaveOffsets[i].y + Offset.y) / Scale * frequency;

                var perlinValue = noise.cnoise(new float2(sampleX, sampleY));
                noiseHeight += perlinValue * amplitude;

                amplitude *= Persistence;
                frequency *= Lacunarity;
            }

            HeightMap[index] = noiseHeight;
        }
    }
}