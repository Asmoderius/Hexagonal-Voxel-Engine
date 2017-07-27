using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
/*
Class: ChunkLoader.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
ChunkLoader contains Chunk specific methods, such as generation, loading, finding and storing of chunks. 
*/
public class ChunkLoader : MonoBehaviour {

    private short enableDelay = 0;

    private short currentDelay = 0;
    public Chunk chunkPrefab;
    private int ViewRange;
    private int LookAheadRange;
    private int PregenerationRange;
    public float xMod;
    public float yMod;
    public float zMod;
    private Vector3 playerPosition = Vector3.one;
    Vector3 newPlayerPosition = new Vector3();
    //Dictionary for fast access to chunk, given a specific World-position vector. 
    private Dictionary<Vector3, Chunk> chunks;
    private Queue<Chunk> chunksCreating = new Queue<Chunk>();
    private Queue<Chunk> chunksDeleting = new Queue<Chunk>();
    private Queue<Chunk> chunksUpdating = new Queue<Chunk>();
    // Use this for initialization
    void Start () {

     
        xMod = Settings.currentSettings.Operations.GetCurrentBlockOperations().X_Modifier() * Settings.currentSettings.chunk_Size_X;   
        yMod = Settings.currentSettings.Operations.GetCurrentBlockOperations().Y_Modifier() * Settings.currentSettings.chunk_Size_Y;
        zMod = Settings.currentSettings.Operations.GetCurrentBlockOperations().Z_Modifier() * Settings.currentSettings.chunk_Size_Z;


        ViewRange = Settings.currentSettings.ViewRange*(int)xMod;
        LookAheadRange = ViewRange * Settings.currentSettings.LookAheadFactor;
        PregenerationRange = Settings.currentSettings.PregenerationRange*(int)xMod;


        ThreadPool.SetMinThreads(8, 0);
        ThreadPool.SetMaxThreads(16, 0);

        chunks = new Dictionary<Vector3, Chunk>(10000);
        Settings.currentSettings.fileHandler.InitializeDirectory();
        Blocks.Initialize();
        Settings.currentSettings.chunkLoader = this;
        if (Settings.currentSettings.Pregenerate) PregenerateWorld();


    }

	// Update is called once per frame
	void Update () {

        newPlayerPosition.x = Mathf.FloorToInt(this.transform.position.x / xMod) * xMod;
        newPlayerPosition.z = Mathf.FloorToInt(this.transform.position.z / zMod) * zMod;
        if (newPlayerPosition.x != playerPosition.x || newPlayerPosition.z != playerPosition.z)
        {
           
            GenerateChunks(this.ViewRange);
            UpdateChunks();
            playerPosition = newPlayerPosition;
        }



        if (chunksDeleting.Count > 0)
        {
            Chunk c = chunksDeleting.Dequeue();
            StartCoroutine(DestroyChunk_Routine(c));
        }


    }

    void OnDestroy()
    {
        while(chunksDeleting.Count > 0)
        {
            Chunk c = chunksDeleting.Dequeue();
            if (c.GetModifiedBlocks().Count > 0) Settings.currentSettings.fileHandler.Save(c);
        }
        Settings.currentSettings.fileHandler.Flush();
    }


    public void UpdateChunk(Chunk c)
    {
        chunksUpdating.Enqueue(c);
    }

    private IEnumerator DestroyChunk_Routine(Chunk c)
    {
        if (c.GetModifiedBlocks().Count > 0) Settings.currentSettings.fileHandler.Save(c);
        chunks.Remove(new Vector3(Mathf.FloorToInt(c.position.x), Mathf.FloorToInt(c.position.y), Mathf.FloorToInt(c.position.z)));      
        DestroyObject(c.gameObject);
        yield return null;
    }

    void FixedUpdate()
    {
        if (chunksUpdating.Count > 0)
        {
            Chunk c = chunksUpdating.Dequeue();
            c.UpdateChunk();
        }

        if (chunksCreating.Count > 0)
        {
            if (currentDelay == Settings.currentSettings.generationDelay)
            {
                chunksCreating.Dequeue().Create();
                currentDelay = 0;
            }
            else
            {
                currentDelay++;
            }
        }


    }

    private void UpdateChunks()
    {
        foreach (Chunk c in chunks.Values)
        {
           
            if (c.ChunkAge <=18)
            {
                c.IncrementChunkAge();
            }

        }
    }

    public void DeleteChunk(Chunk chunk)
    {
        chunksDeleting.Enqueue(chunk);
    }

    public void GenerateChunks(int viewRange)
    {
  
        for (float x = this.transform.position.x - LookAheadRange; x <= this.transform.position.x + LookAheadRange; x += xMod)
        {
            for (float z = this.transform.position.z - LookAheadRange; z <= this.transform.position.z + LookAheadRange; z += zMod)
            {
                Vector3 worldPos = new Vector3();
                worldPos.x = Mathf.FloorToInt(x / xMod) * xMod;
                worldPos.y = Mathf.FloorToInt(0 / yMod) * yMod;
                worldPos.z = Mathf.FloorToInt(z / zMod) * zMod;

           
                Vector3 delta = worldPos - transform.position;
                delta.y = 0f;
                if (delta.magnitude <= LookAheadRange)
                {
                    Vector3 index = new Vector3(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z));
                    Chunk chunkInstance;
                    chunks.TryGetValue(index, out chunkInstance);
                    if(chunkInstance != null)
                    {
                        chunkInstance.Renew();
                    }
                    else
                    {
                        if(delta.magnitude <= ViewRange)
                        {
                            chunkInstance = Instantiate(chunkPrefab, worldPos, Quaternion.identity) as Chunk;
                            chunks.Add(index, chunkInstance);
                            chunksCreating.Enqueue(chunkInstance);
                        }
                    }
                }
           

            }
        }
    }




    /*
    Worst function ever! Should be a better mathematical approach - Hmmm how? It is not perfect circular, nor squared. 

    */
    private int CalculatePregeneratedChunkCount(int viewRange)
    {
        int count = 0;
        for (float x = this.transform.position.x - viewRange; x <= this.transform.position.x + viewRange; x += xMod)
        {
            for (float z = this.transform.position.z - viewRange; z <= this.transform.position.z + viewRange; z += zMod)
            {
                Vector3 worldPos = new Vector3();
                worldPos.x = Mathf.FloorToInt(x / xMod) * xMod;
                worldPos.y = Mathf.FloorToInt(0 / yMod) * yMod;
                worldPos.z = Mathf.FloorToInt(z / zMod) * zMod;


                Vector3 delta = worldPos - transform.position;
                delta.y = 0f;
                if (delta.magnitude > viewRange) continue;
                count++;
            }
        }
        return count;
    }

    private void PregenerateWorld()
    {
        int count = CalculatePregeneratedChunkCount(PregenerationRange);
        GenerateChunks(PregenerationRange);
        while(chunksCreating.Count > 0)
        {
            chunksCreating.Dequeue().Create();
        }
    }


    public Chunk GetChunk(Vector3 position)
    {
        Chunk found = null;
        Vector3 index = new Vector3(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y), Mathf.FloorToInt(position.z));
        chunks.TryGetValue(index, out found);
        return found;
    }



}
