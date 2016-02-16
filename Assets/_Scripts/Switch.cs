using UnityEngine;
using System.Collections;

public enum SwitchMode {  TOGGLE, MOMENTARY, TIMED };

public class Switch : MonoBehaviour
{
    public SwitchMode mode;
    public Color col;
    public float intensity, time;

    [HideInInspector]
    public bool on, pressed, timOn;

    private Renderer rend;
    private bool untouched, litted;

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

        rend = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
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
            Debug.Log("LIGHT ON!");
        }

       if(!on && litted)
        {   
            litted = false;
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
