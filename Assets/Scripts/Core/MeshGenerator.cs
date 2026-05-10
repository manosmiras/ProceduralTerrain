using Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Core
{
    public class MeshGenerator
    {
        private readonly float _heightMultiplier;
        private readonly AnimationCurve _heightCurve;

        public MeshGenerator(float heightMultiplier, AnimationCurve heightCurve)
        {
            _heightMultiplier = heightMultiplier;
            _heightCurve = heightCurve;
        }

        public async Awaitable<MeshData> Generate(NativeArray<float> heightMap, int width, int height, int lod = 0)
        {
            var topLeftX = (width - 1) / -2f;
            var topLeftZ = (height - 1) / 2f;

            var step = lod == 0 ? 1 : lod * 2;

            var verticesPerLineX = Mathf.CeilToInt((width - 1) / (float)step) + 1;
            var verticesPerLineY = Mathf.CeilToInt((height - 1) / (float)step) + 1;

            var meshData = new MeshData(verticesPerLineX, verticesPerLineY);

            var bakedCurve = new NativeArray<float>(256, Allocator.Persistent);
            try
            {
                for (var i = 0; i < bakedCurve.Length; i++)
                {
                    bakedCurve[i] = _heightCurve.Evaluate(i / (float)(bakedCurve.Length - 1));
                }
                await Awaitable.EndOfFrameAsync();
                var meshJob = new MeshJob
                {
                    VerticesPerLineX = verticesPerLineX,
                    VerticesPerLineY = verticesPerLineY,
                    Width = width,
                    Height = height,
                    Step = step,
                    TopLeftX = topLeftX,
                    TopLeftZ = topLeftZ,
                    HeightMultiplier = _heightMultiplier,
                    HeightCurve = bakedCurve,
                    HeightMap = heightMap,
                    Vertices = meshData.Vertices,
                    Uvs = meshData.Uvs,
                    Triangles = meshData.Triangles
                };

                var handle = meshJob.ScheduleParallel(verticesPerLineX * verticesPerLineY, 64, default);
                while (!handle.IsCompleted)
                {
                    await Awaitable.NextFrameAsync();
                }
                handle.Complete();
                return meshData;
            }
            catch
            {
                if (meshData.IsCreated)
                {
                    meshData.Dispose();
                }

                throw;
            }
            finally
            {
                if (bakedCurve.IsCreated)
                {
                    bakedCurve.Dispose();
                }
            }
        }
    }
}