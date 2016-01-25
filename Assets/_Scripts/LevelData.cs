using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelData
{
    public static LevelData current;

    static private int numTimes = 10;

    public int lvlID;
    public bool beat;
    public float[] times;
    public string[] names;

    LevelData()
    {
        beat = false;
        lvlID = -1; //assign somewhere else...

        times = new float[numTimes];
        names = new string[numTimes];
    }

    public void setlvlID( int dID ) //probably don't need a setter since lvlID is public...
    {
        lvlID = dID;
    }
}
