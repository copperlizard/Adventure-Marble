using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class DataManager
{
    public static List<SaveData> saves = new List<SaveData>();
    public static bool loaded = false;

    public static void save(SaveData toSave, int saveAt)
    {
        //Check index is valid
        if(saveAt < 0)
        {
            Debug.Log("INVALID SAVE INDEX!");
            return;
        }

        //Check if index in save current data range
        while(saveAt + 1 > saves.Count)
        {
            //Add null data entries until at desired index
            saves.Add(null);
        }

        //Store data at index
        toSave.lvlID = saveAt; //MAYBE DO THIS SOMEWHERE ELSE!!!
        saves[saveAt] = toSave;

        //Save data to computer   TRY TO FIND A WAY TO SAVE ONE PIECE OF THE ARRAY AT A TIME
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
        bf.Serialize(file, DataManager.saves);
        file.Close();
    }

    public static void load()
    {
        if(File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            DataManager.saves = (List<SaveData>)bf.Deserialize(file);
            file.Close();
        }
    }	
}
