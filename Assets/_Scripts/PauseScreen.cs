using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseScreen : MonoBehaviour {

    public GameObject cam;
    public Text pausedText;

    public GameObject menuButton;

    private bool press, pause, change;
    
	// Use this for initialization
	void Start ()
    {
        press = pause = change = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        press = Input.GetButtonDown("Pause");

        if(press)
        {
            pause = !pause;
            change = true;
            StartCoroutine(noChange());
        }

        if(change)
        {
            if (pause)
            {
                Time.timeScale = 0.0f;
                AudioListener.pause = true;
                Cursor.visible = true;
                cam.BroadcastMessage("pause", true);
                pausedText.text = "PAUSED";
                menuButton.SetActive(true);
            }
            else
            {
                if (pausedText.text == "PAUSED")
                {
                    Time.timeScale = 1.0f;
                    AudioListener.pause = false;
                    Cursor.visible = false;
                    cam.BroadcastMessage("pause", false);
                    pausedText.text = "";
                    menuButton.SetActive(false);
                }
            }

            change = false;
        }        	
	}

    IEnumerator noChange()
    {
        yield return new WaitForSeconds(0.25f);        
    }
}
