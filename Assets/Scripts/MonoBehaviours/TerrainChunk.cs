using Core;
using TMPro;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine;

namespace MonoBehaviours
{
    [ExecuteInEditMode]
    public class TerrainChunk : MonoBehaviour
    {
        public MeshData MeshData;
        public NativeArray<float> HeightMap;
        public int Lod;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private static readonly ProfilerMarker NoiseMarker = new ProfilerMarker("ProceduralTerrain.Noise");
        private static readonly ProfilerMarker MeshMarker = new ProfilerMarker("ProceduralTerrain.Mesh");
        private HeightMapGenerator _heightMapGenerator;
        private MeshGenerator _meshGenerator;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
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

        public void Generate(int lod = 0)
        {
            Lod = lod;
            if (HeightMap.IsCreated)
            {
                HeightMap.Dispose();
            }
            if (MeshData.IsCreated)
            {
                MeshData.Dispose();
            }
            HeightMap = GenerateHeightMap();
            MeshData = GenerateMesh(HeightMap, lod);
        }

        public void UpdateLod(int lod)
        {
            Lod = lod;
            if (MeshData.IsCreated)
            {
                MeshData.Dispose();
            }

            if (!HeightMap.IsCreated)
            {
                HeightMap = GenerateHeightMap();
            }
            
            MeshData = GenerateMesh(HeightMap, lod);
        }

        private NativeArray<float> GenerateHeightMap()
        {
            NoiseMarker.Begin();
            var heightMap = _heightMapGenerator.Generate(transform.position);
            NoiseMarker.End();
            return heightMap;
        }

        private MeshData GenerateMesh(NativeArray<float> heightMap, int lod)
        {
            MeshMarker.Begin();
            var terrain = ProceduralTerrain.Instance;
            var meshData = _meshGenerator.Generate(heightMap, terrain.ChunkSize, terrain.ChunkSize, lod);
            var mesh = meshData.CreateMesh();
            _meshFilter.sharedMesh = mesh;
            //_meshCollider.sharedMesh = mesh;
            MeshMarker.End();
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