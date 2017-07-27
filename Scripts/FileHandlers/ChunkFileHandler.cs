using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
public class ChunkFileHandler
{
    private SaveFileBuffer buffer;
    private string saveFolderName = @"D:\Projects\Nyx\Nyx\chunks\";
    private static ChunkFileHandler handler;


    public void Save(Chunk chunk)
    {
  
            SaveFile saveFile = new SaveFile(FileName(chunk.position), chunk);
            IFormatter formatter = new BinaryFormatter();
            string s = SaveLocation() + saveFile.name;
            if (File.Exists(s))
            {
                File.Delete(s);
            }
            Stream stream = new FileStream(s, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, saveFile);
            stream.Close();
            buffer.RegisterChunkOnDisk(FileName(chunk.position));
    }

    public void InitializeBuffer()
    {
        buffer = new SaveFileBuffer();
    }

    public SaveFile Load(Vector3 position, bool removeFromBuffer)
    {

        string s = SaveLocation() + FileName(position);
        if (!File.Exists(s))
        {
            return null;
        }
        else
        {
            IFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(s, FileMode.Open);
            SaveFile saved = (SaveFile)formatter.Deserialize(stream);
            stream.Close();
            return saved;
        }
    }




    public void InitializeDirectory()
    {

            string saveLocation = saveFolderName + Settings.currentSettings.worldName;
            if (!Directory.Exists(saveLocation))
            {
                Directory.CreateDirectory(saveLocation);
            }
        

    }

    public string SaveLocation()
    {

            return saveFolderName + Settings.currentSettings.worldName + @"\";
        
    }

    public string FileName(Vector3 chunkLocation)
    {

            string fileName = chunkLocation.x + "," + chunkLocation.y + "," + chunkLocation.z + ".bin";
            return fileName;
        
    }

    public void Flush()
    {

            buffer.FlushChunkRegistry();
        
    }
}
