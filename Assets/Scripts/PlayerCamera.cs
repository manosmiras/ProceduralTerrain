using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerCamera : MonoBehaviour
{
    public float Speed = 40f;
    public int PathSamples = 8;

    private Spline _splinePath;
    private float3[] _cameraPath;
    private TerrainChunk _chunk;
    private int _pathIndex;
    private float _progress;
    
    private void OnEnable()
    {
        ProceduralTerrain.Instance.OnTerrainGenerated += GenerateCameraPath;
    }

    private void GenerateCameraPath()
    {
        Debug.Log("Generating camera path");
        var chunk = ProceduralTerrain.Instance.Chunks[1, 0];
        _cameraPath = GetPathForChunk(chunk, PathSamples);
        _splinePath = new Spline();
        _splinePath.AddRange(_cameraPath);
    }

    private float3[] GetPathForChunk(TerrainChunk terrainChunk, int samples)
    {
        var path = new float3[samples];
        var terrain = ProceduralTerrain.Instance;
        var width = terrainChunk.MeshData.Width;
        var height = terrainChunk.MeshData.Height;
        var x = width / 2f;

        for (var i = 0; i < samples; i++)
        {
            var z = height - 1 - i * ((height - 1) / (float)(samples - 1));
            var vertexZ = Mathf.RoundToInt(z);
            var vertexIndex = (int)x + vertexZ * height;

            path[i] = terrainChunk.transform.position + terrainChunk.MeshData.Vertices[vertexIndex];
        }

        return path;
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
        if (_progress >= 1.0f)
        {
            ProceduralTerrain.Instance.AddTerrainChunks();
            _progress = 0f;
            return;
        }
        _splinePath.Evaluate(_progress, out var position, out var tangent, out var up);
        transform.position = position;
        //transform.rotation = Quaternion.LookRotation(tangent, up);
    }
    
    private float GetApproxSplineLength()
    {
        return ProceduralTerrain.Instance.Chunks.GetLength(1) * ProceduralTerrain.Instance.ChunkSize;
    }
}