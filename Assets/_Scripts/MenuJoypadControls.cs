using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuJoypadControls : MonoBehaviour
{    
    public bool engaged;
    private Selectable but;
    private float h, v;
    private bool a, aLock, joypadding;

	// Use this for initialization
	void Start ()
    {        
        joypadding = false;        
        aLock = false;        
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (engaged)
        {
            getInput();

            if (joypadding)
            {
                menuSelection();
            }
        }
        else
        {
            if(but != null)
            {
                but = null;
                joypadding = false;
                aLock = false;
            }            
        }        	
	}

    void getInput()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            a = true;
            aLock = true;
        }        

        //ADD B BUTTON? (CLOSE BRANCHER?)

        if ((h != 0.0f || v != 0.0f || a == true) && !joypadding)
        {
            joypadding = true;            
            but = gameObject.GetComponentInChildren<Selectable>();
            but.Select();
        }
    }

    void menuSelection()
    {        
        if (v > 0.0f)
        {                
            but.FindSelectableOnUp();                
        }
        else if (v < 0.0f)
        {                
            but.FindSelectableOnDown();                
        }                       
           
        if (h > 0.0f)
        {                
            but.FindSelectableOnRight();                
        }
        else if (h < 0.0f)
        {                
            but.FindSelectableOnLeft();                
        }                  
        
        if (a && !aLock)
        {                
            but.Select();                
            a = false;                
            //Debug.Log("Select()!");                
        }
    }    
}
