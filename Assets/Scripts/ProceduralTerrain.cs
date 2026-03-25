using UnityEngine;

[ExecuteAlways]
public class ProceduralTerrain : MonoBehaviour
{
    private Terrain _terrain;
    [SerializeField] private float _heightScale = 20f;
    [SerializeField] private float _noiseScale = 0.01f;
    [SerializeField] private int _seed = 0;

    private void Awake()
    {
        _seed = Random.Range(0, 100);
        _terrain = GetComponent<Terrain>();
    }

    private void Start()
    {
        Generate(_seed);
    }

    public void Generate(int seed)
    {
        _seed = seed;
        var terrainData = _terrain.terrainData;
        var width = terrainData.heightmapResolution;
        var height = terrainData.heightmapResolution;
        terrainData.SetHeights(0, 0, GeneratePerlinNoise(width, height));
    }

    private float[,] GeneratePerlinNoise(int width, int height)
    {
        var heights = new float[width, height];

        var offsetX = _seed * 1000f;
        var offsetZ = _seed * 2000f;

        for (var z = 0; z < height; z++)
        {
            for (var x = 0; x < width; x++)
            {
                var sampleX = (x + offsetX) * _noiseScale;
                var sampleZ = (z + offsetZ) * _noiseScale;
                var noiseValue = Mathf.PerlinNoise(sampleX, sampleZ);
                heights[z, x] = noiseValue * _heightScale / _terrain.terrainData.size.y;
            }
        }
        
        return heights;
    }
}