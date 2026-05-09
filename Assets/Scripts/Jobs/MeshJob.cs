using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Jobs
{
    [BurstCompile]
    public struct MeshJob : IJobFor
    {
        [ReadOnly] public int VerticesPerLineX;
        [ReadOnly] public int VerticesPerLineY;
        [ReadOnly] public int Width;
        [ReadOnly] public int Height;
        [ReadOnly] public float Step;
        [ReadOnly] public float TopLeftX;
        [ReadOnly] public float TopLeftZ;
        [ReadOnly] public float HeightMultiplier;
        [ReadOnly] public NativeArray<float> HeightCurve;
        [ReadOnly] public NativeArray<float> HeightMap;
        
        [WriteOnly] public NativeArray<float3> Vertices;
        [WriteOnly] public NativeArray<float2> Uvs;
        [WriteOnly] [NativeDisableParallelForRestriction] public NativeArray<int> Triangles;
        
        public void Execute(int index)
        {
            var x = index % VerticesPerLineX;
            var y = index / VerticesPerLineX;

            var sourceX = (int)math.min(x * Step, Width - 1);
            var sourceY = (int)math.min(y * Step, Height - 1);

            var heightSample = HeightMap[sourceX + sourceY * Width];

            var curveIndex = (int)math.clamp(heightSample * (HeightCurve.Length - 1), 0, HeightCurve.Length - 1);
            var curvedHeight = HeightCurve[curveIndex];

            Vertices[index] = new float3(
                TopLeftX + sourceX,
                (curvedHeight + heightSample) * HeightMultiplier,
                TopLeftZ - sourceY
            );

            Uvs[index] = new float2(
                sourceX / (float)(Width - 1),
                sourceY / (float)(Height - 1)
            );

            if (x < VerticesPerLineX - 1 && y < VerticesPerLineY - 1)
            {
                var quadIndex = x + y * (VerticesPerLineX - 1);
                var triangleIndex = quadIndex * 6;

                Triangles[triangleIndex] = index;
                Triangles[triangleIndex + 1] = index + VerticesPerLineX + 1;
                Triangles[triangleIndex + 2] = index + VerticesPerLineX;

                Triangles[triangleIndex + 3] = index + VerticesPerLineX + 1;
                Triangles[triangleIndex + 4] = index;
                Triangles[triangleIndex + 5] = index + 1;
            }
        }
    }
}