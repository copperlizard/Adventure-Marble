using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SwitchMode {  TOGGLE, MOMENTARY, TIMED };

public class Switch : MonoBehaviour
{
    public SwitchMode mode;
    public List<GameObject> mutSwitches = new List<GameObject>();
    public List<GameObject> setSwitches = new List<GameObject>();
    public List<GameObject> platforms = new List<GameObject>();
    public Color col;
    public float intensity, time;

    [HideInInspector]
    public bool on, pressed, timOn;

    private Renderer rend;    
    private Color startCol;
    private bool untouched, litted, butSetOn;

    IEnumerator timeDelayOff()
    {
        yield return new WaitForSeconds(time);
        on = false;
        DynamicGI.SetEmissive(rend, col * 0.25f);
    }

    // Use this for initialization
    void Start ()
    {
        on = false;
        pressed = false;
        timOn = false;        

        untouched = true;
        litted = false;
        butSetOn = false;

        rend = GetComponent<Renderer>();        
        startCol = rend.material.color;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Switch logic
        switch(mode)
        {
            case SwitchMode.TOGGLE:
                toggleSwitch();
                break;
            case SwitchMode.MOMENTARY:
                momemtarySwitch();
                break;
            case SwitchMode.TIMED:
                timedSwitch();
                break;
        }

        //This switch on
        if(on)
        {
            //Account for mutually exclusive switches
            if (mutSwitches.Count > 0)
            {
                foreach(GameObject swit in mutSwitches)
                {
                    swit.GetComponent<Switch>().on = false;
                }
            }

            //If switch set
            if (setSwitches.Count > 0)
            {
                butSetOn = true;
                foreach (GameObject swit in setSwitches)
                {
                    if(!swit.GetComponent<Switch>().on)
                    {
                        butSetOn = false;
                    }
                }
                if(butSetOn)
                {
                    //Do stuff
                    Debug.Log("Switch Set Active!");
                }
            }
            //Lone switch
            else
            {
                //Do stuff
                Debug.Log("Switch Active!");
            }
        }

        lit();        
	}

    void toggleSwitch()
    {
        if(pressed)
        {
            if(untouched)
            {
                on = !on;                
                untouched = false;
            }
        }
    }

    void momemtarySwitch()
    {
        if(pressed)
        {
            on = true;            
        }
        else
        {
            on = false;            
        }
    }

    void timedSwitch()
    {
        if (pressed)
        {
            if (untouched)
            {
                on = true;                
                untouched = false;
                StartCoroutine(timeDelayOff());
            }
        }
    }

    void lit()
    {
       if(on && !litted)
        {            
            litted = true;
            DynamicGI.SetEmissive(rend, col * intensity);
            rend.material.color = col;
            Debug.Log("LIGHT ON!");
        }

       if(!on && litted)
        {   
            litted = false;
            DynamicGI.SetEmissive(rend, Color.black);
            rend.material.color = startCol;
            Debug.Log("LIGHT OFF!");
        }
    }

    void OnCollisionStay(Collision other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Debug.Log("PRESSED!");
            pressed = true;           
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Debug.Log("NOT PRESSED!");
            pressed = false;
            untouched = true;
        }
    }
}
