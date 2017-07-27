using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

public class SaveFileBuffer
{
    private HashSet<string> chunksOnDisk = new HashSet<string>();
    private string chunkFile = "chunks.xml";
    public SaveFileBuffer()
    {
        string s = Settings.currentSettings.fileHandler.SaveLocation()+ chunkFile;
        if (File.Exists(s))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HashSet<string>));
            StreamReader reader = new StreamReader(s);
            chunksOnDisk = (HashSet<string>)serializer.Deserialize(reader);
         
        }
    }

    public void RegisterChunkOnDisk(string b)
    {
        if (!chunksOnDisk.Contains(b)) chunksOnDisk.Add(b);
    }

    public bool CheckChunkOnDisk(string b)
    {
        return chunksOnDisk.Contains(b);
    }

    public void FlushChunkRegistry()
    {
        if(chunksOnDisk.Count > 0)
        {
            string s = Settings.currentSettings.fileHandler.SaveLocation() + chunkFile;
            XmlSerializer serializer = new XmlSerializer(typeof(HashSet<string>));
            StreamWriter writer = new StreamWriter(s);
            serializer.Serialize(writer, chunksOnDisk);
            writer.Close();
        }

    }


}
