using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/*
Class: HexagonBlockOperations.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Class that implements IBlockOperations. Contains methods for manipulating individual hexagonal blocks.

WARNING: Class is bloated
*/
public class HexagonBlockOperations : IBlockOperations
{

    public void BuildBlock(Block block, short x, short y, short z, Chunk chunk)
    {
        Vector3 blockWorldPosition = GetBlockWorldPosition(x, y, z);
        HexagonBlockBuilder.BuildHexagon(block, blockWorldPosition.x, blockWorldPosition.y, blockWorldPosition.z, x, y, z, chunk);
    }


    public void DamageBlock(RaycastHit hit, Chunk chunk)
    {
        Vector3 gridPos = CalculateCoordinates(hit, chunk, false);
        int x = (int)gridPos.x;
        int y = (int)gridPos.y;
        int z = (int)gridPos.z;
        if (!chunk.blocks[x, y, z].Indestructible)
        {
            chunk.blocks[x, y, z].blockCurrentHealth--;
            if (chunk.blocks[x, y, z].blockCurrentHealth == 0)
            {
                AddBlockToInventory(chunk.blocks[x, y, z]);
                SetBlock(Blocks.Get(0), chunk, gridPos, x,y,z);
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
        Vector3 gridPos = CalculateCoordinates(hit, chunk,false);

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


    /*
     * Minor bug - The method will not set blocks that goes more than 1 into other chunks. i.e: x-2, if x==0, then method will only set x-1.
     * */
    public void SetBlock(Block newBlock, Chunk chunk, Vector3 gridPos, int x, int y, int z)
    {
        if (x >= 0 && x < chunk.size_X && y >= 0 && y < chunk.size_Y && z >= 0 && z < chunk.size_Z)
        {
            Block currentBlock = chunk.blocks[x, y, z];
            if (!currentBlock.Indestructible || newBlock.ID != currentBlock.ID)
            {
                if(chunk.state == ChunkState.Chunk_OK)
                {
                    chunk.UpdateBlock(newBlock, x, y, z);
                    chunk.UpdateChunk(this, x, y, z, gridPos, newBlock.ID == 0);
                }
     
            }
        }
        else
        {
            float chunk_X = Settings.currentSettings.chunkLoader.xMod;
            float chunk_Y = Settings.currentSettings.chunkLoader.yMod;
            float chunk_Z = Settings.currentSettings.chunkLoader.zMod;
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
            else if (x < 0 && z < 0)
            {
                otherChunk = Settings.currentSettings.chunkLoader.GetChunk(chunk.position - new Vector3(chunk_X, 0f, chunk_Z));
                otherChunk.UpdateBlock(newBlock, chunk.size_X - 1, y, chunk.size_Z - 1);
                gridPos.x = chunk.size_X - 1;
                gridPos.z = chunk.size_Z - 1;
            }
            else if (x > chunk.size_X - 1 && z > chunk.size_Z - 1)
            {
                otherChunk = Settings.currentSettings.chunkLoader.GetChunk(chunk.position + new Vector3(chunk_X, 0f, chunk_Z));
                otherChunk.UpdateBlock(newBlock, 0, y, 0);
                gridPos.x = 0f;
                gridPos.z = 0f;
            }
            if (otherChunk != null)
            {
                if(otherChunk.state == ChunkState.Chunk_OK)
                {
                    otherChunk.UpdateChunk(this, x, y, z, gridPos, newBlock.ID == 0);
                }
 
                //UpdateNeighbourChunk(gridPos, otherChunk.position);
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
            position -= hit.normal / 6f;
        }
        else
        {
            position += hit.normal / 6f;
        }

        int y = Mathf.FloorToInt(position.y/ Y_Modifier());
        int row = Mathf.FloorToInt(position.z / 0.866f);
        int column = Mathf.FloorToInt(position.x / 1.5f);

        float dy = position.z - (float)row * 0.866f;
        float dx = position.x - (float)column * (1.5f);

        if (((row ^ column) & 1) == 0)
            dy = 0.866f - dy;
        int right = dy * (0.5f) < 0.866f * (dx - 0.5f) ? 1 : 0;
        row += (column ^ row ^ right) & 1;

        column += right;
        row = Mathf.FloorToInt(row / 2f);
        return new Vector3(column, y, row);
    }

    public void UpdateNeighbourChunk(Vector3 blockPosition, Vector3 chunkPosition)
    {
        float chunk_X = Settings.currentSettings.chunkLoader.xMod;
        float chunk_Y = Settings.currentSettings.chunkLoader.yMod;
        float chunk_Z = Settings.currentSettings.chunkLoader.zMod;

        List<Chunk> neighbours = new List<Chunk>();
        if (blockPosition.y == Settings.currentSettings.chunk_Size_Y-1 && chunkPosition.y == 0)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(0, chunk_Y, 0)));
     
        }
        else if(blockPosition.y == 0 && chunkPosition.y > 0)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(0, chunk_Y, 0)));
     
        }
        //Block is in first column : Update left neighbour. 
        else if (blockPosition.x == 0 && blockPosition.z > 0 && blockPosition.z < Settings.currentSettings.chunk_Size_Z-1)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(chunk_X, 0, 0)));
        }
        //Block is in Last column : Update right neighbour. 
        else if (blockPosition.x == Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z > 0 && blockPosition.z < Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(chunk_X, 0, 0)));
        }
        //Block is in first row : Update down neighbour. 
        else if (blockPosition.x > 0 && blockPosition.x < Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == 0)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(0, 0, chunk_Z)));
        }
        //Block is in last row : Update up neighbour. 
        else if (blockPosition.x > 0 && blockPosition.x < Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(0, 0, chunk_Z)));
        }
        //Block is in first column and first row : Update left neighbour, down neighbour and diagonal Left neighbour. 
        else if (blockPosition.x == 0 && blockPosition.z == 0)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(chunk_X, 0, 0)));
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(0, 0, chunk_Z)));
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(chunk_X, 0, chunk_Z)));
        }
        //Block is in first column and last row: Update left neighbour and up neighbour. 
        else if (blockPosition.x == 0 && blockPosition.z == Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(chunk_X, 0, 0)));
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(0, 0, chunk_Z)));
        }
        //Block is in last column and first row: Update right neighbour and down neighbour. 
        else if (blockPosition.x == Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == 0)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(chunk_X, 0, 0)));
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition - new Vector3(0, 0, chunk_Z)));
        }
        //Block is in last column and last row: Update right neighbour and up neighbour and right diagonal neighbour. 
        else if (blockPosition.x == Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(chunk_X, 0, 0)));
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(0, 0, chunk_Z)));
            neighbours.Add(Settings.currentSettings.chunkLoader.GetChunk(chunkPosition + new Vector3(chunk_X, 0, chunk_Z)));
        }
        foreach (Chunk c in neighbours)
        {
            if(c != null && c.IsChunkGenerated())
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
        return 1.25f;
    }

    public float Z_Modifier()
    {
        return 1.732f;
    }

    public Vector3 GetBlockWorldPosition(short x, short y, short z)
    {
        float x_i = 0f;
        float y_i = 0f;
        float z_i = 0f;

        x_i = X_Modifier() * x;
        y_i = Y_Modifier() * y;
        z_i = Z_Modifier() * z;
        if (x % 2 == 1)
        {
            x_i = (X_Modifier() * x);
            z_i = (Z_Modifier() * z) + 0.866f;

        }
        return new Vector3(x_i, y_i, z_i);
    }

    


    private static bool IsInRange(int x, int y, int z)
    {
        return (x >= 0 && x <= Settings.currentSettings.chunk_Size_X-1) && (y >= 0 && y <= Settings.currentSettings.chunk_Size_Y - 1) && (z >= 0 && z <= Settings.currentSettings.chunk_Size_Z - 1);
    }


    public List<Tuple3D<int, int ,int>> GetBlockNeighbours(int x, int y, int z)
    {
        List<Tuple3D<int, int, int>> neighbours = new List<Tuple3D<int, int, int>>();
        if (IsInRange(x, y + 1, z)) neighbours.Add(new Tuple3D<int, int, int>(x, y + 1, z));
        if (IsInRange(x, y - 1, z)) neighbours.Add(new Tuple3D<int, int, int>(x, y - 1, z));
        if (IsInRange(x, y, z+1)) neighbours.Add(new Tuple3D<int, int, int>(x, y, z+1));
        if (IsInRange(x, y, z-1)) neighbours.Add(new Tuple3D<int, int, int>(x, y, z-1));
        if(x % 2 == 1)
        {
            if (IsInRange(x-1, y, z+1)) neighbours.Add(new Tuple3D<int, int, int>(x - 1, y, z + 1));
            if (IsInRange(x+1, y, z+1)) neighbours.Add(new Tuple3D<int, int, int>(x + 1, y, z + 1));
            if (IsInRange(x-1, y, z)) neighbours.Add(new Tuple3D<int, int, int>(x - 1, y, z));
            if (IsInRange(x+1, y, z)) neighbours.Add(new Tuple3D<int, int, int>(x + 1, y, z));
        }
        else
        {
            if (IsInRange(x-1, y, z)) neighbours.Add(new Tuple3D<int, int, int>(x - 1, y, z));
            if (IsInRange(x+1, y, z)) neighbours.Add(new Tuple3D<int, int, int>(x + 1, y, z));
            if (IsInRange(x-1, y, z-1)) neighbours.Add(new Tuple3D<int, int, int>(x - 1, y, z - 1));
            if (IsInRange(x+1, y, z-1)) neighbours.Add(new Tuple3D<int, int, int>(x + 1, y, z - 1));
        }


        return neighbours;
    }


    private bool CheckChunk(Chunk c)
    {
        return c != null && c.IsChunkGenerated();
    }

    public void GetChunkNeighbours(Vector3 chunkPosition, Vector3 blockPosition, ref Dictionary<Vector3, Chunk> neighbours)
    {
        float chunk_X = Settings.currentSettings.chunkLoader.xMod;
        float chunk_Y = Settings.currentSettings.chunkLoader.yMod;
        float chunk_Z = Settings.currentSettings.chunkLoader.zMod;

        List<Vector3> neighbourPositions = new List<Vector3>();
        if (blockPosition.y == Settings.currentSettings.chunk_Size_Y - 1 && chunkPosition.y == 0)
        {
            neighbourPositions.Add(chunkPosition + new Vector3(0, chunk_Y, 0));
       
        }
        else if (blockPosition.y == 0 && chunkPosition.y > 0)
        {
            neighbourPositions.Add(chunkPosition - new Vector3(0, chunk_Y, 0));

        }
        //Block is in first column : Update left neighbour. 
        else if (blockPosition.x == 0 && blockPosition.z > 0 && blockPosition.z < Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbourPositions.Add(chunkPosition - new Vector3(chunk_X, 0, 0));
        }
        //Block is in Last column : Update right neighbour. 
        else if (blockPosition.x == Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z > 0 && blockPosition.z < Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbourPositions.Add(chunkPosition + new Vector3(chunk_X, 0, 0));
        }
        //Block is in first row : Update down neighbour. 
        else if (blockPosition.x > 0 && blockPosition.x < Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == 0)
        {
            neighbourPositions.Add(chunkPosition - new Vector3(0, 0, chunk_Z));
        }
        //Block is in last row : Update up neighbour. 
        else if (blockPosition.x > 0 && blockPosition.x < Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbourPositions.Add(chunkPosition + new Vector3(0, 0, chunk_Z));
        }
        //Block is in first column and first row : Update left neighbour, down neighbour and diagonal Left neighbour. 
        else if (blockPosition.x == 0 && blockPosition.z == 0)
        {
            neighbourPositions.Add(chunkPosition - new Vector3(chunk_X, 0, 0));
            neighbourPositions.Add(chunkPosition - new Vector3(0, 0, chunk_Z));
            neighbourPositions.Add(chunkPosition - new Vector3(chunk_X, 0, chunk_Z));
        }
        //Block is in first column and last row: Update left neighbour and up neighbour. 
        else if (blockPosition.x == 0 && blockPosition.z == Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbourPositions.Add(chunkPosition - new Vector3(chunk_X, 0, 0));
            neighbourPositions.Add(chunkPosition + new Vector3(0, 0, chunk_Z));
        }
        //Block is in last column and first row: Update right neighbour and down neighbour. 
        else if (blockPosition.x == Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == 0)
        {
            neighbourPositions.Add(chunkPosition + new Vector3(chunk_X, 0, 0));
            neighbourPositions.Add(chunkPosition - new Vector3(0, 0, chunk_Z));
        }
        //Block is in last column and last row: Update right neighbour and up neighbour and right diagonal neighbour. 
        else if (blockPosition.x == Settings.currentSettings.chunk_Size_X - 1 && blockPosition.z == Settings.currentSettings.chunk_Size_Z - 1)
        {
            neighbourPositions.Add(chunkPosition + new Vector3(chunk_X, 0, 0));
            neighbourPositions.Add(chunkPosition + new Vector3(0, 0, chunk_Z));
            neighbourPositions.Add(chunkPosition + new Vector3(chunk_X, 0, chunk_Z));
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

