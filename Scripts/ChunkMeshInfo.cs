using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkMeshInfo {

    internal List<Vector3> vertices = new List<Vector3>();
    internal List<int> indices = new List<int>();
    internal List<Vector2> uvs = new List<Vector2>();
    internal List<Color32> colors = new List<Color32>();
    internal List<Vector2> uvs2 = new List<Vector2>();
    internal List<Vector3> colliderVertices = new List<Vector3>();
    internal List<int> colliderIndices = new List<int>();

    private const ushort maxVertices = 65334;

    public bool HasCapacity(short addition)
    {
        return vertices.Count+addition <= maxVertices;
    }

    public void Clear()
    {
        vertices.Clear();
        indices.Clear();
        colors.Clear();
        uvs2.Clear();
        colliderVertices.Clear();
        colliderIndices.Clear();
    }
}
