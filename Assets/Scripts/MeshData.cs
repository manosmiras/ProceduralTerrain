using UnityEngine;

public class MeshData
{
    public readonly Vector3[] Vertices;
    public readonly Vector2[] Uvs;
    public readonly int Width;
    public readonly int Height;

    private readonly int[] _triangles;
    private int _triangleIndex;

    public MeshData(int width, int height)
    {
        Width = width;
        Height = height;
        Vertices = new Vector3[width * height];
        Uvs = new Vector2[width * height];
        _triangles = new int[(width - 1) * (height - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        _triangles[_triangleIndex] = a;
        _triangles[_triangleIndex + 1] = b;
        _triangles[_triangleIndex + 2] = c;
        _triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        var mesh = new Mesh
        {
            vertices = Vertices,
            triangles = _triangles,
            uv = Uvs
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}