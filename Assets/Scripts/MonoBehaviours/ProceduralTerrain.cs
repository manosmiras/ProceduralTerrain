using System;
using Unity.Profiling;
using UnityEngine;

namespace MonoBehaviours
{
    public class ProceduralTerrain : MonoSingleton<ProceduralTerrain>
    {
        public float NoiseScale = 80f;
        public int Seed = 0;
        public float HeightMultiplier = 50;
        public int Octaves = 8;
        public float Persistence = 0.5f;
        public float Lacunarity = 2f;
        public AnimationCurve HeightCurve;
        public int ChunkSize = 256;
        public int ChunkRadius = 2;
        public GameObject TerrainChunkPrefab;
        public TerrainChunk[,] Chunks;
    
        private static readonly ProfilerMarker InitializeChunksMarker = new("ProceduralTerrain.InitializeChunks");
        private static readonly ProfilerMarker RegenerateChunksMarker = new("ProceduralTerrain.RegenerateChunks");
        private static readonly ProfilerMarker UpdateLodsMarker = new("ProceduralTerrain.UpdateLods");
        private int _generationCount = 1;
        private PlayerCamera _playerCamera;

        protected override void Awake()
        {
            base.Awake();
            InitializeChunks();
        }

        protected void Start()
        {
            _playerCamera = FindFirstObjectByType<PlayerCamera>();
            _playerCamera.TraversedChunk += RegenerateChunks;
        }

        private void OnDisable()
        {
            if (_playerCamera != null)
            {
                _playerCamera.TraversedChunk -= RegenerateChunks;
            }
        }

        public void InitializeChunks()
        {
            InitializeChunksMarker.Begin();
            Chunks = new TerrainChunk[ChunkRadius, ChunkRadius];
            ClearChildren(transform);
            var start = transform.position;
            for (var x = 0; x < ChunkRadius; x++)
            {
                for (var y = 0; y < ChunkRadius; y++)
                {
                    var position = start + new Vector3(x * (ChunkSize - 1), 0, y * (ChunkSize - 1));
                    var chunk = SpawnTerrainChunk(position: position, lod: GetLod(x, y));
                    Chunks[x, y] = chunk;
                }
            }
            InitializeChunksMarker.End();
        }

        private void RegenerateChunks()
        {
            RegenerateChunksMarker.Begin();
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
                var chunk = SpawnTerrainChunk(position, Math.Max(0, height - 2));
                Chunks[x, height - 1] = chunk;
            }
            _generationCount++;
            RegenerateChunksMarker.End();
            UpdateLods();
        }

        private void UpdateLods()
        {
            UpdateLodsMarker.Begin();
            var width = Chunks.GetLength(0);
            var height = Chunks.GetLength(1);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var chunk = Chunks[x, y];
                    chunk.UpdateLod(GetLod(x, y));
                }
            }
            UpdateLodsMarker.End();
        }
        
        private int GetLod(int x, int y)
        {
            var centerX = ChunkRadius / 2;
            var lodY = Math.Max(1, y - 1);
            var lodX = Math.Max(1, Math.Abs(centerX - x));
            return Math.Max(lodX, lodY);
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