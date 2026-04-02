using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTerrain : MonoBehaviour
{
    private MeshFilter _meshFilter;
    [SerializeField] private float _noiseScale = 80f;
    [SerializeField] private int _seed = 0;
    [SerializeField] private float _heightMultiplier = 50;
    [SerializeField] private int _octaves = 8;
    [SerializeField] private float _persistence = 0.5f;
    [SerializeField] private float _lacunarity = 2f;
    [SerializeField] private AnimationCurve _heightCurve;
    [Range(0, 6)]
    [SerializeField] private int _lod = 0;
    [SerializeField] private int _mapChunkSize = 241;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void Generate()
    {
        //var noise = Noise.Perlin(_mapChunkSize, _mapChunkSize, _noiseScale, _seed, _octaves, _persistence, _lacunarity, new Vector2(transform.position.x, transform.position.z));
        var noise = Noise.PerlinSimple(_mapChunkSize, _mapChunkSize, _noiseScale, new Vector2(transform.position.x, transform.position.z));
        var meshData = MeshGenerator.GenerateTerrainMesh(noise, _heightMultiplier, _heightCurve, _lod);
        _meshFilter.sharedMesh = meshData.CreateMesh();
    }
}