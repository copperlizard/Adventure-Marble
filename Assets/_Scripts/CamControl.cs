using UnityEngine;
using System.Collections;

public class CamControl : MonoBehaviour {

    public GameObject player;
    public float fDist, minDist, maxDist, xspeed, yspeed, ymin, ymax, hstart, vstart, damp;

    private Quaternion rotation;
    private Vector3 viewLine;
    private RaycastHit interAt;
    private float h, v, dist;  

    // Use this for initialization
    void Start ()
    {
        viewLine = transform.position - player.transform.position;
        viewLine = viewLine.normalized;

        transform.position = player.transform.position + viewLine * fDist;

        h = hstart;
        v = vstart;

        rotation = Quaternion.Euler(v, h, 0.0f);
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

    void getInput()
    {
        h -= Input.GetAxis("rHorizontal") * xspeed * 0.02f;
        v += Input.GetAxis("rVertical") * yspeed * 0.02f;

        v = clampAngle(v, ymin, ymax);
    }

	// Update is called once per frame
	void Update()
    {
        getInput();
	}
       
    void LateUpdate()
    {
        //Build rotation using input
        rotation = Quaternion.Slerp(rotation, Quaternion.Euler(v, h, 0.0f), damp * Time.deltaTime);

        //Set desired dist
        dist = fDist;

        //Place and rotate camera transform based on player transform and user input
        transform.position = rotation * (new Vector3(0.0f, 0.0f, -dist)) + player.transform.position;
        transform.rotation = rotation;
        
        //Check for intersection along view ray and move camera if necessary
        if (Physics.Raycast(player.transform.position, transform.position - player.transform.position, out interAt))
        {
            //Ignore intersections with game pieces
            if( interAt.collider.gameObject.layer != 9 )
            {
                dist = Mathf.Clamp(interAt.distance - 0.1f, minDist, maxDist);
                transform.position = rotation * (new Vector3(0.0f, 0.0f, -dist)) + player.transform.position;
            }            
        }        
    }
}

//transform.position = Vector3.Lerp(transform.position, rotation * (new Vector3(0.0f, 0.0f, -dist)) + player.transform.position, damp * Time.fixedDeltaTime);