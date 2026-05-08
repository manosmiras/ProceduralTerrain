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

            for (var y = 0; y < verticesPerLineY; y++)
            {
                var sourceY = Mathf.Min(y * step, height - 1);

                for (var x = 0; x < verticesPerLineX; x++)
                {
                    var sourceX = Mathf.Min(x * step, width - 1);
                    var vertexIndex = x + y * verticesPerLineX;

                    var heightSample = heightMap[sourceX, sourceY];
                    meshData.Vertices[vertexIndex] = new Vector3(
                        topLeftX + sourceX,
                        (_heightCurve.Evaluate(heightSample) + heightSample) * _heightMultiplier,
                        topLeftZ - sourceY
                    );

                    meshData.Uvs[vertexIndex] = new Vector2(
                        sourceX / (float)(width - 1),
                        sourceY / (float)(height - 1)
                    );

                    if (x < verticesPerLineX - 1 && y < verticesPerLineY - 1)
                    {
                        meshData.AddTriangle(
                            vertexIndex,
                            vertexIndex + verticesPerLineX + 1,
                            vertexIndex + verticesPerLineX
                        );

                        meshData.AddTriangle(
                            vertexIndex + verticesPerLineX + 1,
                            vertexIndex,
                            vertexIndex + 1
                        );
                    }
                }
            }

            return meshData;
        }
    }
}