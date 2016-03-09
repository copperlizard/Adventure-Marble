using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleMarbleTrail : MonoBehaviour
{
    private bool active;

	// Use this for initialization
	void Start ()
    {
        if(PlayerPrefs.GetInt("MarbleTrail", 0) == 1)
        {
            GetComponent<Toggle>().isOn = true;
            active = true;
        }
        else
        {
            GetComponent<Toggle>().isOn = false;
            active = false;
        }	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void toggleTrailState()
    {
        if (!active)
        {
            PlayerPrefs.SetInt("MarbleTrail", 1);
            GetComponent<Toggle>().isOn = true;
            active = true;
        }
        else
        {
            PlayerPrefs.SetInt("MarbleTrail", 0);
            GetComponent<Toggle>().isOn = false;
            active = false;
        }
    }
}
