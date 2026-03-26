using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class ProceduralTerrain : MonoBehaviour
{
    private Terrain _terrain;
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
        var noise = Noise.Perlin(width, height, _noiseScale, _seed);
        terrainData.SetHeights(0, 0, noise);
    }
}