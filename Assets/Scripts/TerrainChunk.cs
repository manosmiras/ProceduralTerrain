using UnityEngine;

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
        //var noise = Noise.SimplePerlin(terrain.ChunkSize, terrain.ChunkSize, terrain.NoiseScale, transform.position);
        var meshData = MeshGenerator.GenerateTerrainMesh(noise, terrain.HeightMultiplier, terrain.HeightCurve, terrain.Lod);
        _meshFilter.sharedMesh = meshData.CreateMesh();
    }
}