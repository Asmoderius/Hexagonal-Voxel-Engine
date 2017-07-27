using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
Class: Climate.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Abstract class used to describe Climate functionality.
Note: Currently not using the abstract class beyond being an Interface. Will change in future updates. 
*/
public abstract class Climate
{
    public Biome[] biomes;
    public abstract float Bid(float temperatureValue);
    public abstract Biome GetIdealBiome(float temperatureValue, float moistureValue, int groundHeight);
    

}
