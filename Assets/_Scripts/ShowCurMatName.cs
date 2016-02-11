using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowCurMatName : MonoBehaviour
{
    public Text displayText;
    public GameObject playerPrefab;

    private int curMat;

	// Use this for initialization
	void Start ()
    {
        curMat = PlayerPrefs.GetInt("MarbleMat", 0);
        displayText.text = playerPrefab.GetComponent<PlayerControl>().marbleMats[curMat].name.ToString();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(curMat != PlayerPrefs.GetInt("MarbleMat", 0))
        {
            curMat = PlayerPrefs.GetInt("MarbleMat", 0);
            displayText.text = playerPrefab.GetComponent<PlayerControl>().marbleMats[curMat].name.ToString();
        }        	
	}
}
