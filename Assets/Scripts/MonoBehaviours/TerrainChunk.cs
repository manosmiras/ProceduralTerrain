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
        private static readonly ProfilerMarker NoiseMarker = new ProfilerMarker("Terrain.Noise");
        private static readonly ProfilerMarker MeshMarker = new ProfilerMarker("Terrain.Mesh");

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
            _textMesh = GetComponentInChildren<TextMeshPro>();
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
            var terrain = ProceduralTerrain.Instance;
            float[,] noise;
            if (terrain.NoiseType == NoiseType.Simple)
            {
                noise = Noise.Perlin(
                    terrain.ChunkSize,
                    terrain.ChunkSize,
                    terrain.NoiseScale,
                    terrain.Seed,
                    terrain.Octaves,
                    terrain.Persistence,
                    terrain.Lacunarity,
                    transform.position
                );
            }
            else
            {
                noise = Noise.PerlinFromJob(
                    terrain.ChunkSize,
                    terrain.ChunkSize,
                    terrain.NoiseScale,
                    terrain.Seed,
                    terrain.Octaves,
                    terrain.Persistence,
                    terrain.Lacunarity,
                    transform.position
                );
            }
            NoiseMarker.End();
            return noise;
        }

        private MeshData GenerateMesh(float[,] noise, int lod)
        {
            MeshMarker.Begin();
            var terrain = ProceduralTerrain.Instance;
            var meshData =
                MeshGenerator.GenerateTerrainMesh(noise, terrain.HeightMultiplier, terrain.HeightCurve, lod);
            var mesh = meshData.CreateMesh();
            _meshFilter.sharedMesh = mesh;
            _meshCollider.sharedMesh = mesh;
            MeshMarker.End();
            return meshData;
        }
    }
}