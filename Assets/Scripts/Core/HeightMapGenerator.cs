using Jobs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Core
{
    public class HeightMapGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly float _scale;
        private readonly int _seed;
        private readonly int _octaves;
        private readonly float _persistence;
        private readonly float _lacunarity;

        public HeightMapGenerator(int width, int height, float scale, int seed = 1, int octaves = 8,
            float persistence = 0.5f, float lacunarity = 2f)
        {
            _width = width;
            _height = height;
            _scale = scale;
            _seed = seed;
            _octaves = octaves;
            _persistence = persistence;
            _lacunarity = lacunarity;
        }

        public async Awaitable<NativeArray<float>> Generate(Vector3 offset)
        {
            var rng = new System.Random(_seed);
            var octaveOffsets = new NativeArray<float2>(_octaves, Allocator.Persistent);
            float maxPossibleHeight = 1 / (1 - _persistence);
            for (var i = 0; i < _octaves; i++)
            {
                var offsetX = rng.Next(-100000, 100000) + offset.x;
                var offsetY = rng.Next(-100000, 100000) - offset.z;
                octaveOffsets[i] = new float2(offsetX, offsetY);
            }

            var length = _width * _height;
            var result = new NativeArray<float>(length, Allocator.Persistent);

            try
            {
                await Awaitable.EndOfFrameAsync();
                var noiseJob = new HeightMapJob
                {
                    Width = _width,
                    Height = _height,
                    Scale = _scale,
                    Octaves = _octaves,
                    Persistence = _persistence,
                    Lacunarity = _lacunarity,
                    OctaveOffsets = octaveOffsets,
                    HeightMap = result
                };

                var handle = noiseJob.ScheduleParallel(length, 64, default);
                //await Awaitable.NextFrameAsync();
                while (!handle.IsCompleted)
                {
                    await Awaitable.NextFrameAsync();
                }
                handle.Complete();
                Normalize(result, maxPossibleHeight);
                return result;
            }
            catch
            {
                if (result.IsCreated)
                {
                    result.Dispose();
                }

                throw;
            }
            finally
            {
                if (octaveOffsets.IsCreated)
                {
                    octaveOffsets.Dispose();
                }
            }
        }

        private static void Normalize(NativeArray<float> heights, float maxHeight)
        {
            for (var i = 0; i < heights.Length; i++)
            {
                heights[i] /= maxHeight;
            }
        }
    }
}