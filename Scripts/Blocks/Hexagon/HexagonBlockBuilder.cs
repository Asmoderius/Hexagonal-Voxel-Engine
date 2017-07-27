using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;

/*
Class: HexagonBlockBuilder.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Class used to create an individual hexagonal block

    WARNING: Class is long!
*/

public class HexagonBlockBuilder
{
    public static float side = 1f;
    public static float height = 1.732f;
    public static float half_height = 0.866f;
    public static float diagonal = 2f;

    private static Vector3 offsetA = new Vector3(-0.5f,0f,-half_height);
    private static Vector3 offsetB = new Vector3(-1f, 0f, 0f);
    private static Vector3 offsetC = new Vector3(-0.5f, 0f,  half_height);
    private static Vector3 offsetD = new Vector3( 0.5f, 0f,  half_height);
    private static Vector3 offsetE = new Vector3( 1f, 0f, 0f);
    private static Vector3 offsetF = new Vector3( 0.5f, 0f, -half_height);
    private static Vector3 upVector = new Vector3(0f, 1.25f, 0f);

    public static void BuildHexagon(Block block, float worldPos_x, float worldPos_y, float worldPos_z, short gridPos_x, short gridPos_y, short gridPos_z, Chunk chunk)
    {
        bool oddIndex = false;
        if (gridPos_y % 2 == 1)
        {
            oddIndex = true;
        }
        Vector3 origin = new Vector3(worldPos_x, worldPos_y, worldPos_z);
        //Top
        if (!CullFace(block, gridPos_x, (short)(gridPos_y + 1), gridPos_z, chunk))
        {
            AddHexagonalTopFace(block, origin + offsetA + upVector, origin + offsetB + upVector, origin + offsetC + upVector, origin + offsetD + upVector, origin + offsetE + upVector, origin + offsetF + upVector, chunk, oddIndex);
        }

        //Bottom
        if (!CullFace(block, gridPos_x, (short)(gridPos_y - 1), gridPos_z, chunk))
        {
            AddHexagonalBottomFace(block, origin + offsetA, origin + offsetB, origin + offsetC, origin + offsetD, origin + offsetE, origin + offsetF, chunk);
        }

        //Up
        if (!CullFace(block, gridPos_x, gridPos_y, (short)(gridPos_z + 1), chunk))
        {
            AddSideFace(block, origin + offsetC, origin + offsetC + upVector, origin + offsetD + upVector, origin + offsetD, chunk, oddIndex);
        }
        //Down
        if (!CullFace(block, gridPos_x, gridPos_y, (short)(gridPos_z - 1), chunk))
        {
            AddSideFace(block, origin + offsetF, origin + offsetF + upVector, origin + offsetA + upVector, origin + offsetA, chunk, oddIndex);
        }
        if (gridPos_x % 2 == 0)
        {
            //DownLeft
            if (!CullFace(block, (short)(gridPos_x - 1), gridPos_y, (short)(gridPos_z - 1), chunk))
            {
                AddSideFace(block, origin + offsetA, origin + offsetA + upVector, origin + offsetB + upVector, origin + offsetB, chunk, oddIndex);
            }
            //UpLeft
            if (!CullFace(block, (short)(gridPos_x - 1), gridPos_y, gridPos_z, chunk))
            {
                AddSideFace(block, origin + offsetB, origin + offsetB + upVector, origin + offsetC + upVector, origin + offsetC, chunk, oddIndex);
            }

            //UpRight
            if (!CullFace(block, (short)(gridPos_x + 1), gridPos_y, gridPos_z, chunk))
            {
                AddSideFace(block, origin + offsetD, origin + offsetD + upVector, origin + offsetE + upVector, origin + offsetE, chunk, oddIndex);
            }
            //DownRight
            if (!CullFace(block, (short)(gridPos_x + 1), gridPos_y, (short)(gridPos_z - 1), chunk))
            {
                AddSideFace(block, origin + offsetE, origin + offsetE + upVector, origin + offsetF + upVector, origin + offsetF, chunk, oddIndex);
            }
        }
        else
        {
            //DownLeft
            if (!CullFace(block, (short)(gridPos_x - 1), gridPos_y, (gridPos_z), chunk))
            {
                AddSideFace(block, origin + offsetA, origin + offsetA + upVector, origin + offsetB + upVector, origin + offsetB, chunk, oddIndex);
            }
            //UpLeft
            if (!CullFace(block, (short)(gridPos_x - 1), gridPos_y, (short)(gridPos_z + 1), chunk))
            {
                AddSideFace(block, origin + offsetB, origin + offsetB + upVector, origin + offsetC + upVector, origin + offsetC, chunk, oddIndex);
            }

            //UpRight
            if (!CullFace(block, (short)(gridPos_x + 1), gridPos_y, (short)(gridPos_z + 1), chunk))
            {
                AddSideFace(block, origin + offsetD, origin + offsetD + upVector, origin + offsetE + upVector, origin + offsetE, chunk, oddIndex);
            }
            //DownRight
            if (!CullFace(block, (short)(gridPos_x + 1), gridPos_y, gridPos_z, chunk))
            {
                AddSideFace(block, origin + offsetE, origin + offsetE + upVector, origin + offsetF + upVector, origin + offsetF, chunk, oddIndex);
            }
        }



    }


    public static void AddHexagonalTopFace(Block block, Vector3 vectorA, Vector3 vectorB, Vector3 vectorC, Vector3 vectorD, Vector3 vectorE, Vector3 vectorF, Chunk chunk, bool oddIndex) //Four vectors
    {
        if(!chunk.CurrentMeshInfo.HasCapacity(6))
        {
            chunk.NewChunkMeshInfo();
        }

        int index = chunk.CurrentMeshInfo.vertices.Count;
        chunk.CurrentMeshInfo.vertices.Add(vectorA);
        chunk.CurrentMeshInfo.vertices.Add(vectorB);
        chunk.CurrentMeshInfo.vertices.Add(vectorC);
        chunk.CurrentMeshInfo.vertices.Add(vectorD);
        chunk.CurrentMeshInfo.vertices.Add(vectorE);
        chunk.CurrentMeshInfo.vertices.Add(vectorF);
        //uvs
        /*
        int blockColumn = ((block.ID - 1) % BlockUVDetails.uvColumns);
        int blockRow = Mathf.FloorToInt((block.ID - 1) / BlockUVDetails.uvRows);
        Vector2 uvOffset = new Vector2(blockColumn / (BlockUVDetails.uvColumns * 1f), blockRow / (BlockUVDetails.uvRows * 1f));
        */

        /*
        chunk.uvs.Add(new Vector2(0.125f / BlockUVDetails.uvColumns, 0.5f / BlockUVDetails.uvRows) + uvOffset);
        chunk.uvs.Add(new Vector2(0f / BlockUVDetails.uvColumns, 0.75f / BlockUVDetails.uvRows) + uvOffset);
        chunk.uvs.Add(new Vector2(0.125f / BlockUVDetails.uvColumns, 1f / BlockUVDetails.uvRows) + uvOffset);
        chunk.uvs.Add(new Vector2(0.375f / BlockUVDetails.uvColumns, 1f / BlockUVDetails.uvRows) + uvOffset);
        chunk.uvs.Add(new Vector2(0.5f / BlockUVDetails.uvColumns, 0.75f / BlockUVDetails.uvRows) + uvOffset);
        chunk.uvs.Add(new Vector2(0.375f / BlockUVDetails.uvColumns, 0.5f / BlockUVDetails.uvRows) + uvOffset);
        */

        chunk.CurrentMeshInfo.colors.Add(block.Colorize(oddIndex));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(oddIndex));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(oddIndex));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(oddIndex));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(oddIndex));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(oddIndex));



        Vector2 uv2Offset = DetermineOverlayOffSet(block);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.125f / 4, 0.5f / 2) + uv2Offset);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0f / 4, 0.75f / 2) + uv2Offset);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.125f / 4, 1f / 2) + uv2Offset);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.375f / 4, 1f / 2) + uv2Offset);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.5f / 4, 0.75f / 2) + uv2Offset);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.375f / 4, 0.5f / 2) + uv2Offset);

        chunk.CurrentMeshInfo.indices.Add(index + 0);
        chunk.CurrentMeshInfo.indices.Add(index + 4);
        chunk.CurrentMeshInfo.indices.Add(index + 5);

        chunk.CurrentMeshInfo.indices.Add(index + 0);
        chunk.CurrentMeshInfo.indices.Add(index + 3);
        chunk.CurrentMeshInfo.indices.Add(index + 4);

        chunk.CurrentMeshInfo.indices.Add(index + 0);
        chunk.CurrentMeshInfo.indices.Add(index + 1);
        chunk.CurrentMeshInfo.indices.Add(index + 3);

        chunk.CurrentMeshInfo.indices.Add(index + 1);
        chunk.CurrentMeshInfo.indices.Add(index + 2);
        chunk.CurrentMeshInfo.indices.Add(index + 3);

    
       
        if (block.Solid)
        {
            int colliderIndex = chunk.CurrentMeshInfo.colliderVertices.Count;

            chunk.CurrentMeshInfo.colliderVertices.Add(vectorA);
            chunk.CurrentMeshInfo.colliderVertices.Add(vectorB);
            chunk.CurrentMeshInfo.colliderVertices.Add(vectorC);
            chunk.CurrentMeshInfo.colliderVertices.Add(vectorD);
            chunk.CurrentMeshInfo.colliderVertices.Add(vectorE);
            chunk.CurrentMeshInfo.colliderVertices.Add(vectorF);

            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 0);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 4);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 5);

            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 0);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 3);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 4);

            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 0);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 1);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 3);

            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 1);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 2);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 3);
        }
    }

    public static void AddHexagonalBottomFace(Block block, Vector3 vectorA, Vector3 vectorB, Vector3 vectorC, Vector3 vectorD, Vector3 vectorE, Vector3 vectorF, Chunk chunk) //Four vectors
    {
        if (!chunk.CurrentMeshInfo.HasCapacity(6))
        {
            chunk.NewChunkMeshInfo();
        }

        int index = chunk.CurrentMeshInfo.vertices.Count;
        chunk.CurrentMeshInfo.vertices.Add(vectorA);
        chunk.CurrentMeshInfo.vertices.Add(vectorB);
        chunk.CurrentMeshInfo.vertices.Add(vectorC);
        chunk.CurrentMeshInfo.vertices.Add(vectorD);
        chunk.CurrentMeshInfo.vertices.Add(vectorE);
        chunk.CurrentMeshInfo.vertices.Add(vectorF);
        //uvs
        /*
        int blockColumn = ((block.ID - 1) % BlockUVDetails.uvColumns);
        int blockRow = Mathf.FloorToInt((block.ID - 1) / BlockUVDetails.uvRows);
        Vector2 uvOffset = new Vector2(blockColumn / (BlockUVDetails.uvColumns * 1f), blockRow / (BlockUVDetails.uvRows * 1f));
        
        chunk.uvs.Add(new Vector2(0.125f / BlockUVDetails.uvColumns, 0f / BlockUVDetails.uvRows) + uvOffset);
        chunk.uvs.Add(new Vector2(0f / BlockUVDetails.uvColumns, 0.25f / BlockUVDetails.uvRows) + uvOffset);
        chunk.uvs.Add(new Vector2(0.125f / BlockUVDetails.uvColumns, 0.5f / BlockUVDetails.uvRows) + uvOffset);
        chunk.uvs.Add(new Vector2(0.375f / BlockUVDetails.uvColumns, 0.5f / BlockUVDetails.uvRows) + uvOffset);
        chunk.uvs.Add(new Vector2(0.5f / BlockUVDetails.uvColumns, 0.25f / BlockUVDetails.uvRows) + uvOffset);
        chunk.uvs.Add(new Vector2(0.375f / BlockUVDetails.uvColumns, 0f / BlockUVDetails.uvRows) + uvOffset);
        */
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(true));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(true));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(true));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(true));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(true));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(true));

        Vector2 uv2Offset = DetermineOverlayOffSet(block);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.125f / 4, 0f / 2) + uv2Offset);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0f / 4, 0.25f / 2) + uv2Offset);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.125f / 4, 0.5f / 2) + uv2Offset);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.375f / 4, 0.5f / 2) + uv2Offset);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.5f / 4, 0.25f / 2) + uv2Offset);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.375f / 4, 0f / 2) + uv2Offset);

        chunk.CurrentMeshInfo.indices.Add(index + 4);
        chunk.CurrentMeshInfo.indices.Add(index + 0);
        chunk.CurrentMeshInfo.indices.Add(index + 5);

        chunk.CurrentMeshInfo.indices.Add(index + 3);
        chunk.CurrentMeshInfo.indices.Add(index + 0);
        chunk.CurrentMeshInfo.indices.Add(index + 4);

        chunk.CurrentMeshInfo.indices.Add(index + 1);
        chunk.CurrentMeshInfo.indices.Add(index + 0);
        chunk.CurrentMeshInfo.indices.Add(index + 3);

        chunk.CurrentMeshInfo.indices.Add(index + 2);
        chunk.CurrentMeshInfo.indices.Add(index + 1);
        chunk.CurrentMeshInfo.indices.Add(index + 3);


        if (block.Solid)
        {
            int colliderIndex = chunk.CurrentMeshInfo.colliderVertices.Count;

            chunk.CurrentMeshInfo.colliderVertices.Add(vectorA);
            chunk.CurrentMeshInfo.colliderVertices.Add(vectorB);
            chunk.CurrentMeshInfo.colliderVertices.Add(vectorC);
            chunk.CurrentMeshInfo.colliderVertices.Add(vectorD);
            chunk.CurrentMeshInfo.colliderVertices.Add(vectorE);
            chunk.CurrentMeshInfo.colliderVertices.Add(vectorF);

            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 4);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 0);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 5);

            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 3);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 0);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 4);

            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 1);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 0);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 3);

            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 2);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 1);
            chunk.CurrentMeshInfo.colliderIndices.Add(colliderIndex + 3);
        }
    }


    public static void AddSideFace(Block block, Vector3 sideCornerA, Vector3 sideCornerB, Vector3 sideCornerC, Vector3 sideCornerD, Chunk chunk, bool oddIndex) //Four vectors
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
        if (sideCornerA.y % 2 == 1) tearOffset = 0.5f;

        int blockColumn = ((block.ID - 1) % BlockUVDetails.uvColumns);
        int blockRow = Mathf.FloorToInt((block.ID - 1) / BlockUVDetails.uvRows);
        Vector2 uvOffset = new Vector2(blockColumn / (BlockUVDetails.uvColumns * 1f), (tearOffset + blockRow) / (BlockUVDetails.uvRows * 1f));

    
        chunk.uvs.Add(new Vector2(1f / BlockUVDetails.uvColumns, 0f / BlockUVDetails.uvRows) + uvOffset);
        chunk.uvs.Add(new Vector2(1f / BlockUVDetails.uvColumns, 0.50f / BlockUVDetails.uvRows) + uvOffset);
        chunk.uvs.Add(new Vector2(0.5f / BlockUVDetails.uvColumns, 0.50f / BlockUVDetails.uvRows) + uvOffset);
        chunk.uvs.Add(new Vector2(0.5f / BlockUVDetails.uvColumns, 0f / BlockUVDetails.uvRows) + uvOffset);
        */
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(oddIndex));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(!oddIndex));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(!oddIndex));
        chunk.CurrentMeshInfo.colors.Add(block.Colorize(oddIndex));


        Vector2 uv2Offset = DetermineOverlayOffSet(block);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(1f / 4, 0f / 2)+uv2Offset);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(1f / 4, 0.50f / 2) + uv2Offset);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.5f / 4, 0.50f / 2) + uv2Offset);
        chunk.CurrentMeshInfo.uvs2.Add(new Vector2(0.5f / 4, 0f / 2) + uv2Offset);


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
        if(y < 0)
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
        try
        {
            Block other = Blocks.Get(0);

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
                    x_c = chunk.position.x - (1.5f * chunk.size_X);
                    x = (short)(chunk.size_X - 1);
                }
                if (x > chunk.size_X - 1)
                {
                    x_c = chunk.position.x + (1.5f * chunk.size_X);
                    x = 0;
                }

                if (z < 0)
                {
                    z_c = chunk.position.z - (1.732f * chunk.size_Z);

                    z = (short)(chunk.size_Z - 1);
                }
                if (z > chunk.size_Z - 1)
                {
                    z_c = chunk.position.z + (1.732f * chunk.size_Z);
                    z = 0;
                }
                Vector3 worldPos = new Vector3(x_c, y_c, z_c);
                Chunk otherChunk = Settings.currentSettings.chunkLoader.GetChunk(worldPos);
                if (otherChunk != null)
                {
                    if(otherChunk.IsChunkGenerated())
                    {
                        other = otherChunk.blocks[x, y, z];
                    }
                    else if(otherChunk.IsChunkLoaded() && otherChunk.blocks[x,y,z].LoadedFromDisk)
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
        catch(System.Exception any)
        {
            string s = "ErrorReport.txt";
            if (File.Exists(s))
            {
                File.Delete(s);
            }
            StreamWriter writer = new StreamWriter(s);
            writer.WriteLine("Chunk error at : ");
            writer.WriteLine(chunk.position.ToString());
            writer.WriteLine("Coordinates : " + x + " " + y + " " + z);
            writer.WriteLine("Error: ");
            writer.WriteLine(any.ToString());
            writer.Flush();
            writer.Close();
            throw any;
        }
        

     
    }

  

    private static Vector2 DetermineOverlayOffSet(Block block)
    {
        float hp = (block.blockCurrentHealth*1f) / (block.blockOriginalHealth);

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
