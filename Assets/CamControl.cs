using UnityEngine;
using System.Collections;

public class CamControl : MonoBehaviour {

    public GameObject player;

    public float fDist;

    private Vector3 viewLine;
    private float h;
    private float v;

    
    private Transform temp;

    // Use this for initialization
    void Start () {

        viewLine = transform.position - player.transform.position;
        viewLine = viewLine.normalized;

        transform.position = player.transform.position + viewLine * fDist;

        h = 0.0f;
        v = 0.0f;	
	}
	
    void getInput() {

        h = Input.GetAxis( "rHorizontal" ) * 180.0f;
        v = Input.GetAxis( "rVertical" ) * 180.0f;
    }

	// Update is called once per frame
	void Update () {

        getInput();
	}


    // LateUpdate is called once per frame after Update
    void LateUpdate() {

        //viewLine = transform.position - player.transform.position;

        //transform.position = player.transform.position;

        v = Mathf.Clamp(v, -120.0f, 120.0f);

        Quaternion temp = Quaternion.Euler(v, h, 0.0f);

        transform.position = temp * ( new Vector3(0.0f, 0.0f, -fDist) ) + player.transform.position;
        transform.rotation = temp;
    }
}

/*
Note to self

    The camera can be positioned anywhere (respecting planes of symetry for camera rotation)

    Now I need some trick to make the controls behave the way I want....
*/