using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
Interface: IBlockOperations.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Interface used to describe individual block functionality. 
*/
public interface IBlockOperations
{
    void BuildBlock(Block block, short x, short y, short z, Chunk chunk);
    Block GetBlock(RaycastHit hit,  Chunk chunk);
    Block GetBlock(short x, short y, short z, Chunk chunk);
    Vector3 CalculateCoordinates(RaycastHit hit, Chunk chunk, bool reverseNormal);
    void SetBlock(Block newBlock, RaycastHit hit, Chunk chunk, bool reverseNormal);
    void DamageBlock(RaycastHit hit, Chunk chunk);
    void PlaceBlock(Block block, RaycastHit hit, Chunk chunk);
    void AddBlockToInventory(Block block);
    void UpdateNeighbourChunk(Vector3 position, Vector3 chunkPosition);
    void GetChunkNeighbours(Vector3 chunkPosition, Vector3 blockPosition, ref Dictionary<Vector3, Chunk> neighbours);
    float X_Modifier();
    float Y_Modifier();
    float Z_Modifier();
    Vector3 GetBlockWorldPosition(short x, short y, short z);
    List<Tuple3D<int, int, int>> GetBlockNeighbours(int x, int y, int z);
}