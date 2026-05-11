using System;
using System.Threading.Tasks;
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
        
        public event Action TerrainInitialized;
    
        private static readonly ProfilerMarker InitializeChunksMarker = new("ProceduralTerrain.InitializeChunks");
        private static readonly ProfilerMarker RegenerateChunksMarker = new("ProceduralTerrain.RegenerateChunks");
        private static readonly ProfilerMarker UpdateLodsMarker = new("ProceduralTerrain.UpdateLods");
        private int _generationCount = 1;
        private PlayerCamera _playerCamera;

        protected override void Awake()
        {
            base.Awake();
            _ = InitializeChunks();
        }

        protected void Start()
        {
            _playerCamera = FindFirstObjectByType<PlayerCamera>();
            _playerCamera.TraversedChunk += OnTraversedChunk;
        }

        private void OnDisable()
        {
            if (_playerCamera != null)
            {
                _playerCamera.TraversedChunk -= OnTraversedChunk;
            }
        }

        private void OnTraversedChunk()
        {
            _ = RegenerateChunks();
        }

        public async Task InitializeChunks()
        {
            Chunks = new TerrainChunk[ChunkRadius, ChunkRadius];
            ClearChildren(transform);
            var start = transform.position;
            var tasks = new Task<(int x, int y, TerrainChunk chunk)>[ChunkRadius * ChunkRadius];
            var index = 0;
            for (var x = 0; x < ChunkRadius; x++)
            {
                for (var y = 0; y < ChunkRadius; y++)
                {
                    var position = start + new Vector3(x * (ChunkSize - 1), 0, y * (ChunkSize - 1));
                    var localX = x;
                    var localY = y;
                    tasks[index++] = SpawnTerrainChunk(position: position, lod: GetLod(x, y))
                        .ContinueWith(t => (localX, localY, t.Result));
                }
            }

            var results = await Task.WhenAll(tasks);
            foreach (var (x, y, chunk) in results)
            {
                Chunks[x, y] = chunk;
            }
            TerrainInitialized?.Invoke();
        }

        private async Task RegenerateChunks()
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

            var tasks = new Task<(int x, TerrainChunk chunk)>[width];
            for (var x = 0; x < width; x++)
            {
                var position = transform.position + new Vector3(x * (ChunkSize - 1), 0, (height - 1 + _generationCount) * (ChunkSize - 1));
                var localX = x;
                tasks[x] = SpawnTerrainChunk(position, Math.Max(0, height - 2))
                    .ContinueWith(t => (localX, t.Result));
            }

            var results = await Task.WhenAll(tasks);
            foreach (var (x, chunk) in results)
            {
                Chunks[x, height - 1] = chunk;
            }

            _generationCount++;
            await UpdateLods();
        }

        private async Task UpdateLods()
        {
            var width = Chunks.GetLength(0);
            var height = Chunks.GetLength(1);
            var tasks = new Task[width * height];
            var index = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var chunk = Chunks[x, y];
                    tasks[index++] = chunk.UpdateLod(GetLod(x, y));
                }
            }

            await Task.WhenAll(tasks);
        }
        
        private int GetLod(int x, int y)
        {
            var centerX = ChunkRadius / 2;
            var lodY = Math.Max(1, y - 1);
            var lodX = Math.Max(1, Math.Abs(centerX - x));
            return Math.Max(lodX, lodY);
        }
    
        private async Task<TerrainChunk> SpawnTerrainChunk(Vector3 position, int lod)
        {
            var go = Instantiate(TerrainChunkPrefab, transform);
            go.transform.position = new Vector3(position.x, 0, position.z);
            var terrainChunk = go.GetComponent<TerrainChunk>();
            await terrainChunk.Generate(lod);
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