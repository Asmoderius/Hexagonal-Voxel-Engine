using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
/*
Class: Chunk.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Chunk, primary component that contains all the blocks and methods for generating, building and displaying the chunk. 
*/


public class Chunk : MonoBehaviour
{
    public BlockGenerator blockGenerator;
    public ChunkState state = ChunkState.Chunk_Empty;
    public ChunkMesh chunkMeshPrefab;

    public bool DetailsGenerated = false;
    internal short size_X = 1;
    internal short size_Y = 1;
    internal short size_Z = 1;

    internal Block[,,] blocks;
    private Dictionary<BlockGridPosition, short> modifiedBlocks = new Dictionary<BlockGridPosition, short>();

   // private bool ChunkUpdated = false; //Used to indicate that chunk should be serialized to disk. 
    internal Vector3 position;

    //private
    public short ChunkAge;
    private ChunkMeshInfo currentMeshInfo;
    private List<ChunkMeshInfo> chunkMeshInfos = new List<ChunkMeshInfo>();
    private List<ChunkMesh> chunkMeshes = new List<ChunkMesh>();
    private ChunkMesh currentMesh;
    private bool chunk_updated = false;
    private bool built = false;
    private float createTime;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Check if chunk is stuck in a very rare and utterly strange thread race condition. 
        if(state == ChunkState.Chunk_Building && !built)
        {
            if(Time.time - createTime > 10f)
            {
                Debug.Log("Chunk is stuck at building - restarting " + this.position.ToString() + " " + state.ToString());
                Create(); //Recreating. Wasting a WorkerThread on this... BAD! Fix or reload world
            }
        }    
    }

    void FixedUpdate()
    {

        if (state == ChunkState.Chunk_Built)
        {
            StartCoroutine(Render());

            if (chunk_updated)
            {
                Settings.currentSettings.chunkUpdater.UpdateCompleted(this);
                chunk_updated = false;
            }
        }

    }

    void OnApplicationQuit()
    {
        if(this.state != ChunkState.Chunk_Destroying)
        {
            this.state = ChunkState.Chunk_Destroying;
            if (modifiedBlocks.Count > 0) Settings.currentSettings.chunkLoader.DeleteChunk(this);
        }
    }

    public void UpdateBlock(Block newBlock, int x, int y, int z)
    {
        blocks[x, y, z] = newBlock;
        BlockGridPosition blockGridKey = new BlockGridPosition(x, y, z);
        if(modifiedBlocks.ContainsKey(blockGridKey))
        {
            modifiedBlocks[blockGridKey] = newBlock.ID;
        }
        else
        {
            modifiedBlocks.Add(new BlockGridPosition(x, y, z), newBlock.ID);
        }

    }

    public void Create()
    {
        this.position = transform.position;
        this.createTime = Time.time;
        blockGenerator = new BlockGenerator();
        if (Settings.currentSettings.multiThreaded)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                try
                {
                    Generate();
                    Build();
                    ChunkAge = 0;
                }
                catch(Exception e)
                {
                    throw e;
                }
            }), null);
        }
        else
        {
            Generate();
            Build();
            ChunkAge = 0;

        }

    }


    private void Generate()
    {
        state = ChunkState.Chunk_Generating;
        size_X = Settings.currentSettings.chunk_Size_X;
        size_Y = Settings.currentSettings.chunk_Size_Y;
        size_Z = Settings.currentSettings.chunk_Size_Z;
        blocks = new Block[size_X, size_Y, size_Z];

        modifiedBlocks = new Dictionary<BlockGridPosition, short>();
        Dictionary<Vector3, Chunk> neighbours = LoadChunk();
        for (short x = 0; x < size_X; x++)
        {
            for (short z = 0; z < size_Z; z++)
            {
                blockGenerator.GenerateBlockColumn(x, z, this);
            }
        }
        AddDetails();
        state = ChunkState.Chunk_Generated;
        if(neighbours != null)
        {
            foreach (Chunk neighbour in neighbours.Values)
            {
                neighbour.UpdateChunk();
            }
        }
    }



    private Dictionary<Vector3, Chunk> LoadChunk()
    {
        SaveFile saved = Settings.currentSettings.fileHandler.Load(position, false);
        if (saved != null)
        {
            Dictionary<Vector3, Chunk> neighbours = new Dictionary<Vector3, Chunk>();
            foreach (KeyValuePair<BlockGridPosition, short> p in saved.changes)
            {
                blocks[p.Key.x, p.Key.y, p.Key.z] = Blocks.Get(p.Value);
                blocks[p.Key.x, p.Key.y, p.Key.z].LoadedFromDisk = true;
                modifiedBlocks.Add(p.Key, p.Value);
                Settings.currentSettings.Operations.GetCurrentBlockOperations().GetChunkNeighbours(this.position, new Vector3(p.Key.x, p.Key.y, p.Key.z), ref neighbours);
            }
            state = ChunkState.Chunk_Loaded;
            return neighbours;
        }
        else
        {
            return null;
        }

    }



    private void AddDetails()
    {
        
    }


    private void Build()
    {
        if (state == ChunkState.Chunk_Generated || state == ChunkState.Chunk_Updating || state == ChunkState.Chunk_Disabled)
        {
            NewChunkMeshInfo();
            state = ChunkState.Chunk_Building;
            for (short x = 0; x < size_X; x++)
            {
                for (short z = 0; z < size_Z; z++)
                {
                    for (short y = (short)(size_Y - 1); y >= 0; y--)
                    { 
                        Block block = blocks[x, y, z];           
                        if (block.ID != 0)
                        {
                            if (Settings.currentSettings.RemoveFloatingBlocks)
                            {
                                if (IsFloating(x, y, z))
                                {
                                    block.ID = 19; //Sets block to Error block - For testing. Will change it to 0 after test. 
                                }
                            }
                            blockGenerator.BuildBlock(block, x, y, z, this);
                            if(state != ChunkState.Chunk_Building) //Check if chunk has been built. Return from method and cancel. 
                            {
                                return;
                            }
                        }
                    }
                }
            }
            state = ChunkState.Chunk_Built;
            built = true;
        }
    }

    public bool IsChunkGenerated()
    {
        return state != ChunkState.Chunk_Generating && state != ChunkState.Chunk_Empty && state != ChunkState.Chunk_Destroying && state != ChunkState.Chunk_Loaded;
    }

    public bool IsChunkLoaded()
    {
        return state == ChunkState.Chunk_Loaded;
    }

    public bool IsFloating(short x, short y, short z)
    {
        bool isFloating = true;
        List<Tuple3D<int,int,int>> neighbourIndices = Settings.currentSettings.Operations.GetBlockNeighbours(x, y, z);
        foreach (Tuple3D<int,int,int> tuple in neighbourIndices)
        {
            if(blocks[tuple.x, tuple.y, tuple.z].ID != 0)
            {
                isFloating = false;
            }
        }
        return isFloating;
    }



    /*
     * Updates the chunk when it is a neighbour. Method is called from BlockOperations and Chunk itself...
     * */
    public void UpdateChunk()
    {
        if (this.state == ChunkState.Chunk_OK)
        {
            this.state = ChunkState.Chunk_Updating;
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                {
                    Settings.currentSettings.chunkUpdater.RegisterUpdate(this);
                    Build();
                    chunk_updated = true;

                }), null);
        }
        else
        {
            Settings.currentSettings.chunkLoader.UpdateChunk(this);
        }
    }

    /*
     * Allows for threading building of chunk and update of neighbours. Note that I had to move the neighbour updating into this method else it would spawn a racing condition and break the update. 
     **/
    internal void UpdateChunk(IBlockOperations blockOperations, int x, int y, int z, Vector3 gridPos, bool removedBlock)
    {
        if (this.state != ChunkState.Chunk_Updating)
        {
            this.state = ChunkState.Chunk_Updating;
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                if (removedBlock)
                {
                    if (x == 0 || x == size_X - 1 || z == 0 || z == size_Z - 1)
                    {
                        blockOperations.UpdateNeighbourChunk(gridPos, position);
                        Settings.currentSettings.chunkUpdater.UpdateComplete += new UpdateCompleteEvent(BuildWhenUpdateCompleted);
                    }
                    else
                    {
                        Build();
                    }
                }
                else
                {
                    Build();
                    if (x == 0 || x == size_X - 1 || z == 0 || z == size_Z - 1)
                    {
                        blockOperations.UpdateNeighbourChunk(gridPos, position);
                    }
                }


            }), null);
        }

    }


    private void BuildWhenUpdateCompleted()
    {
        Build();
        Settings.currentSettings.chunkUpdater.UpdateComplete -= new UpdateCompleteEvent(BuildWhenUpdateCompleted);
    }

    public ChunkMeshInfo CurrentMeshInfo
    {
        get
        {
            return currentMeshInfo;
        }
    }

    public void NewChunkMeshInfo()
    {
        currentMeshInfo = new ChunkMeshInfo();
        chunkMeshInfos.Add(currentMeshInfo);
    }


    public IEnumerator Render()
    {
        if(chunkMeshes.Count == chunkMeshInfos.Count)
        {
            for (int i = 0; i < chunkMeshInfos.Count; i++)
            {
                chunkMeshes[i].Render(chunkMeshInfos[i]);
            }
            chunkMeshInfos.Clear();
        }
        else
        {
            DestroyChunkMeshes();
            for (int i = 0; i < chunkMeshInfos.Count; i++)
            {
                ChunkMesh chunkMesh = Instantiate(chunkMeshPrefab, position, Quaternion.identity) as ChunkMesh;
                chunkMesh.SetParent(this);
                chunkMeshes.Add(chunkMesh);
                chunkMesh.Render(chunkMeshInfos[i]);
            }
            chunkMeshInfos.Clear();
        }
            state = ChunkState.Chunk_OK;
        yield return null;
    }

    private void DestroyChunkMeshes()
    {
        foreach (ChunkMesh oldChunkMesh in chunkMeshes)
        {
            Destroy(oldChunkMesh.gameObject);
        }
        chunkMeshes.Clear();
    }



    public void IncrementChunkAge()
    {
        ChunkAge++;
        if(ChunkAge > 10 && ChunkAge < 16)
        {
            state = ChunkState.Chunk_Disabled;
            this.gameObject.SetActive(false);
          
        }
        else if(ChunkAge >= 16 && this.state != ChunkState.Chunk_Destroying)
        {
            this.state = ChunkState.Chunk_Destroying;
            Settings.currentSettings.chunkLoader.DeleteChunk(this);
        }
    }

    public void Renew()
    {
        ChunkAge = 0;
        if(state == ChunkState.Chunk_Disabled)
        {
            this.gameObject.SetActive(true);
        }
    }


    public Dictionary<BlockGridPosition, short> GetModifiedBlocks()
    {
        return this.modifiedBlocks;
    }

    

}


