using UnityEngine;
using System.Collections;

public class MarbleTrail : MonoBehaviour
{
    void Awake ()
    {
        int state = PlayerPrefs.GetInt("MarbleTrail", 0);

        if (state == 0)
        {
            gameObject.SetActive(false);
        }
    }

	// Use this for initialization
	void Start ()
    {       

    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}    
}
