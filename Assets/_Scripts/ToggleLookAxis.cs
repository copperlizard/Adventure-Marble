using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum LOOKAXIS { XAXIS, YAXIS }

public class ToggleLookAxis : MonoBehaviour
{
    public LOOKAXIS axis;

	// Use this for initialization
	void Start ()
    {
        int invert = 0;
        switch (axis)
        {
            case LOOKAXIS.XAXIS:
                invert = PlayerPrefs.GetInt("invertLookX", 1);
                break;
            case LOOKAXIS.YAXIS:
                invert = PlayerPrefs.GetInt("invertLookY", 1);
                break;
        }

        if(invert == -1)
        {
            GetComponent<Toggle>().isOn = true;
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void invertAxis()
    {
        bool invert = GetComponent<Toggle>().isOn;

        switch(axis)
        {
            case LOOKAXIS.XAXIS:
                if (invert)
                {
                    PlayerPrefs.SetInt("invertLookX", -1);
                }
                else
                {
                    PlayerPrefs.SetInt("invertLookX", 1);
                }
                break;
            case LOOKAXIS.YAXIS:
                if (invert)
                {
                    PlayerPrefs.SetInt("invertLookY", -1);
                }
                else
                {
                    PlayerPrefs.SetInt("invertLookY", 1);
                }
                break;
        }
    }
}
