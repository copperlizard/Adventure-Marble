using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public Menu curMen;

	// Use this for initialization
	void Start ()
    {
        ShowMenu(curMen);	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void ShowMenu(Menu menu)
    {
        if(curMen != null)
        {
            curMen.isOpen = false;
        }

        curMen = menu;
        curMen.isOpen = true;
    }
}
