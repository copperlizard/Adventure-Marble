using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResetSaveButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void reset()
    {
        //Later, make this spawn a confirmation prefab, and use the prefab to reset save data
        DataManager.resetSave();
        gameObject.GetComponentInChildren<Text>().text = "RESET!";
    }
}
