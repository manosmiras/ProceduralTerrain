using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTerrain : MonoBehaviour
{
    private MeshFilter _meshFilter;
    [SerializeField] private float _noiseScale = 80f;
    [SerializeField] private int _seed = 0;
    [SerializeField] private Vector2Int _dimensions = new (256, 256);
    [SerializeField] private float _heightMultiplier = 50;
    [SerializeField] private int _octaves = 8;
    [SerializeField] private float _persistence = 0.5f;
    [SerializeField] private float _lacunarity = 2f;
    [SerializeField] private AnimationCurve _heightCurve;

    private void Start()
    {
        _seed = Random.Range(int.MinValue, int.MaxValue);
        _meshFilter = GetComponent<MeshFilter>();
        Generate(_seed);
    }

    public void Generate(int seed = 0)
    {
        if (seed == 0)
            seed = Random.Range(int.MinValue, int.MaxValue);
        _seed = seed;
        var width = _dimensions.x;
        var height = _dimensions.y;
        var noise = Noise.Perlin(width, height, _noiseScale, _seed, _octaves, _persistence, _lacunarity);
        var meshData = MeshGenerator.GenerateTerrainMesh(noise, _heightMultiplier, _heightCurve);
        _meshFilter.sharedMesh = meshData.CreateMesh();
    }
}