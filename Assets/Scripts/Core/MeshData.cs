using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Core
{
    public struct MeshData : IDisposable
    {
        public NativeArray<float3> Vertices;
        public NativeArray<float2> Uvs;
        public NativeArray<int> Triangles;
        public readonly int Width;
        public readonly int Height;
        public bool IsCreated => Vertices.IsCreated;

        public MeshData(int width, int height)
        {
            Width = width;
            Height = height;
            Vertices = new NativeArray<float3>(width * height, Allocator.Persistent);
            Uvs = new NativeArray<float2>(width * height, Allocator.Persistent);
            Triangles = new NativeArray<int>((width - 1) * (height - 1) * 6, Allocator.Persistent);
        }

        public Mesh CreateMesh()
        {
            var mesh = new Mesh();
            mesh.SetVertices(Vertices);
            mesh.SetUVs(0, Uvs);
            mesh.SetIndices(Triangles, MeshTopology.Triangles, 0);
            mesh.RecalculateNormals();
            return mesh;
        }
        
        public void Dispose()
        {
            if (Vertices.IsCreated)
                Vertices.Dispose();
            if (Uvs.IsCreated)
                Uvs.Dispose();
            if (Triangles.IsCreated)
                Triangles.Dispose();
        }
    }
}