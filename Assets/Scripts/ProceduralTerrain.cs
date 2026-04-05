using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
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

    private void Start()
    {
        Generate();
        _camera = Camera.main;
    }


    public void Generate()
    {
        ClearChildren(transform);
        var center = transform.position;
        for (var x = -ChunkRadius; x <= ChunkRadius; x++)
        {
            for (var z = -ChunkRadius; z <= ChunkRadius; z++)
            {
                var position = center + new Vector3(x * (ChunkSize - 1), 0, z * (ChunkSize - 1));
                var tc = SpawnTerrainChunk(position);
                _terrainChunks.Add(tc);
            }
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