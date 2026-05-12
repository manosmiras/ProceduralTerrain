using System;
using UnityEngine;

namespace MonoBehaviours
{
    public class PlayerCamera : MonoBehaviour
    {
        public float Speed = 40f;
        public event Action TraversedChunk;
        private float _distanceTraveled;
        
        private void Start()
        {
            Cursor.visible = false;
            var terrain = ProceduralTerrain.Instance;
            terrain.TerrainInitialized += Init;
        }

        private void Init()
        {
            var terrain = ProceduralTerrain.Instance;
            var centerX = terrain.Chunks.GetLength(0) / 2;
            var chunk = terrain.Chunks[centerX, 0];
            transform.position = chunk.transform.position + new Vector3(0, terrain.HeightMultiplier * 1.5f, 0);
            _distanceTraveled = 0;
        }

        private void Update()
        {
            var translation = Vector3.forward * Time.deltaTime * Speed;
            transform.Translate(translation);
            _distanceTraveled += translation.magnitude;
            if (_distanceTraveled > ProceduralTerrain.Instance.ChunkSize)
            {
                TraversedChunk?.Invoke();
                _distanceTraveled = 0;
            }
        }
    }
}