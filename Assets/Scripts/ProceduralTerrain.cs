using System.Text;
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
    
    [Header("Normal Debug")]
    [SerializeField] private bool _drawNormals = true;
    [SerializeField] private float _normalLength = 1f;
    [SerializeField] private Color _normalColor = Color.green;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void Generate()
    {
        var noise = Noise.Perlin(_mapChunkSize, _mapChunkSize, _noiseScale, _seed, _octaves, _persistence, _lacunarity, new Vector2(transform.position.x, transform.position.z));

        var sb = new StringBuilder();
        for (var y = 0; y < noise.GetLength(1); y++)
        {
            for (var x = 0; x < noise.GetLength(0); x++)
            {
                sb.AppendLine($"{noise[x, y]}");
            }
        }
        Debug.Log(sb.ToString());
        var meshData = MeshGenerator.GenerateTerrainMesh(noise, _heightMultiplier, _heightCurve, _lod);
        _meshFilter.sharedMesh = meshData.CreateMesh();
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!_drawNormals)
            return;

        if (_meshFilter == null)
            _meshFilter = GetComponent<MeshFilter>();

        var mesh = _meshFilter != null ? _meshFilter.sharedMesh : null;
        if (mesh == null)
            return;

        var vertices = mesh.vertices;
        var normals = mesh.normals;

        if (vertices == null || normals == null || vertices.Length != normals.Length)
            return;

        Gizmos.color = _normalColor;

        for (var i = 0; i < vertices.Length; i++)
        {
            var worldVertex = transform.TransformPoint(vertices[i]);
            var worldNormal = transform.TransformDirection(normals[i]);
            Gizmos.DrawLine(worldVertex, worldVertex + worldNormal * _normalLength);
        }
    }
}