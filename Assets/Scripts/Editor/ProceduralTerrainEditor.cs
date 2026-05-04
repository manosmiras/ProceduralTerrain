using MonoBehaviours;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ProceduralTerrain))]
    public class ProceduralTerrainEditor : UnityEditor.Editor
    {
        private const int MaxChunkSize = 256;
        private const int MinChunkSize = 8;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            var chunkSizeProperty = serializedObject.FindProperty("ChunkSize");
            if (chunkSizeProperty != null)
            {
                chunkSizeProperty.intValue = Mathf.Clamp(chunkSizeProperty.intValue, MinChunkSize, MaxChunkSize);
            }

            serializedObject.ApplyModifiedProperties();

            var terrain = (ProceduralTerrain)target;

            if (GUILayout.Button("Generate"))
            {
                terrain.InitializeChunks();
            }
        }
    }
}