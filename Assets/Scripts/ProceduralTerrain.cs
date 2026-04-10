using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum NoiseType { Simple, Job }

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
    public int ChunkSize = 241;
    public int ChunkRadius = 2;
    public GameObject TerrainChunkPrefab;
    private Camera _camera;
    private List<TerrainChunk> _terrainChunks = new List<TerrainChunk>();
    private float _closestDistance = float.MaxValue;
    private TerrainChunk _closestChunk;
    public NoiseType NoiseType = NoiseType.Simple;
    public Vector3[] cameraPath;
    private const int pathSimplification = 32;

    private void Start()
    {
        Generate();
        _camera = Camera.main;
    }


    public void Generate()
    {
        var sw = new Stopwatch();
        sw.Start();
        ClearChildren(transform);
        var center = transform.position;
        //for (var x = -ChunkRadius; x <= ChunkRadius; x++)
        //{
            for (var z = -ChunkRadius; z <= ChunkRadius; z++)
            {
                var position = center + new Vector3(0, 0, z * (ChunkSize - 1));
                var tc = SpawnTerrainChunk(position);
                _terrainChunks.Add(tc);
            }
        //}
        sw.Stop();
        Debug.Log($"Terrain generation took {sw.ElapsedMilliseconds}ms");
        var chunkCount = _terrainChunks.Count;
        var pointsPerChunk = (ChunkSize + pathSimplification - 1) / pathSimplification;
        cameraPath = new Vector3[pointsPerChunk * chunkCount];
        for (var chunkIndex = 0; chunkIndex < chunkCount; chunkIndex++)
        {
            var terrainChunk = _terrainChunks[chunkIndex];
            var middleX = ChunkSize / 2;
            
            var pathIndex = 0;
            var baseIndex = chunkIndex * pointsPerChunk;
            for (var z = ChunkSize - 1; z >= 0; z -= pathSimplification)
            {
                var index = middleX + z * ChunkSize;
                cameraPath[baseIndex + pathIndex] = terrainChunk.transform.position + terrainChunk.MeshData.Vertices[index];
                pathIndex++;
            }
            Debug.Log($"Camera path has {cameraPath.Length} points");
        }
    }

    private void Update()
    {
        for (var i = 0; i < cameraPath.Length - 1; i++)
        {
            Debug.DrawLine(cameraPath[i], cameraPath[i + 1], Color.red);
        }
    }

    private TerrainChunk SpawnTerrainChunk(Vector3 position)
    {
        var go = Instantiate(TerrainChunkPrefab, transform);
        go.transform.position = new Vector3(position.x, 0, position.z);
        var terrainChunk = go.GetComponent<TerrainChunk>();
        terrainChunk.Generate();
        return terrainChunk;
    }
    
    private static void ClearChildren(Transform parent)
    {
        for (var i = parent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
    
    // private void Update()
    // {
    //     foreach (var terrainChunk in _terrainChunks)
    //     {
    //         var dist = Vector3.Distance(terrainChunk.transform.position, _camera.transform.position);
    //         if (dist < _closestDistance)
    //         {
    //             _closestDistance = dist;
    //             _closestChunk = terrainChunk;
    //         }
    //     }
    //     var distToClosest = Vector3.Distance(_camera.transform.position, _closestChunk.transform.position);
    //     Debug.Log($"Closest chunk: {_closestChunk.transform.position}, {distToClosest} units away.");
    // }
}