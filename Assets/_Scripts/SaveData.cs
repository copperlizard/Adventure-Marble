using UnityEngine;
using System.Collections;

[System.Serializable]
public class SaveData
{
    public static SaveData current;
    
    static private int numTimes = 10;

    public int lvlID;
    public bool beat;
    public float[] times;
    public string[] names;

    public SaveData()
    {
        //Debug.Log("New SaveData()!");  //REMOVE THIS LATER!!!
               
        beat = false;
        lvlID = -1; //assign somewhere else...

        times = new float[numTimes];
        names = new string[numTimes];

        for(int i = 0; i < numTimes; i++)
        {
            times[i] = 1000000.0f;
            names[i] = "Default";
        }
    }    
}
