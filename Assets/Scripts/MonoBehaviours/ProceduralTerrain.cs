using System;
using Unity.Profiling;
using UnityEngine;

namespace MonoBehaviours
{
    public enum NoiseType { Simple, Job }

    [ExecuteInEditMode]
    public class ProceduralTerrain : MonoSingleton<ProceduralTerrain>
    {
        public float NoiseScale = 80f;
        public int Seed = 0;
        public float HeightMultiplier = 50;
        public int Octaves = 8;
        public float Persistence = 0.5f;
        public float Lacunarity = 2f;
        public AnimationCurve HeightCurve;
        [Range(0, 6)]
        public int Lod = 0;
        public int ChunkSize = 256;
        public int ChunkRadius = 2;
        public GameObject TerrainChunkPrefab;
        public NoiseType NoiseType = NoiseType.Simple;
        public TerrainChunk[,] Chunks;
        public event Action OnTerrainGenerated;
    
        private static readonly ProfilerMarker TerrainGeneration = new ProfilerMarker("ProceduralTerrain.Generation");
        private int _generationCount = 1;

        protected void Start()
        {
            InitializeChunks();
        }

        public void InitializeChunks()
        {
            TerrainGeneration.Begin();
            Chunks = new TerrainChunk[ChunkRadius, ChunkRadius];
            ClearChildren(transform);
            var start = transform.position;
            var centerX = ChunkRadius / 2;
            for (var x = 0; x < ChunkRadius; x++)
            {
                for (var y = 0; y < ChunkRadius; y++)
                {
                    var position = start + new Vector3(x * (ChunkSize - 1), 0, y * (ChunkSize - 1));
                    var chunk = SpawnTerrainChunk(position: position, lod: y);
                    Chunks[x, y] = chunk;
                }
            }
            OnTerrainGenerated?.Invoke();
            TerrainGeneration.End();
        }

        public void RegenerateChunks()
        {
            var width = Chunks.GetLength(0);
            var height = Chunks.GetLength(1);
            
            // Shift everything up one row
            for (var y = 0; y < height - 1; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (y == 0)
                    {
                        var chunk = Chunks[x, y];
                        Destroy(chunk.gameObject);
                    }
                    Chunks[x, y] = Chunks[x, y + 1];
                }
            }
            
            for (var x = 0; x < width; x++)
            {
                var position = transform.position + new Vector3(x * (ChunkSize - 1), 0, (height - 1 + _generationCount) * (ChunkSize - 1));
                var chunk = SpawnTerrainChunk(position, x == 1 ? 0 : 1);
                Chunks[x, height - 1] = chunk;
            }
            
            _generationCount++;
            OnTerrainGenerated?.Invoke();
        }

        public void UpdateLods()
        {
            var width = Chunks.GetLength(0);
            var height = Chunks.GetLength(1);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var chunk = Chunks[x, y];
                    var newLod = Math.Max(0, chunk.Lod - 1);
                    Debug.Log($"Updating lod for chunk {x}, {y}, it's lod is {chunk.Lod}, new lod is {newLod}");
                    chunk.UpdateLod(newLod);
                }
            }
        }
    
        private TerrainChunk SpawnTerrainChunk(Vector3 position, int lod)
        {
            var go = Instantiate(TerrainChunkPrefab, transform);
            go.transform.position = new Vector3(position.x, 0, position.z);
            var terrainChunk = go.GetComponent<TerrainChunk>();
            terrainChunk.Generate(lod);
            return terrainChunk;
        }
    
        private static void ClearChildren(Transform parent)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(parent.GetChild(i).gameObject);
            }
        }
    }
}