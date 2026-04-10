using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float Speed = 20f;
    private const float Distance = 10000;
    private TerrainChunk _chunk;
    private int _pathIndex;
    private float _time = 0;

    private void Start()
    {
        transform.position = new Vector3(0, 1000f, 0);
        _time = 0f;
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