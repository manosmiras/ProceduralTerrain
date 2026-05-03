using UnityEngine;

namespace Core
{
    public static class MeshGenerator
    {
        public static MeshData GenerateTerrainMeshOld(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve,
            int lod = 0)
        {
            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);
            var topLeftX = (width - 1) / -2f;
            var topLeftZ = (height - 1) / 2f;
            // TODO: This assumes the chunk size lines up perfectly with the simplification step, need to fix
            var meshSimplificationIncrement = lod == 0 ? 1 : lod * 2;
            var verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

            var meshData = new MeshData(verticesPerLine, verticesPerLine);
            var vertexIndex = 0;

            for (var y = 0; y < height; y += meshSimplificationIncrement)
            {
                for (var x = 0; x < width; x += meshSimplificationIncrement)
                {
                    var heightSample = heightMap[x, y];
                    meshData.Vertices[vertexIndex] = new Vector3(topLeftX + x,
                        (heightCurve.Evaluate(heightSample) + heightSample) * heightMultiplier, topLeftZ - y);
                    meshData.Uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                    if (x < width - 1 && y < height - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                        meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }
            return meshData;
        }
    
        public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve,
            int lod = 0)
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
                        (heightCurve.Evaluate(heightSample) + heightSample) * heightMultiplier,
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