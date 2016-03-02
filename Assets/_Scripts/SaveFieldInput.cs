using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SaveFieldInput : MonoBehaviour
{
    private InputField inField;
    private string curName;

    IEnumerator checkDelay()
    {
        //DOESN'T STOP UNTIL OBJECT DESTORYED
        while (true)
        {
            yield return new WaitForSeconds(3);
            curName = PlayerPrefs.GetString("PlayerName", "");
            if (curName != inField.text)
            {
                //...
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
        inField = gameObject.GetComponent<InputField>();
        StartCoroutine(checkDelay());	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void SaveInput()
    {
        PlayerPrefs.SetString("PlayerName", inField.text);        
    }
}
