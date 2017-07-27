using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
Class: BlockOperations.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Switch class used to create and store the correct type of BlockOperations. 
*/
public class BlockOperations 
{
    IBlockOperations currentBlockOperations;
    public BlockOperations()
    {
 
        switch (Settings.currentSettings.buildMode)
	    {
		    case BuildMode.Cube:
                currentBlockOperations = new CubeBlockOperations();
                break;
            case BuildMode.Hexagon:
                 currentBlockOperations = new HexagonBlockOperations();
                 break;
            default:
                 break;
	     }

    }

    public void DamageBlock(RaycastHit hit, Chunk chunk)
    {
        currentBlockOperations.DamageBlock(hit, chunk);
        chunk.UpdateChunk();
    }

    public void PlaceBlock(Block block, RaycastHit hit, Chunk chunk)
    {
        currentBlockOperations.PlaceBlock(block, hit, chunk);
       
    }

    public Block GetBlock(RaycastHit hit, Chunk chunk)
    {
        return currentBlockOperations.GetBlock(hit, chunk);
    }

    public IBlockOperations GetCurrentBlockOperations()
    {
        return this.currentBlockOperations;
    }


    public List<Tuple3D<int, int, int>> GetBlockNeighbours(int x, int y, int z)
    {
        return currentBlockOperations.GetBlockNeighbours(x, y, z);
    }
}

