using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ProceduralTerrain))]
    public class ProceduralTerrainEditor : UnityEditor.Editor
    {
        private const int MaxWidth = 256;
        private const int MinWidth = 16;
        private const int MaxHeight = 256;
        private const int MinHeight = 16;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            var dimensionsProperty = serializedObject.FindProperty("dimensions");
            if (dimensionsProperty != null)
            {
                var xProp = dimensionsProperty.FindPropertyRelative("x");
                var yProp = dimensionsProperty.FindPropertyRelative("y");

                xProp.intValue = Mathf.Clamp(xProp.intValue, MinWidth, MaxWidth);
                yProp.intValue = Mathf.Clamp(yProp.intValue, MinHeight, MaxHeight);
            }

            serializedObject.ApplyModifiedProperties();

            var terrain = (ProceduralTerrain)target;

            if (GUILayout.Button("Generate"))
            {
                terrain.Generate();
            }
        }
    }
}