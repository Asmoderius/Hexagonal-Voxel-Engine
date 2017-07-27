using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class SaveFile
{
    public string name;
    public Dictionary<BlockGridPosition, short> changes;

    public SaveFile(string name, Chunk chunk)
    {
        this.name = name;
        this.changes = chunk.GetModifiedBlocks();
    }

}
