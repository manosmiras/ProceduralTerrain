using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerCamera : MonoBehaviour
{
    public float Speed = 40f;

    private Spline _splinePath;
    private float3[] _cameraPath;
    private TerrainChunk _chunk;
    private int _pathIndex;
    private const int PathSimplification = 64;
    private float _progress;
    
    private void OnEnable()
    {
        ProceduralTerrain.Instance.OnTerrainGenerated += GenerateCameraPath;
    }

    private void OnDisable()
    {
        ProceduralTerrain.Instance.OnTerrainGenerated -= GenerateCameraPath;
    }

    private void Start()
    {
        transform.position = new Vector3(0, 1000f, 0);
    }

    private void GenerateCameraPath()
    {
        var terrain = ProceduralTerrain.Instance;
        var chunkCount = terrain.TerrainChunks.Count;
        var pointsPerChunk = (terrain.ChunkSize + PathSimplification - 1) / PathSimplification;
        _cameraPath = new float3[pointsPerChunk * chunkCount];
        for (var chunkIndex = 0; chunkIndex < chunkCount; chunkIndex++)
        {
            var terrainChunk = terrain.TerrainChunks[chunkIndex];
            var middleX = terrain.ChunkSize / 2;
            
            var pathIndex = 0;
            var baseIndex = chunkIndex * pointsPerChunk;
            for (var z = terrain.ChunkSize - 1; z >= 0; z -= PathSimplification)
            {
                var index = middleX + z * terrain.ChunkSize;
                _cameraPath[baseIndex + pathIndex] =  terrainChunk.transform.position + terrainChunk.MeshData.Vertices[index];
                pathIndex++;
            }
        }

        _splinePath = new Spline();
        _splinePath.AddRange(_cameraPath);
        Debug.Log($"Camera path has {_cameraPath.Length} points");
    }
    
    private void Update()
    {
        for (var i = 0; i < _cameraPath.Length - 1; i++)
        {
            Debug.DrawLine(_cameraPath[i], _cameraPath[i + 1], Color.red);
        }
    }
    
    private void LateUpdate()
    {
        if (_splinePath == null || _splinePath.Count < 2)
            return;

        _progress += (Speed * Time.deltaTime) / GetApproxSplineLength();
        _progress = Mathf.Repeat(_progress, 1f);

        _splinePath.Evaluate(_progress, out var position, out var tangent, out var up);

        transform.position = position;
        transform.rotation = Quaternion.LookRotation(tangent, up);
    }
    
    private float GetApproxSplineLength()
    {
        return ProceduralTerrain.Instance.TerrainChunks.Count * ProceduralTerrain.Instance.ChunkSize;
    }
}