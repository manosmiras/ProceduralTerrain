using Core;
using Unity.Collections;
using UnityEngine;

namespace MonoBehaviours
{
    [ExecuteInEditMode]
    public class TerrainChunk : MonoBehaviour
    {
        public MeshData MeshData;
        public NativeArray<float> HeightMap;
        private MeshFilter _meshFilter;
        private HeightMapGenerator _heightMapGenerator;
        private MeshGenerator _meshGenerator;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            var terrain = ProceduralTerrain.Instance;
            _heightMapGenerator = new HeightMapGenerator(
                width: terrain.ChunkSize,
                height: terrain.ChunkSize,
                scale: terrain.NoiseScale,
                seed: terrain.Seed,
                octaves: terrain.Octaves,
                persistence: terrain.Persistence,
                lacunarity: terrain.Lacunarity
            );
            _meshGenerator = new MeshGenerator(terrain.HeightMultiplier, terrain.HeightCurve);
        }

        public async Awaitable Generate(int lod = 0)
        {
            if (HeightMap.IsCreated)
            {
                HeightMap.Dispose();
            }
            if (MeshData.IsCreated)
            {
                MeshData.Dispose();
            }
            HeightMap = await GenerateHeightMap();
            MeshData = await GenerateMesh(HeightMap, lod);
        }

        public async Awaitable UpdateLod(int lod)
        {
            if (MeshData.IsCreated)
            {
                MeshData.Dispose();
            }
            
            if (!HeightMap.IsCreated)
            {
                HeightMap = await GenerateHeightMap();
            }
            
            MeshData = await GenerateMesh(HeightMap, lod);
        }

        private Awaitable<NativeArray<float>> GenerateHeightMap()
        {
            var heightMap = _heightMapGenerator.Generate(transform.position);
            return heightMap;
        }

        private async Awaitable<MeshData> GenerateMesh(NativeArray<float> heightMap, int lod)
        {
            var terrain = ProceduralTerrain.Instance;
            var meshData = await _meshGenerator.Generate(heightMap, terrain.ChunkSize, terrain.ChunkSize, lod);
            var mesh = meshData.CreateMesh();
            _meshFilter.sharedMesh = mesh;
            return meshData;
        }

        private void OnDisable()
        {
            DisposeNativeData();
        }

        private void OnDestroy()
        {
            DisposeNativeData();
        }

        private void DisposeNativeData()
        {
            if (HeightMap.IsCreated)
            {
                HeightMap.Dispose();
            }

            if (MeshData.IsCreated)
            {
                MeshData.Dispose();
            }
        }
    }
}