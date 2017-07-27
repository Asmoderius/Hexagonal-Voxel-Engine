using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
/*
Class: Settings.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
Game settings, can be modified ingame. 
*/
public class Settings : MonoBehaviour
{
    public static Settings currentSettings;
    public string worldName;
    public string seed;
    //Offsets for noise fields
    public Vector3 grain0Offset;
    public Vector3 grain1Offset;
    public Vector3 grain2Offset;
    public Vector3 offsetMoisture;
    public Vector3 offsetTemperature;

    public ChunkLoader chunkLoader;
    public ChunkUpdateHandler chunkUpdater;
    public ChunkFileHandler fileHandler;
    public BuildMode buildMode;
    public Worlds SelectedWorld;
    public World CurrentWorld;
    public bool multiThreaded;

    public BlockOperations Operations;
    public GameObject Player;
    //Chunk
    public short chunk_Size_X = 1;
    public short chunk_Size_Y = 1;
    public short chunk_Size_Z = 1;
    public short seaLevel = 60;

    public short generationDelay = 4;
    public int ViewRange = 1;
    public int LookAheadFactor = 2;
    public bool Pregenerate = false;
    public int PregenerationRange = 10;
    public bool RemoveFloatingBlocks = false;
    //private
    private int hashedSeed;

    void Awake()
    {
        currentSettings = this;
        this.Operations = new BlockOperations();
        chunkUpdater = new ChunkUpdateHandler();
        CurrentWorld = new Earth();
     
    }


    void Update()
    {
    }

    public void StartGame()
    {
        DontDestroyOnLoad(this);
        SetSeed(seed);
        InitializeFileHandlers();
        SceneManager.LoadSceneAsync(1);
    }

    public void GenerateGame()
    {
        DontDestroyOnLoad(this);
        SetSeed(seed);
        Pregenerate = true;
        InitializeFileHandlers();
        StartCoroutine(StartLevelAsync());
    }

    private void InitializeFileHandlers()
    {
        fileHandler = new ChunkFileHandler();
        fileHandler.InitializeBuffer();
    }
    private IEnumerator StartLevelAsync()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(1);
        async.allowSceneActivation = true;
        yield return null;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ChangeBuildMode(int b)
    {
        buildMode = (BuildMode)b;
        this.Operations = new BlockOperations();
    }

    public void SetSeed(string seed)
    {
        this.seed = seed;
        if (seed.Equals(string.Empty))
        {
            hashedSeed = Random.Range(0, int.MaxValue);
            worldName = hashedSeed.ToString();
        }
        else
        {
            hashedSeed = seed.GetHashCode();
            worldName = seed;
        }
        Random.seed = hashedSeed;
      
        grain0Offset = new Vector3(Random.Range(0, 65536), Random.Range(0, 65536), Random.Range(0, 65536));
        grain1Offset = new Vector3(Random.Range(0, 65536), Random.Range(0, 65536), Random.Range(0, 65536));
        grain2Offset = new Vector3(Random.Range(0, 65536), Random.Range(0, 65536), Random.Range(0, 65536));
        offsetMoisture = new Vector3(Random.Range(0, 65536), Random.Range(0, 65536), Random.Range(0, 65536));
        offsetTemperature = new Vector3(Random.Range(0, 65536), Random.Range(0, 65536), Random.Range(0, 65536));
    }

    public void ToggleRemoveFloatingBlocks(bool value)
    {
        this.RemoveFloatingBlocks = value;
    }




}
