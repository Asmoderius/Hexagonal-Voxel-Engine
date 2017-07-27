using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
Class: PlayerInventory.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
NOTE: 

THIS CLASS IS A DUMMY, USED ONLY FOR TESTING. REPLACE WITH PROPER SYSTEM.

*/

public class PlayerInventory : MonoBehaviour 
{
    Dictionary<Block, int> blocks;

    void Start()
    {
        blocks = new Dictionary<Block, int>(100);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddBlock(Block block)
    {
        if(blocks.ContainsKey(block))
        {
            blocks[block]++;
        }
        else
        {          
            blocks.Add(block, 1);
        }
       
    }

    public void RemoveBlock(Block block)
    {
        if (blocks.ContainsKey(block))
        {
            blocks[block]--;
            if (blocks[block] == 0)
            {
                blocks.Remove(block);
            }
        }
    }
    
}
