using UnityEngine;
using System.Collections;

[System.Serializable]
public class SaveData
{
    public static SaveData current;

    static public int numLvls = 10;
    static private int numTimes = 10;

    public int lvlID;
    public bool beat;
    public float[] times;
    public string[] names;

    SaveData()
    {       
        beat = false;
        lvlID = -1; //assign somewhere else...

        times = new float[numTimes];
        names = new string[numTimes];

        for(int i = 0; i < numTimes; i++)
        {
            times[i] = 100000000.0f;
            names[i] = "Default";
        }
    }    
}
