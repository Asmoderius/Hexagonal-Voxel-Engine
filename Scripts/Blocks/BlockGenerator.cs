using UnityEngine;
/*
Class: BlockGenerator.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Class used to generate individual blocks.
*/
public class BlockGenerator
{

    IBlockOperations currentBlockOperations;
    World currentWorld;
    bool topSoil;


 
    public BlockGenerator()
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

        currentWorld = Settings.currentSettings.CurrentWorld;

    }

   

    public void GenerateBlockColumn(short x, short z, Chunk chunk)
    {
        float noise_X = chunk.position.x + currentBlockOperations.X_Modifier() * x;
        float noise_Z = chunk.position.z + currentBlockOperations.Z_Modifier() * z;
        topSoil = true;

        float temperature = CalculateTemperature(noise_X, noise_Z);
        float moisture = CalculateMoisture(noise_X, noise_Z);
        float uniqueness = CalculateUniqueField(noise_X, noise_Z);
        Tuple<short, short> t = currentWorld.GetHeight(noise_X, noise_Z);

        Biome idealBiome = currentWorld.GetIdealBiome(temperature, 0f, t.x);

        for (short y = (short)(chunk.size_Y - 1); y >= 0; y--) //Generates an entire column, starting at top. 
        {
            if(!chunk.blocks[x,y,z].LoadedFromDisk)
            {
              
                if (y == 0)
                {

                    chunk.blocks[x, y, z] = Blocks.Get(1);

                }
                else
                {
                    if (y < chunk.size_Y - 1)
                    {

                        if (chunk.blocks[x, y + 1, z].ID != 0 || chunk.blocks[x, y + 1, z].LoadedFromDisk)
                        {
                            topSoil = false;
                        }
                        float noise_Y = chunk.position.y + currentBlockOperations.Y_Modifier() * y;
                        Block generatedBlock = idealBiome.GenerateBlock(noise_X, noise_Y, noise_Z, y, t.y, t.x, topSoil, temperature, moisture);
                        chunk.blocks[x, y, z] = generatedBlock;
                    }
        
                }
            }
           
        }
    }
   



    public Block SneakPeakBlock(float chunk_X, float chunk_y, float chunk_z, short x, short y, short z)
    {
 
        float noise_X = chunk_X + currentBlockOperations.X_Modifier() * x;
        float noise_Y = chunk_y + currentBlockOperations.Y_Modifier() * y;
        float noise_Z = chunk_z + currentBlockOperations.Z_Modifier() * z;


        float temperature = CalculateTemperature(noise_X, noise_Z);
        float moisture = CalculateMoisture(noise_X, noise_Z);
        float uniqueness = CalculateUniqueField(noise_X, noise_Z);
        Tuple<short, short> t = currentWorld.GetHeight(noise_X, noise_Z);

        Biome idealBiome = currentWorld.GetIdealBiome(temperature, 0f, t.x);
        return idealBiome.GenerateBlock(noise_X, noise_Y, noise_Z, y, t.y, t.x, true, temperature, moisture);

    }



    private float CalculateTemperature(float noise_X, float noise_Z)
    {
        /*
         * Rework the variables. Make them adjustable. 
         * */
        float temperature = CalculateNoiseValue(noise_X, noise_Z, Settings.currentSettings.grain0Offset, 0.000125f, 20f, 1f);
        temperature += CalculateNoiseValue(noise_X, noise_Z, Settings.currentSettings.grain1Offset, 0.00075f, 20f, 1f);
        temperature += CalculateNoiseValue(noise_X, noise_Z, Settings.currentSettings.grain2Offset, 0.015f, 2f, 1f);
        return temperature;
    }

    private float CalculateMoisture(float noise_X, float noise_Z)
    {
        /*
         * Rework the variables. Make them adjustable. 
         * 
         * DO NOT USE THESE VALUES!!!!!!!!!!!... and !
         * */
        float temperature = CalculateNoiseValue(noise_X, noise_Z, Settings.currentSettings.grain0Offset, 0.000125f, 20f, 1f);
        temperature += CalculateNoiseValue(noise_X, noise_Z, Settings.currentSettings.grain1Offset, 0.00075f, 20f, 1f);
        temperature += CalculateNoiseValue(noise_X, noise_Z, Settings.currentSettings.grain2Offset, 0.015f, 2f, 1f);
        return temperature;
    }

    private float CalculateUniqueField(float noise_X, float noise_Z)
    {
        /*
         * Rework the variables. Make them adjustable. 
         * 
         * DO NOT USE THESE VALUES!!!!!!!!!!!... and !
         * */
        float temperature = CalculateNoiseValue(noise_X, noise_Z, Settings.currentSettings.grain0Offset, 0.000125f, 20f, 1f);
        temperature += CalculateNoiseValue(noise_X, noise_Z, Settings.currentSettings.grain1Offset, 0.00075f, 20f, 1f);
        temperature += CalculateNoiseValue(noise_X, noise_Z, Settings.currentSettings.grain2Offset, 0.015f, 2f, 1f);
        return temperature;
    }



    public void BuildBlock(Block block, short x, short y, short z, Chunk chunk)
    {
        currentBlockOperations.BuildBlock(block, x, y, z, chunk);
    }


    private float CalculateNoiseValue(float x, float z, Vector3 offset, float scale, float max, float power)
    {
        float noise_X = (x + offset.x) * scale;
        float noise_Z = (z + offset.z) * scale;
        if (noise_X < 0) noise_X *= -1;
        if (noise_Z < 0) noise_Z *= -1;

        float noise = (SimplexNoise.SimplexNoise.Generate(noise_X, noise_Z) + 1f) * (max / 2f);
        noise = Mathf.Pow(noise, power);
        return noise;
    }


}





