using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void UpdateCompleteEvent();

public class ChunkUpdateHandler
{
    public event UpdateCompleteEvent UpdateComplete;
    private Dictionary<Vector3, Chunk> chunksUpdating = new Dictionary<Vector3, Chunk>();

    public void RegisterUpdate(Chunk c)
    {
        lock(this)
        {
            chunksUpdating.Add(c.position, c);
        }

    }

    public void UpdateCompleted(Chunk c)
    {
        lock(this)
        {
            chunksUpdating.Remove(c.position);
            if (chunksUpdating.Count == 0)
            {
                if (UpdateComplete != null)
                {
                    UpdateComplete();
                }
            }
        }
    }

}
