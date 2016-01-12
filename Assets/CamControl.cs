using UnityEngine;
using System.Collections;

public class CamControl : MonoBehaviour {

    public GameObject player;

    public float fDist, minDist, maxDist, xspeed, yspeed, ymin, ymax;

    private Vector3 viewLine;

    private float h, v;
    private float dist;
    
    private Transform temp;

    private RaycastHit interAt;

    // Use this for initialization
    void Start () {

        viewLine = transform.position - player.transform.position;
        viewLine = viewLine.normalized;

        transform.position = player.transform.position + viewLine * fDist;

        h = 0.0f;
        v = 0.0f;
    }

    float clampAngle(float ang, float min, float max)
    {
        if (ang < -360.0f)
        {
            ang += 360.0f;
        }
        else if (ang > 360.0f)
        {
            ang -= 360.0f;
        }
        return Mathf.Clamp(ang, min, max);
    }

    void getInput() {

        h -= Input.GetAxis("rHorizontal") * xspeed * 0.02f;
        v += Input.GetAxis("rVertical") * yspeed * 0.02f;

        v = clampAngle(v, ymin, ymax);
    }

	// Update is called once per frame
	void Update() {

        getInput();
	}

    // Update called once per physics update
    void FixedUpdate()
    {
        
    }


    // LateUpdate is called once per frame after Update
    void LateUpdate() {

        //Build rotation using input
        Quaternion rotation = Quaternion.Euler(v, h, 0.0f);

        dist = fDist;

        //Place and rotate camera transform based on player transform and user input
        transform.position = rotation * (new Vector3(0.0f, 0.0f, -dist)) + player.transform.position;
        transform.rotation = rotation;
        
        //Check for intersection along view ray and move camera if necessary
        if (Physics.Raycast(transform.position, player.transform.position, out interAt))
        {
            dist = Mathf.Clamp(interAt.distance, minDist, maxDist );
            transform.position = rotation * (new Vector3(0.0f, 0.0f, -dist)) + player.transform.position;
        }

        
    }
}

