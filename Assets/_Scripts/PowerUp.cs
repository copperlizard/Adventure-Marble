using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

    public GameObject player;
    public string power;
    public float respawnTime;

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update () {
	
	}

    float getRespawnTime()
    {
        return respawnTime;
    }

    void collected()
    {
        object[] parameters = new object[3];
        parameters[0] = power;
        parameters[1] = 5.0f;
        parameters[2] = gameObject;
        player.BroadcastMessage("setPower", parameters);
    }
}
