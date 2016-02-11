using UnityEngine;
using System.Collections;

public class ChangePlayerMat : MonoBehaviour
{
    public int numMats;
    private int curMat;

	// Use this for initialization
	void Start ()
    {
        curMat = PlayerPrefs.GetInt("MarbleMat", -1);	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void prevMat()
    {
        curMat++;
        curMat = Mathf.Clamp(curMat, 0, numMats - 1);
        PlayerPrefs.SetInt("MarbleMat", curMat);
    }

    public void nextMat()
    {
        curMat--;
        curMat = Mathf.Clamp(curMat, 0, numMats - 1);
        PlayerPrefs.SetInt("MarbleMat", curMat);
    }
}
