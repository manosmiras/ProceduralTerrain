using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTerrain : MonoBehaviour
{
    private MeshFilter meshFilter;
    [SerializeField] private float noiseScale = 80f;
    [SerializeField] private int _seed = 0;
    [SerializeField] private Vector2Int dimensions = new (256, 256);
    [SerializeField] private float heightMultiplier = 50;

    private void Start()
    {
        _seed = Random.Range(int.MinValue, int.MaxValue);
        meshFilter = GetComponent<MeshFilter>();
        Generate(_seed);
    }

    public void Generate(int seed = 0)
    {
        if (seed == 0)
            seed = Random.Range(int.MinValue, int.MaxValue);
        _seed = seed;
        var width = dimensions.x;
        var height = dimensions.y;
        var noise = Noise.Perlin(width, height, noiseScale, _seed);
        var meshData = MeshGenerator.GenerateTerrainMesh(noise, heightMultiplier);
        meshFilter.sharedMesh = meshData.CreateMesh();
    }
}