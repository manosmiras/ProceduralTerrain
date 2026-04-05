using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[ExecuteAlways]
public class TerrainChunk : MonoBehaviour
{
    private MeshFilter _meshFilter;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void Generate()
    {
        var terrain = ProceduralTerrain.Instance;
        var sw = new Stopwatch();
        sw.Start();
        var noise = Noise.Perlin(
            terrain.ChunkSize,
            terrain.ChunkSize,
            terrain.NoiseScale,
            terrain.Seed,
            terrain.Octaves,
            terrain.Persistence,
            terrain.Lacunarity,
            transform.position
        );
        sw.Stop();
        Debug.Log($"Generated noise in {sw.ElapsedMilliseconds}ms");
        sw.Restart();
        //var noise = Noise.SimplePerlin(terrain.ChunkSize, terrain.ChunkSize, terrain.NoiseScale, transform.position);
        var meshData = MeshGenerator.GenerateTerrainMesh(noise, terrain.HeightMultiplier, terrain.HeightCurve, terrain.Lod);
        sw.Stop();
        Debug.Log($"Generated mesh in {sw.ElapsedMilliseconds}ms");
        _meshFilter.sharedMesh = meshData.CreateMesh();
    }
}