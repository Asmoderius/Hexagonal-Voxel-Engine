using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/*
Class: CubeBlockOperations.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Class that implements IBlockOperations. Contains methods for manipulating individual cubic blocks.

WARNING: Class is bloated
*/
public class CubeBlockOperations : IBlockOperations {

    public void BuildBlock(Block block, short x, short y, short z, Chunk chunk)
    {
        Vector3 blockWorldPosition = GetBlockWorldPosition(x, y, z);
        CubeBlockBuilder.BuildCube(block, blockWorldPosition.x, blockWorldPosition.y, blockWorldPosition.z, x, y, z, chunk);
    }

    public void DamageBlock(RaycastHit hit, Chunk chunk)
    {
        Vector3 gridPos = CalculateCoordinates(hit, chunk,false);
        int x = (int)gridPos.x;
        int y = (int)gridPos.y;
        int z = (int)gridPos.z;
        if (!chunk.blocks[x, y, z].Indestructible)
        {

            chunk.blocks[x, y, z].blockCurrentHealth--;
            if (chunk.blocks[x, y, z].blockCurrentHealth == 0)
            {
                AddBlockToInventory(chunk.blocks[x, y, z]);
                SetBlock(Blocks.Get(0), hit, chunk, false);

            }
        }

    }

    public void PlaceBlock(Block block, RaycastHit hit, Chunk chunk)
    {
        SetBlock(block, hit, chunk, true);
    }

    public void AddBlockToInventory(Block block)
    {
        Settings.currentSettings.Player.GetComponent<PlayerInventory>().AddBlock(block);
    }

    public Block GetBlock(RaycastHit hit, Chunk chunk)
    {
        Vector3 gridPos = CalculateCoordinates(hit, chunk, false);

        return chunk.blocks[(int)gridPos.x, (int)gridPos.y, (int)gridPos.z];
    }

    public void SetBlock(Block newBlock, RaycastHit hit, Chunk chunk, bool reverseNormal)
    {
        Vector3 gridPos = CalculateCoordinates(hit, chunk, reverseNormal);
        int x = (int)gridPos.x;
        int y = (int)gridPos.y;
        int z = (int)gridPos.z;
        SetBlock(newBlock, chunk, gridPos, x, y, z);
    }



    public void SetBlock(Block newBlock, Chunk chunk, Vector3 gridPos, int x, int y, int z)
    {
        
        if (x >= 0 && x < chunk.size_X && y >= 0 && y < chunk.size_Y && z >= 0 && z < chunk.size_Z)
        {
            Block currentBlock = chunk.blocks[x, y, z];
            if (!currentBlock.Indestructible || newBlock.ID != currentBlock.ID)
            {
                chunk.UpdateBlock(newBlock, x, y, z);
                chunk.UpdateChunk(this, x, y, z, gridPos, newBlock.ID == 0);
            }
        }
        else
        {

            float chunk_X = Settings.currentSettings.chunk_Size_X;
            float chunk_Y = Settings.currentSettings.chunk_Size_Y;
            float chunk_Z = Settings.currentSettings.chunk_Size_Z;
            Chunk otherChunk = null;
            if (x < 0 && z >= 0 && z < chunk.size_Z)
            {
                otherChunk = Settings.currentSettings.chunkLoader.GetChunk(chunk.position - new Vector3(chunk_X, 0f, 0f));
                otherChunk.UpdateBlock(newBlock, chunk.size_X - 1, y, z);
                gridPos.x = chunk.size_X - 1;
            }
            else if (x > chunk.size_X - 1 && z >= 0 && z < chunk.size_Z)
            {
                otherChunk = Settings.currentSettings.chunkLoader.GetChunk(chunk.position + new Vector3(chunk_X, 0f, 0f));
                otherChunk.UpdateBlock(newBlock, 0, y, z);
                gridPos.x = 0;
            }
            else if (x >= 0 && x < chunk.size_X && z < 0)
            {
                otherChunk = Settings.currentSettings.chunkLoader.GetChunk(chunk.position - new Vector3(0f, 0f, chunk_Z));
                otherChunk.UpdateBlock(newBlock, x, y, chunk.size_Z - 1);
                gridPos.z = chunk.size_Z - 1;
            }
            else if (x >= 0 && x < chunk.size_X && z > chunk.size_Z - 1)
            {
                otherChunk = Settings.currentSettings.chunkLoader.GetChunk(chunk.position + new Vector3(0f, 0f, chunk_Z));
                otherChunk.UpdateBlock(newBlock, x, y, 0);
                gridPos.z = 0;
            }
            if (otherChunk != null)
            {
                otherChunk.UpdateChunk(this, x, y, z, gridPos, newBlock.ID == 0);
            }

        }

    }


    public Block GetBlock(short x, short y, short z, Chunk chunk)
    {
        return chunk.blocks[x, y, z];
    }

    public Vector3 CalculateCoordinates(RaycastHit hit, Chunk chunk, bool reverseNormal)
    {
        Vector3 position = hit.point - chunk.transform.position;
      
        if (!reverseNormal)
        {
            position -= hit.normal / 4f;
        }
        else
        {
            position += hit.normal / 4f;
        }

        int x = Mathf.FloorToInt(position.x / 1.5f);
        int y = Mathf.FloorToInt(position.y / 1.5f);
        int z = Mathf.FloorToInt(position.z / 1.5f);
        return new Vector3(x, y, z);
    }


    public void UpdateNeighbourChunk(Vector3 blockPosition, Vector3 chunkPosition)
    {
        float chunk_X = Settings.currentSettings.chunk_Size_X;
        float chunk_Y = Settings.currentSettings.chunk_Size_Y;
        float chunk_z = Settings.currentSettings.chunk_Size_Z;
        List<Chunk> neighbours = new List<Chunk>();
        //Update left neighbour
        if (blockPosition.x == 0 && blockPosition.z > 0 && blockPosition.z < Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(1.5f * chunk_X, 0, 0)));
        }
        //Update right neighbour
        else if (blockPosition.x == Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z > 0 && blockPosition.z < Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(1.5f * chunk_X, 0, 0)));
        }
        //Update down neighbour
        else if (blockPosition.x > 0 && blockPosition.x < Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == 0)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(0, 0, 1.5f * chunk_z)));
        }
        //Update up neighbour
        else if (blockPosition.x > 0 && blockPosition.x < Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(0, 0, 1.5f * chunk_z)));
        }
        //Update left and down neighbours
        else if (blockPosition.x == 0 && blockPosition.z == 0)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(1.5f * chunk_X, 0, 0)));
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(0, 0, 1.5f * chunk_z)));
        }
        //Update left and up neighbours
        else if (blockPosition.x == 0 && blockPosition.z == Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(1.5f * chunk_X, 0, 0)));
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(0, 0, 1.5f * chunk_z)));
        }
        //Update right and down neighbours
        else if (blockPosition.x == Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == 0)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(1.5f * chunk_X, 0, 0)));
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(0, 0, 1.5f * chunk_z)));
        }
        //Update right and up neighbours
        else if (blockPosition.x == Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(1.5f * chunk_X, 0, 0)));
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(0, 0, 1.5f * chunk_z)));
        }
        foreach (Chunk c in neighbours)
        {
            if (c != null && c.IsChunkGenerated())
            {
                c.UpdateChunk();
            }
        }
    }

    public float X_Modifier()
    {
        return 1.5f;
    }

    public float Y_Modifier()
    {
        return 1.5f;
    }

    public float Z_Modifier()
    {
        return 1.5f;
    }

    public Vector3 GetBlockWorldPosition(short x, short y, short z)
    {
        return new Vector3(x, y, z);
    }


    private static bool IsInRange(int x, int y, int z)
    {
        return (x >= 0 && x <= Settings.currentSettings.chunk_Size_X - 1) && (y >= 0 && y <= Settings.currentSettings.chunk_Size_Y - 1) && (z >= 0 && z <= Settings.currentSettings.chunk_Size_Z - 1);
    }

    public List<Tuple3D<int, int ,int>> GetBlockNeighbours(int x, int y, int z)
    {
        List<Tuple3D<int, int, int>> neighbours = new List<Tuple3D<int, int, int>>();
        if (IsInRange(x, y + 1, z)) neighbours.Add(new Tuple3D<int, int, int>(x, y + 1, z));
        if (IsInRange(x, y - 1, z)) neighbours.Add(new Tuple3D<int, int, int>(x, y - 1, z));
        if (IsInRange(x, y, z + 1)) neighbours.Add(new Tuple3D<int, int, int>(x, y, z + 1));
        if (IsInRange(x, y, z - 1)) neighbours.Add(new Tuple3D<int, int, int>(x, y, z - 1));    
        if (IsInRange(x - 1, y, z)) neighbours.Add(new Tuple3D<int, int, int>(x - 1, y, z));
        if (IsInRange(x + 1, y, z)) neighbours.Add(new Tuple3D<int, int, int>(x + 1, y, z));

        return neighbours;

    }

    public void GetChunkNeighbours(Vector3 chunkPosition, Vector3 blockPosition, ref Dictionary<Vector3, Chunk> neighbours)
    {
        float chunk_X = Settings.currentSettings.chunkLoader.xMod;
        float chunk_Y = Settings.currentSettings.chunkLoader.yMod;
        float chunk_z = Settings.currentSettings.chunkLoader.zMod;

        List<Vector3> neighbourPositions = new List<Vector3>();
        //Update left neighbour
        if (blockPosition.x == 0 && blockPosition.z > 0 && blockPosition.z < Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbourPositions.Add((chunkPosition - new Vector3(1.5f * chunk_X, 0, 0)));
        }
        //Update right neighbour
        else if (blockPosition.x == Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z > 0 && blockPosition.z < Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbourPositions.Add((chunkPosition + new Vector3(1.5f * chunk_X, 0, 0)));
        }
        //Update down neighbour
        else if (blockPosition.x > 0 && blockPosition.x < Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == 0)
        {
            neighbourPositions.Add((chunkPosition - new Vector3(0, 0, 1.5f * chunk_z)));
        }
        //Update up neighbour
        else if (blockPosition.x > 0 && blockPosition.x < Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbourPositions.Add((chunkPosition + new Vector3(0, 0, 1.5f * chunk_z)));
        }
        //Update left and down neighbours
        else if (blockPosition.x == 0 && blockPosition.z == 0)
        {
            neighbourPositions.Add((chunkPosition - new Vector3(1.5f * chunk_X, 0, 0)));
            neighbourPositions.Add((chunkPosition - new Vector3(0, 0, 1.5f * chunk_z)));
        }
        //Update left and up neighbours
        else if (blockPosition.x == 0 && blockPosition.z == Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbourPositions.Add((chunkPosition - new Vector3(1.5f * chunk_X, 0, 0)));
            neighbourPositions.Add((chunkPosition + new Vector3(0, 0, 1.5f * chunk_z)));
        }
        //Update right and down neighbours
        else if (blockPosition.x == Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == 0)
        {
            neighbourPositions.Add((chunkPosition + new Vector3(1.5f * chunk_X, 0, 0)));
            neighbourPositions.Add((chunkPosition - new Vector3(0, 0, 1.5f * chunk_z)));
        }
        //Update right and up neighbours
        else if (blockPosition.x == Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbourPositions.Add((chunkPosition + new Vector3(1.5f * chunk_X, 0, 0)));
            neighbourPositions.Add((chunkPosition + new Vector3(0, 0, 1.5f * chunk_z)));
        }
        foreach (Vector3 n_Pos in neighbourPositions)
        {
            if (!neighbours.ContainsKey(n_Pos))
            {
                Chunk c = Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + n_Pos);
                if (c != null && c.IsChunkGenerated())
                {
                    neighbours.Add(n_Pos, c);
                }
            }
        }
    }
}
