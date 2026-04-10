using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float Speed = 20f;
    public Vector3[] CameraPath;
    private TerrainChunk _chunk;
    private int _pathIndex;
    private const int PathSimplification = 32;
    
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
        CameraPath = new Vector3[pointsPerChunk * chunkCount];
        for (var chunkIndex = 0; chunkIndex < chunkCount; chunkIndex++)
        {
            var terrainChunk = terrain.TerrainChunks[chunkIndex];
            var middleX = terrain.ChunkSize / 2;
            
            var pathIndex = 0;
            var baseIndex = chunkIndex * pointsPerChunk;
            for (var z = terrain.ChunkSize - 1; z >= 0; z -= PathSimplification)
            {
                var index = middleX + z * terrain.ChunkSize;
                CameraPath[baseIndex + pathIndex] = terrainChunk.transform.position + terrainChunk.MeshData.Vertices[index];
                pathIndex++;
            }
        }
        Debug.Log($"Camera path has {CameraPath.Length} points");
    }
    
    private void Update()
    {
        for (var i = 0; i < CameraPath.Length - 1; i++)
        {
            Debug.DrawLine(CameraPath[i], CameraPath[i + 1], Color.red);
        }
    }


//     void LateUpdate()
//     {
//         var origin = transform.position;
//         origin.y = 1000f;
//         var direction = Vector3.down;
//         if (Physics.Raycast(origin, direction * Distance, out var hitInfo))
//         {
//             Debug.Log($"Hit: {hitInfo.collider.name}");
//             if (hitInfo.collider.TryGetComponent<TerrainChunk>(out var chunk))
//             {
//                 Debug.Log($"Chunk: {chunk.transform.position}");
//                 if (_chunk != chunk)
//                 {
//                     _chunk = chunk;
//                     _pathIndex = 0;
//                 }
//             }
//             //Debug.DrawLine(origin, direction * Distance, Color.red);
//             //transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
//         }
//
//         if (_chunk == null || _chunk.cameraPath == null || _pathIndex >= _chunk.cameraPath.Length) return;
//         _time += Time.deltaTime;
//         if (_time < 1f) return;
//         _time = 0f;
//         transform.position = _chunk.cameraPath[_pathIndex];
//         _pathIndex++;
//         /*var targetPosition = _chunk.cameraPath[_pathIndex];
//         transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
//
//         if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
//         {
//             _pathIndex++;
//         }*/
//     }
}