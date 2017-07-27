using UnityEngine;
using System.Collections;

public class CubeBlockBuilder  {

    /**
        Class: CubeBlockBuilder.
        Date: 11-03-2016.
        Author: Louis Fleron.
        Description:
        Class used to create a single cubic block.

         Vector1---------Vector2
               |         |   
               |         |
               |         |
        Vector0 --------- Vector3             
        
         
         WARNING: Class is long!  

     * */

    private static float size = 1.5f;

    public static void BuildCube(Block block, float worldPos_x, float worldPos_y, float worldPos_z, short gridPos_x, short gridPos_y, short gridPos_z, Chunk chunk)
    {

        Vector3 vector0 = new Vector3(worldPos_x* size, worldPos_y* size, worldPos_z* size);
        Vector3 vector1 = new Vector3(worldPos_x* size, worldPos_y* size, worldPos_z* size + size);
        Vector3 vector2 = new Vector3(worldPos_x* size + size, worldPos_y* size, worldPos_z* size + size);
        Vector3 vector3 = new Vector3(worldPos_x* size + size, worldPos_y* size, worldPos_z* size);

        bool oddIndex = false;
        if (gridPos_y % 2 == 1) oddIndex = true;
        //Bottom

            if (!CullFace(block, gridPos_x, (short)(gridPos_y - 1), gridPos_z, chunk))
            {
                AddSideFace(block, vector0, vector1, vector2, vector3, chunk, false, true, oddIndex);
            }
      
        //Top
        if (!CullFace(block, gridPos_x, (short)(gridPos_y + 1), gridPos_z, chunk))
        {
            AddSideFace(block, vector3 + Vector3.up*size, vector2 + Vector3.up * size, vector1 + Vector3.up * size, vector0 + Vector3.up * size, chunk, true, false, oddIndex);
        }

        //Left
        if (!CullFace(block, (short)(gridPos_x-1), gridPos_y, gridPos_z, chunk))
        {
            AddSideFace(block, vector0, vector0 +  Vector3.up * size, vector1 +  Vector3.up * size, vector1, chunk, false, false, oddIndex);
        }
        //Up
        if (!CullFace(block, gridPos_x, gridPos_y, (short)(gridPos_z + 1), chunk))
        {
            AddSideFace(block, vector1, vector1 + Vector3.up * size, vector2 + Vector3.up * size, vector2, chunk, false, false, oddIndex);
        }
        //Right
        if (!CullFace(block, (short)(gridPos_x + 1), gridPos_y, gridPos_z, chunk))
        {
            AddSideFace(block, vector2, vector2 + Vector3.up * size, vector3 + Vector3.up * size, vector3, chunk, false, false, oddIndex);
        }
        //Down
        if (!CullFace(block, gridPos_x, gridPos_y, (short)(gridPos_z - 1), chunk))
        {
            AddSideFace(block, vector3, vector3 + Vector3.up * size, vector0 + Vector3.up * size, vector0, chunk, false, false, oddIndex);
        }
    }




    public static void AddSideFace(Block block, Vector3 sideCornerA, Vector3 sideCornerB, Vector3 sideCornerC, Vector3 sideCornerD, Chunk chunk, bool top, bool bottom, bool oddIndex) //Four vectors
    {
        if (!chunk.CurrentMeshInfo.HasCapacity(4))
        {
            chunk.NewChunkMeshInfo();
        }

        int index = chunk.CurrentMeshInfo.vertices.Count;
        chunk.CurrentMeshInfo.vertices.Add(sideCornerA);
        chunk.CurrentMeshInfo.vertices.Add(sideCornerB);
        chunk.CurrentMeshInfo.vertices.Add(sideCornerC);
        chunk.CurrentMeshInfo.vertices.Add(sideCornerD);
        //uvs

        /*
        float tearOffset = 0f;

        int blockColumn = ((block.ID - 1) % BlockUVDetails.uvColumns);
        int blockRow = Mathf.FloorToInt((block.ID - 1) / BlockUVDetails.uvRows);
        Vector2 uvOffset = new Vector2(blockColumn / (BlockUVDetails.uvColumns * 1f), (tearOffset+blockRow) / (BlockUVDetails.uvRows * 1f));
        */
        Vector2 uv2Offset = DetermineOverlayOffSet(block);

        chunk.CurrentMeshInfo.colors.Add(block.Colorize(oddIndex));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(oddIndex));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(oddIndex));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(oddIndex));

        if (!top && !bottom)
        {
            /*
            chunk.uvs.Add(new Vector2(1f / BlockUVDetails.uvColumns, 0f / BlockUVDetails.uvRows) + uvOffset);
            chunk.uvs.Add(new Vector2(1f / BlockUVDetails.uvColumns, 0.50f / BlockUVDetails.uvRows) + uvOffset);
            chunk.uvs.Add(new Vector2(0.5f / BlockUVDetails.uvColumns, 0.50f / BlockUVDetails.uvRows) + uvOffset);
            chunk.uvs.Add(new Vector2(0.5f / BlockUVDetails.uvColumns, 0f / BlockUVDetails.uvRows) + uvOffset);
            */


            chunk.CurrentMeshInfo.uvs2.Add(new Vector2(1f / 4, 0f/2) + uv2Offset);
            chunk.CurrentMeshInfo.uvs2.Add(new Vector2(1f / 4, 0.50f/2) + uv2Offset);
            chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.5f / 4, 0.50f/2) + uv2Offset);
            chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.5f / 4, 0f/2) + uv2Offset);
        }
        else
        {
            if (top)
            {
                /*
                chunk.uvs.Add(new Vector2(0.50f / BlockUVDetails.uvColumns, 0.50f / BlockUVDetails.uvRows) + uvOffset);
                chunk.uvs.Add(new Vector2(0.50f / BlockUVDetails.uvColumns, 1f / BlockUVDetails.uvRows) + uvOffset);
                chunk.uvs.Add(new Vector2(0.1f / BlockUVDetails.uvColumns, 1f / BlockUVDetails.uvRows) + uvOffset);
                chunk.uvs.Add(new Vector2(0.1f / BlockUVDetails.uvColumns, 0.50f / BlockUVDetails.uvRows) + uvOffset);
                */
                chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.5f / 4, 0.5f/2) + uv2Offset);
                chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.5f / 4, 1f/2) + uv2Offset);
                chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0f / 4, 1f/2) + uv2Offset);
                chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0f / 4, 0.5f/2) + uv2Offset);

            }
            else
            {
                /*
                chunk.uvs.Add(new Vector2(0.5f / BlockUVDetails.uvColumns, 0f / BlockUVDetails.uvRows) + uvOffset);
                chunk.uvs.Add(new Vector2(0.5f / BlockUVDetails.uvColumns, 0.5f / BlockUVDetails.uvRows) + uvOffset);
                chunk.uvs.Add(new Vector2(0.1f / BlockUVDetails.uvColumns, 0.5f / BlockUVDetails.uvRows) + uvOffset);
                chunk.uvs.Add(new Vector2(0.1f / BlockUVDetails.uvColumns, 0f / BlockUVDetails.uvRows) + uvOffset);
                */
                chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.5f / 4, 0f/2) + uv2Offset);
                chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.5f / 4, 0.5f/2) + uv2Offset);
                chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0f / 4, 0.5f/2) + uv2Offset);
                chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0f / 4, 0f/2) + uv2Offset);
            }
        }
        chunk.CurrentMeshInfo.indices.Add(index + 1);
        chunk.CurrentMeshInfo.indices.Add(index + 0);
        chunk.CurrentMeshInfo.indices.Add(index + 2);

        chunk.CurrentMeshInfo.indices.Add(index + 3);
        chunk.CurrentMeshInfo.indices.Add(index + 2);
        chunk.CurrentMeshInfo.indices.Add(index + 0);

        if (block.Solid)
        {
            int colliderIndex = chunk.CurrentMeshInfo.colliderVertices.Count;

            chunk.CurrentMeshInfo.colliderVertices.Add(sideCornerA);
            chunk.CurrentMeshInfo.colliderVertices.Add(sideCornerB);
            chunk.CurrentMeshInfo.colliderVertices.Add(sideCornerC);
            chunk.CurrentMeshInfo.colliderVertices.Add(sideCornerD);

            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 1);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 0);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 2);

            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 3);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 2);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 0);
        }
    }

    public static bool CullFace(Block block, short x, short y, short z, Chunk chunk)
    {
        if (y < 0)
        {
            return true;
        }
        Block other = Blocks.Get(0);
        other = CullChunk(x, y, z, chunk);


        if (other.ID == 0)
            {
                return false;
            }
            else
            {

                if (!block.Transparent)
                {
                    return true;
                }

                return !chunk.blocks[x, y, z].Transparent;
            }
        
    }

    private static Block CullChunk(short x, short y, short z, Chunk chunk)
    {
        Block other;
        float x_c = chunk.position.x;
        float y_c = chunk.position.y;
        float z_c = chunk.position.z;
        if (y > chunk.size_Y - 1)
        {
            other = Blocks.Get(0);
        }
        else if (x < 0 || x > chunk.size_X - 1 || z < 0 || z > chunk.size_Z - 1)
        {

            if (x < 0)
            {
                x_c = chunk.position.x - (1.5f*chunk.size_X);
                x = (short)(chunk.size_X - 1);
            }
            if (x > chunk.size_X - 1)
            {
                x_c = chunk.position.x + (1.5f * chunk.size_X);
                x = 0;
            }
            if (z < 0)
            {
                z_c = chunk.position.z - (1.5f*chunk.size_Z);
                z = (short)(chunk.size_Z - 1);
            }
            if (z > chunk.size_Z - 1)
            {
                z_c = chunk.position.z + (1.5f * chunk.size_Z);
                z = 0;
            }
            Vector3 worldPos = new Vector3(x_c, y_c, z_c);
            Chunk otherChunk = Settings.currentSettings.chunkLoader.GetChunk(worldPos);
            if (otherChunk != null)
            {
                if (otherChunk.IsChunkGenerated())
                {
                    other = otherChunk.blocks[x, y, z];
                }
                else if (otherChunk.IsChunkLoaded() && otherChunk.blocks[x, y, z].LoadedFromDisk)
                {
                    other = otherChunk.blocks[x, y, z];
                }
                else
                {
                    other = chunk.blockGenerator.SneakPeakBlock(worldPos.x, worldPos.y, worldPos.z, x, y, z);
                }


            }
            else
            {
                other = chunk.blockGenerator.SneakPeakBlock(worldPos.x, worldPos.y, worldPos.z, x, y, z);
            }


        }
        else
        {
            other = chunk.blocks[x, y, z];
        }

        return other;
    }



    private static Vector2 DetermineOverlayOffSet(Block block)
    {
        float hp = (block.blockCurrentHealth * 1f) / (block.blockOriginalHealth);

        Vector2 offset = new Vector2();
        if (hp == 1)
        {
            offset.y = 1f;
        }
        else if (hp < 1 && hp >= 0.75f)
        {
            offset.x = 0f;
            offset.y = 0f;
        }
        else if (hp < 0.75f && hp >= 0.50f)
        {
            offset.x = 0.25f;
            offset.y = 0f;
        }
        else if (hp < 0.50 && hp >= 0.25f)
        {
            offset.x = 0.5f;
            offset.y = 0f;
        }
        else if (hp < 0.25 && hp >= 0f)
        {
            offset.x = 0.75f;
            offset.y = 0f;
        }
        return offset;
    }
  

}
