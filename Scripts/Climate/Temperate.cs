using UnityEngine;
using System.Collections;
using System;
/*
Class: Temperate.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Temperate inherits from Climate class. Temperate is used to store and retrieve biomes within an Temperate climate. 
*/
public class Temperate : Climate {

    private float ideal_Temperature_Start = 16;
    private float ideal_Temperature_End = 25;

    public Temperate()
    {
        biomes = new Biome[1];
        biomes[0] = new Grassland();
       // biomes[1] = new Highland();
       // biomes[2] = new Mountain();

    }
    public override float Bid(float temperatureValue)
    {
        float bid = 0f;
        if (temperatureValue >= ideal_Temperature_Start && temperatureValue < ideal_Temperature_End)
        {
            bid += 1;
        }
        return bid;
    }

    public override Biome GetIdealBiome(float temperatureValue, float moistureValue, int groundHeight)
    {
        Biome idealBiome = biomes[0];
        float bestBid = 0f;
        foreach (Biome biome in biomes)
        {
            float bid = biome.Bid(temperatureValue, moistureValue, groundHeight);
            if (bid > bestBid)
            {
                bestBid = bid;
                idealBiome = biome;
            }
        }
        return idealBiome;
    }

}
