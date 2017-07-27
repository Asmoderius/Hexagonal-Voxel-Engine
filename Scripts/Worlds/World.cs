using UnityEngine;
using System.Collections;
/*
Class: World.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
An abstract class used to describe functionality that all world types must contain. Currently acting as interface, will change in future updates. 
*/
public abstract class World
{
    public Worlds worldType;
    public Climate[] climates;

    public abstract Biome GetIdealBiome(float temperature, float moisture, short groundHeight);

    public abstract Tuple<short, short> GetHeight(float noise_X, float noise_Z);


    public abstract float GetUpperGroundHeight(float x, float z, float lowerGroundHeight);

    public abstract float GetLowerGroundHeight(float x, float z);

}
