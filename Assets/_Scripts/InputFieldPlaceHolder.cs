using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputFieldPlaceHolder : MonoBehaviour
{
    private string nameTxt, pholderTxt;

	// Use this for initialization
	void Start ()
    {
        pholderTxt = GetComponent<Text>().text;
        nameTxt = PlayerPrefs.GetString("PlayerName", "");
        if (nameTxt != "")
        {
            GetComponent<Text>().text = nameTxt;
        }
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(nameTxt != PlayerPrefs.GetString("PlayerName", ""))
        {
            nameTxt = PlayerPrefs.GetString("PlayerName", "");
            if(nameTxt != "")
            {
                GetComponent<Text>().text = nameTxt;
            }
            else
            {
                GetComponent<Text>().text = pholderTxt;
            }
        }	
	}
}
