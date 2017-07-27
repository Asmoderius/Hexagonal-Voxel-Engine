using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class ChunkMesh : MonoBehaviour {

    private Chunk parent;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetParent(Chunk c)
    {
        this.transform.parent = c.transform;
        this.parent = c;
    }

    public Chunk GetParent()
    {
        return this.parent;
    }

    public void Render(ChunkMeshInfo meshInfo)
    {

        //Visual mesh
        try
        {
            Mesh mesh = new Mesh();

            mesh.vertices = meshInfo.vertices.ToArray();
            mesh.triangles = meshInfo.indices.ToArray();
            mesh.colors32 = meshInfo.colors.ToArray();

            //mesh.uv = uvs.ToArray();
            mesh.uv = meshInfo.uvs2.ToArray();

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.Optimize();

            //Collider mesh
            Mesh colliderMesh = new Mesh();
            colliderMesh.vertices = meshInfo.colliderVertices.ToArray();
            colliderMesh.triangles = meshInfo.colliderIndices.ToArray();
            colliderMesh.RecalculateBounds();
            colliderMesh.RecalculateNormals();
            colliderMesh.Optimize();

            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = colliderMesh;
            meshInfo.Clear();
        }
        catch (Exception e)
        {
            Debug.Log("Chunk : Critical error " + e.ToString());
        }
    }
}
