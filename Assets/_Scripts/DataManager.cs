using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class DataManager
{
    public static List<SaveData> saves = new List<SaveData>();
    public static bool loaded = false;

    static public int numLvls = 10;

    public static void save()
    {
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
            //Debug.Log("LOADING FILE!");

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            DataManager.saves = (List<SaveData>)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            //Debug.Log("FILE DOES NOT EXIST!\nCreating Save Data!");
            resetSave();
        }
    }
    
    public static void resetSave()
    {
        //Debug.Log("clearing save data!");
        saves.Clear(); //ensure fresh list
        for (int i = 0; i < numLvls; i++)
        {
            saves.Add(new SaveData());  //New lvl save data
            saves[i].lvlID = i;         //Assign lvl ID

            //Debug.Log("new SaveData.lvlID == " + saves[i].lvlID.ToString());
        }
        save();
    }	
}
