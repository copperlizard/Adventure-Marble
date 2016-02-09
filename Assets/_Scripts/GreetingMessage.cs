using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GreetingMessage : MonoBehaviour {

    public Text notificationText;

    private string PlayerName;

    // Use this for initialization
    void Start ()
    {
        PlayerName = PlayerPrefs.GetString("PlayerName", "");
        UpdateText();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(PlayerPrefs.GetString("PlayerName", "") != PlayerName)
        {
            PlayerName = PlayerPrefs.GetString("PlayerName", "");
            UpdateText();
        }
	}

    void UpdateText()
    {
        name = PlayerPrefs.GetString("PlayerName", "");
        if (name != "")
        {
            notificationText.text = "Welcome " + name;
        }
        else
        {
            notificationText.text = "Player name not set! Enter player name under Settings.";
        }
    }
}
