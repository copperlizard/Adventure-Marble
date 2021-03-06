﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseScreen : MonoBehaviour {

    public GameObject cam;
    public Text pausedText;

    public GameObject menuButton;

    private HUDScript HUD;
    private bool press, pause, change;
    
	// Use this for initialization
	void Start ()
    {
        press = pause = change = false;
        HUD = gameObject.GetComponent<HUDScript>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        press = Input.GetButtonDown("Pause"); //improve this later

        if(press && !HUD.gameOver) 
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
                gameObject.GetComponent<MenuJoypadControls>().engaged = true;
            }
            else
            {
                if (pausedText.text == "PAUSED")
                {
                    gameObject.GetComponent<MenuJoypadControls>().engaged = false;
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
