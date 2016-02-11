using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ButtonBrancher : MonoBehaviour
{
    public enum ScaleMode { whmatch, whindependent }
    public bool revealOnStart;

    [System.Serializable]
    public class animSettings
    {
        public float tSmooth, fSmooth;
        
        [HideInInspector]
        public bool opening, spawned;
    }

    private class buttonScaler
    {
        private Vector2 refButtonSize, refScreenSize;
        private ScaleMode smode;

        public Vector2 newButtonSize;

        public void init(Vector2 butSize, Vector2 screenSize, ScaleMode scaleMode)
        {
            refButtonSize = butSize;
            refScreenSize = screenSize;
            smode = scaleMode;
            setButtonSize();
        }

        public void setButtonSize()
        {
            if(smode == ScaleMode.whindependent)
            {
                newButtonSize.x = (refButtonSize.x * Screen.width) / refScreenSize.x;
                newButtonSize.y = (refButtonSize.y * Screen.height) / refScreenSize.y;
            }
            else if(smode == ScaleMode.whmatch)
            {
                newButtonSize.x = (refButtonSize.x * Screen.width) / refScreenSize.x;
                newButtonSize.y = newButtonSize.x;
            }
        }
    }

    [System.Serializable]
    public class linSpawner
    {
        public Vector2 spawnDir;
        public float baseButtonSpacing;
        public int buttonNumOffset;
        private Vector2 refScreenSize;

        [HideInInspector]
        public float buttonSpacing;

        public void init(Vector2 screenSize)
        {
            refScreenSize = screenSize;
            fitSpacingtoScreenSize();
        }

        public void fitSpacingtoScreenSize()
        {
            float refScreenFloat = (refScreenSize.x + refScreenSize.y) / 2.0f;
            float screenFloat = (Screen.width + Screen.height) / 2.0f;
            buttonSpacing = (baseButtonSpacing * screenFloat) / refScreenFloat;
        }
    }

    public ScaleMode mode;
    public Vector2 refButtonSize, refScreenSize;

    private float lastScreenWidth, lastScreenHeight;

    public GameObject[] buttonRefs;
    private List<GameObject> buttons;

    public animSettings animSets = new animSettings();
    private buttonScaler butScaler = new buttonScaler();
    public linSpawner linSpawn = new linSpawner();    

	// Use this for initialization
	void Start ()
    {        
        buttons = new List<GameObject>();

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        butScaler.init(refButtonSize, refScreenSize, mode);
        linSpawn.init(refScreenSize);	

        if(revealOnStart)
        {
            spawnButtons();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;

            butScaler.init(refButtonSize, refScreenSize, mode);
            linSpawn.init(refScreenSize);
        }

        if(animSets.opening)
        {
            if(!animSets.spawned)
            {
                spawnButtons();
            }

            linearReveal();
        }
        else
        {
            if(animSets.spawned)
            {
                //Clear current button list
                for (int i = 0; i < buttons.Count; i++)
                {
                    Destroy(buttons[i]);
                }
                buttons.Clear();
                animSets.spawned = false;
            }
        }
	
	}

    public void toggle()
    {
        //Debug.Log(gameObject.name.ToString() + " TOGGLE!");
        animSets.opening = !animSets.opening;
    }

    void spawnButtons()
    {
        if(!animSets.spawned)
        {
            animSets.opening = true;

            //Clear current button list
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(buttons[i]);
            }
            buttons.Clear();

            clearCommonButtonBranchers();

            for (int i = 0; i < buttonRefs.Length; i++)
            {
                GameObject b = Instantiate(buttonRefs[i] as GameObject);
                b.transform.SetParent(transform);
                b.transform.position = transform.position;
                buttons.Add(b);
            }

            animSets.spawned = true;
        }        
    }

    void clearCommonButtonBranchers()
    {
        GameObject[] branchers = GameObject.FindGameObjectsWithTag("ButtonBrancher");

        foreach(GameObject brancher in branchers)
        {
            //Check if common brancher
            if(brancher.transform.parent == transform.parent)
            {
                ButtonBrancher mb = brancher.GetComponent<ButtonBrancher>();
                for(int i = 0; i < mb.buttons.Count; i++ )
                {
                    Destroy(mb.buttons[i]);
                }
                mb.buttons.Clear();
            }
        }
    }

    void linearReveal()
    {        
        for(int i = 0; i < buttons.Count; i++)
        {
            RectTransform butRect = buttons[i].GetComponent<RectTransform>();
            butRect.sizeDelta = butScaler.newButtonSize;

            Vector3 targetPos = butRect.transform.position;
            targetPos.x = linSpawn.spawnDir.x * ((i + linSpawn.buttonNumOffset) * (butRect.sizeDelta.x + linSpawn.buttonSpacing)) + transform.position.x;
            targetPos.y = linSpawn.spawnDir.y * ((i + linSpawn.buttonNumOffset) * (butRect.sizeDelta.y + linSpawn.buttonSpacing)) + transform.position.y;

            butRect.position = Vector3.Lerp(butRect.position, targetPos, animSets.tSmooth * Time.deltaTime);
        }

    }
}
