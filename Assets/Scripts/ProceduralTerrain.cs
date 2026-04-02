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
    public GameObject TerrainChunkPrefab;

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        ClearChildren(transform);
        for (var i = 0; i < 4; i++)
        {
            var go = Instantiate(TerrainChunkPrefab, transform);
            go.transform.position = new Vector3(i * (ChunkSize - 1), 0, 0);
            go.GetComponent<TerrainChunk>().Generate();
        }
    }
    
    public static void ClearChildren(Transform parent)
    {
        for (var i = parent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
}