using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ProceduralTerrain))]
    public class ProceduralTerrainEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var terrain = (ProceduralTerrain)target;

            if (GUILayout.Button("Generate"))
            {
                terrain.Generate(seed: Random.Range(0, 100));
            }
        }
    }
}