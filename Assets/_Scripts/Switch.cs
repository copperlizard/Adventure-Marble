using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SwitchMode {  TOGGLE, MOMENTARY, TIMED };

public class Switch : MonoBehaviour
{
    public SwitchMode mode;

    [System.Serializable]
    public class refObjects
    {
        public List<GameObject> mutSwitches = new List<GameObject>();
        public List<GameObject> setSwitches = new List<GameObject>();
        public List<GameObject> platforms = new List<GameObject>();
    }
    public refObjects refObs = new refObjects();

    public bool setTarPos, setRotAxis, setTSpeed, setRSpeed, setRTar;
    [System.Serializable]
    public class PlatformProperties
    {
        public Vector3 tarPos, rotAxis;
        public float tSpeed, rSpeed, rTar;
    }
    public PlatformProperties platProps = new PlatformProperties();
    
    public Color col;
    public float intensity, time;

    [HideInInspector]
    public bool on, pressed;

    private Renderer rend;    
    private Color startCol;    

    private bool untouched, litted, butSetOn, forcedOff;

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

        untouched = true;
        litted = true;
        butSetOn = false;
        forcedOff = false;

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

            if (setTarPos) //RECONSIDER THIS!!!!
            {   
                foreach (GameObject plat in refObs.platforms)
                {
                    plat.GetComponent<DynamicPlatform>().tarPos = platProps.tarPos;
                }
            }

            if (setRotAxis)
            {
                foreach (GameObject plat in refObs.platforms)
                {
                    plat.GetComponent<DynamicPlatform>().rotAxis = platProps.rotAxis;
                }
            }

            if (setTSpeed)
            {
                foreach (GameObject plat in refObs.platforms)
                {
                    plat.GetComponent<DynamicPlatform>().tSpeed = platProps.tSpeed;
                }
            }

            if (setRSpeed)
            {
                foreach (GameObject plat in refObs.platforms)
                {
                    plat.GetComponent<DynamicPlatform>().rSpeed = platProps.rSpeed;
                }
            }

            if (setRTar)
            {
                foreach (GameObject plat in refObs.platforms)
                {
                    plat.GetComponent<DynamicPlatform>().rTar = platProps.rTar;
                }
            }

            //Account for mutually exclusive switches
            if (refObs.mutSwitches.Count > 0)
            {
                foreach (GameObject swit in refObs.mutSwitches)
                {
                    swit.GetComponent<Switch>().forceOff();
                }
            }

            //If switch set
            if (refObs.setSwitches.Count > 0)
            {
                butSetOn = true;
                foreach (GameObject swit in refObs.setSwitches)
                {
                    if (!swit.GetComponent<Switch>().on)
                    {
                        butSetOn = false;
                    }
                }
                if (butSetOn)
                {
                    //Do stuff
                    Debug.Log("Switch Set Active!");
                    foreach (GameObject plat in refObs.platforms)
                    {
                        plat.GetComponent<DynamicPlatform>().frozen = false;
                        Debug.Log(plat.name.ToString() + ".frozen = false");
                    }
                }
            }
            //Lone switch
            else
            {
                //Do stuff
                Debug.Log("Switch Active!");
                foreach (GameObject plat in refObs.platforms)
                {
                    plat.GetComponent<DynamicPlatform>().frozen = false;
                    Debug.Log(plat.name.ToString() + ".frozen = false");
                }
            }
        }

       if(!on && litted && !forcedOff)
        {   
            litted = false;
            DynamicGI.SetEmissive(rend, Color.black);
            rend.material.color = startCol;
            Debug.Log("LIGHT OFF!");
            
            foreach (GameObject plat in refObs.platforms)
            {
                plat.GetComponent<DynamicPlatform>().frozen = true;
                Debug.Log(plat.name.ToString() + ".frozen = true");
            }
        }

       if(litted && forcedOff)
        {
            on = false;
            forcedOff = false;
            litted = false;
            DynamicGI.SetEmissive(rend, Color.black);
            rend.material.color = startCol;
            Debug.Log("LIGHT OFF!");
        }
    }

    void forceOff()
    {
        on = false;
        forcedOff = true;
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
