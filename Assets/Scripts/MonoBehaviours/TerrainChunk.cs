using Core;
using TMPro;
using Unity.Profiling;
using UnityEngine;

namespace MonoBehaviours
{
    [ExecuteInEditMode]
    public class TerrainChunk : MonoBehaviour
    {
        public MeshData MeshData;
        public float[,] HeightMap;
        public int Lod;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private TextMeshPro _textMesh;
        private static readonly ProfilerMarker NoiseMarker = new ProfilerMarker("ProceduralTerrain.Noise");
        private static readonly ProfilerMarker MeshMarker = new ProfilerMarker("ProceduralTerrain.Mesh");
        private HeightMapGenerator _heightMapGenerator;
        private MeshGenerator _meshGenerator;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
            _textMesh = GetComponentInChildren<TextMeshPro>();
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

        private void SetLabel(int lod)
        {
            _textMesh.text = $"LOD: {lod}";
        }

        public void Generate(int lod = 0)
        {
            Lod = lod;
            HeightMap = GenerateHeightMap();
            MeshData = GenerateMesh(HeightMap, lod);
            SetLabel(lod);
        }

        public void UpdateLod(int lod)
        {
            Lod = lod;
            MeshData = GenerateMesh(HeightMap, lod);
            SetLabel(lod);
        }

        private float[,] GenerateHeightMap()
        {
            NoiseMarker.Begin();
            var heightMap = _heightMapGenerator.Generate(transform.position);
            NoiseMarker.End();
            return heightMap;
        }

        private MeshData GenerateMesh(float[,] heightMap, int lod)
        {
            MeshMarker.Begin();
            var meshData = _meshGenerator.Generate(heightMap, lod);
            var mesh = meshData.CreateMesh();
            _meshFilter.sharedMesh = mesh;
            //_meshCollider.sharedMesh = mesh;
            MeshMarker.End();
            return meshData;
        }
    }
}