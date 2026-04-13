using System;
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
    private float _progress;
    
    private void OnEnable()
    {
        ProceduralTerrain.Instance.OnTerrainGenerated += GenerateCameraPath;
    }

    private void GenerateCameraPath()
    {
        Debug.Log("Generating camera path");
        _cameraPath = GetPathForChunk(ProceduralTerrain.Instance.TerrainChunks[0], 4);
        _splinePath = new Spline();
        _splinePath.AddRange(_cameraPath);
        Debug.Log($"Camera path has {_cameraPath.Length} points");
    }

    private float3[] GetPathForChunk(TerrainChunk terrainChunk, int samples)
    {
        var path = new float3[samples];
        var terrain = ProceduralTerrain.Instance;
        var x = terrain.ChunkSize / 2f;

        for (var i = 0; i < samples; i++)
        {
            var z = terrain.ChunkSize - 1 - i * ((terrain.ChunkSize - 1) / (float)(samples - 1));
            var vertexZ = Mathf.RoundToInt(z);
            var vertexIndex = (int)x + vertexZ * terrain.ChunkSize;

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
            ProceduralTerrain.Instance.AddTerrainChunk();
            _progress = 0f;
            return;
        }
        _splinePath.Evaluate(_progress, out var position, out var tangent, out var up);
        transform.position = position;
        //transform.rotation = Quaternion.LookRotation(tangent, up);
    }
    
    private float GetApproxSplineLength()
    {
        return ProceduralTerrain.Instance.TerrainChunks.Count * ProceduralTerrain.Instance.ChunkSize;
    }
}