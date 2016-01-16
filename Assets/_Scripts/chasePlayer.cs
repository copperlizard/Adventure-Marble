using UnityEngine;
using System.Collections;

public class chasePlayer : MonoBehaviour {

    public GameObject player;
    //public float damp;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        //transform.position = Vector3.Lerp(transform.position, player.transform.position, damp * Time.deltaTime);
        transform.position = player.transform.position;
    }
}
