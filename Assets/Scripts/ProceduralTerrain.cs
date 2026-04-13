using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

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
    public int ChunkSize = 241;
    public int ChunkRadius = 2;
    public GameObject TerrainChunkPrefab;
    public NoiseType NoiseType = NoiseType.Simple;
    public List<TerrainChunk> TerrainChunks = new();
    public event Action OnTerrainGenerated;
    public TerrainChunk LastChunk;
    
    private Camera _camera;
    private TerrainChunk _closestChunk;
    private static readonly ProfilerMarker TerrainGeneration = new ProfilerMarker("Terrain.Generation");

    protected void Start()
    {
        Generate();
        _camera = Camera.main;
    }


    public void Generate()
    {
        TerrainGeneration.Begin();
        ClearChildren(transform);
        var center = transform.position;
        TerrainChunks.Clear();
        //for (var x = -ChunkRadius; x <= ChunkRadius; x++)
        //{
            for (var z = -ChunkRadius; z < ChunkRadius; z++)
            {
                var position = center + new Vector3(0, 0, z * (ChunkSize - 1));
                var tc = SpawnTerrainChunk(position);
                TerrainChunks.Add(tc);
                LastChunk = tc;
            }
        //}
        OnTerrainGenerated?.Invoke();
        TerrainGeneration.End();
    }

    public void AddTerrainChunk()
    {
        var tc = SpawnTerrainChunk(LastChunk.transform.position + new Vector3(0, 0, ChunkSize - 1));
        TerrainChunks.Add(tc);
        LastChunk = tc;
        var firstChunk = TerrainChunks[0];
        TerrainChunks.Remove(TerrainChunks[0]);
        Destroy(firstChunk.gameObject);
        OnTerrainGenerated?.Invoke();
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
}