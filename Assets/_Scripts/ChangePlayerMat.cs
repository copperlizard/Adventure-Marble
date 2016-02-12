using UnityEngine;
using System.Collections;

public class ChangePlayerMat : MonoBehaviour
{
    public int numMats;    

	// Use this for initialization
	void Start ()
    {
        	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void prevMat()
    {        
        PlayerPrefs.SetInt("MarbleMat", Mathf.Clamp(PlayerPrefs.GetInt("MarbleMat", -1) - 1, 0, numMats - 1));
    }

    public void nextMat()
    {
        PlayerPrefs.SetInt("MarbleMat", Mathf.Clamp(PlayerPrefs.GetInt("MarbleMat", -1) + 1, 0, numMats - 1));
    }
}
