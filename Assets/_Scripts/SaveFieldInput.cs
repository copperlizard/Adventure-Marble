using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SaveFieldInput : MonoBehaviour
{
    private InputField inField;

	// Use this for initialization
	void Start ()
    {
        inField = gameObject.GetComponent<InputField>();	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void SaveInput()
    {
        PlayerPrefs.SetString("PlayerName", inField.text);        
    }
}
