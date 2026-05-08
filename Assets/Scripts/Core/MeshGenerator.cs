using UnityEngine;

namespace Core
{
    public class MeshGenerator
    {
        private float _heightMultiplier;
        private AnimationCurve _heightCurve;
        public MeshGenerator(float heightMultiplier, AnimationCurve heightCurve)
        {
            _heightMultiplier = heightMultiplier;
            _heightCurve = heightCurve;
        }

        public MeshData Generate(float[,] heightMap, int lod = 0)
        {
            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);

            var topLeftX = (width - 1) / -2f;
            var topLeftZ = (height - 1) / 2f;

            var step = lod == 0 ? 1 : lod * 2;

            var verticesPerLineX = Mathf.CeilToInt((width - 1) / (float)step) + 1;
            var verticesPerLineY = Mathf.CeilToInt((height - 1) / (float)step) + 1;

            var meshData = new MeshData(verticesPerLineX, verticesPerLineY);
            
            var count = verticesPerLineX * verticesPerLineY;

            for (var index = 0; index < count; index++)
            {
                var x = index % verticesPerLineX;
                var y = index / verticesPerLineX;

                var sourceX = Mathf.Min(x * step, width - 1);
                var sourceY = Mathf.Min(y * step, height - 1);

                var heightSample = heightMap[sourceX, sourceY];

                meshData.Vertices[index] = new Vector3(
                    topLeftX + sourceX,
                    (_heightCurve.Evaluate(heightSample) + heightSample) * _heightMultiplier,
                    topLeftZ - sourceY
                );

                meshData.Uvs[index] = new Vector2(
                    sourceX / (float)(width - 1),
                    sourceY / (float)(height - 1)
                );

                if (x < verticesPerLineX - 1 && y < verticesPerLineY - 1)
                {
                    meshData.AddTriangle(
                        index,
                        index + verticesPerLineX + 1,
                        index + verticesPerLineX
                    );

                    meshData.AddTriangle(
                        index + verticesPerLineX + 1,
                        index,
                        index + 1
                    );
                }
            }

            return meshData;
        }
    }
}