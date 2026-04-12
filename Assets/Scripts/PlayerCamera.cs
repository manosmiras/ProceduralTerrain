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

    private void Start()
    {
        transform.position = new Vector3(0, 1000f, 0);
    }

    private void GenerateCameraPath()
    {
        Debug.Log("Generating camera path");
        // TODO: Take lod into account? 
        _cameraPath = GetPathForChunk(ProceduralTerrain.Instance.TerrainChunks[0], 4);
        foreach (var p in _cameraPath)
        {
            new GameObject("Camera Path Point").transform.position = p;
        }

        _splinePath = new Spline();
        _splinePath.AddRange(_cameraPath);
        Debug.Log($"Camera path has {_cameraPath.Length} points");
    }

    private float3[] GetPathForChunk(TerrainChunk chunk, int samples)
    {
        var path = new float3[samples + 1];
        var position = chunk.transform.position;
        var step = ProceduralTerrain.Instance.ChunkSize / samples;
        var start = -samples / 2;
        var end = samples / 2;
        var index = 0;
        for (var offset = start; offset <= end; offset++)
        {
            var sample = position + new Vector3(0, 1000f, step * offset);
            // Shove in / out of chunk for successful raycast
            if (offset == start)
            {
                sample.z += 0.5f;
            } else if (offset == end)
            {
                sample.z -= 0.5f;
            }
            var isHit = Physics.Raycast(sample, Vector3.down, out var hit, 10000f);
            Debug.Log(isHit);
            path[index] = hit.point;
            index++;
        }
        return path;
    }
    
    /*private void Update()
    {
        for (var i = 0; i < _cameraPath.Length - 1; i++)
        {
            Debug.DrawLine(_cameraPath[i], _cameraPath[i + 1], Color.red);
        }
        var dist = Vector3.Distance(ProceduralTerrain.Instance.LastChunk.transform.position, transform.position);
        if (dist <= ProceduralTerrain.Instance.ChunkSize / 2f)
        {
            ProceduralTerrain.Instance.AddTerrainChunk();
        }
        //Debug.Log($"Distance to last chunk: {dist}");
    }*/
    
    /*private void LateUpdate()
    {
        if (_splinePath == null || _splinePath.Count < 2)
            return;

        _progress += (Speed * Time.deltaTime) / GetApproxSplineLength();
        _progress = Mathf.Repeat(_progress, 1f);

        _splinePath.Evaluate(_progress, out var position, out var tangent, out var up);

        transform.position = position;
        transform.rotation = Quaternion.LookRotation(tangent, up);
    }*/
    
    private float GetApproxSplineLength()
    {
        return ProceduralTerrain.Instance.TerrainChunks.Count * ProceduralTerrain.Instance.ChunkSize;
    }
}