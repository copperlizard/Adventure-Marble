using UnityEngine;
using System.Collections;

public class CamControl : MonoBehaviour {

    public GameObject player;    
    public float fDist, minDist, maxDist, xspeed, yspeed, ymin, ymax, hstart, vstart, damp;

    private Quaternion rotation;
    private Vector3 viewLine;
    private RaycastHit interAt;
    private float h, v, dist, invLookX, invLookY;
    private bool paused;

    // Use this for initialization
    void Start ()
    {
        AudioListener.pause = false;

        viewLine = transform.position - player.transform.position;
        viewLine = viewLine.normalized;

        transform.position = player.transform.position + viewLine * fDist;

        h = hstart;
        v = vstart;

        rotation = Quaternion.Euler(v, h, 0.0f);

        paused = false;

        //Check for look control inversion
        if (PlayerPrefs.GetInt("invertLookX", 1) == -1)
        {
            invLookX = -1.0f;
        }
        else
        {
            invLookX = 1.0f;
        }

        if (PlayerPrefs.GetInt("invertLookY", 1) == -1)
        {
            invLookY = -1.0f;
        }
        else
        {
            invLookY = 1.0f;
        }
    }

    public void pause(bool state)
    {
        paused = state;
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
        if(!paused)
        {
            h -= Input.GetAxis("rHorizontal") * xspeed * 0.02f * invLookX;
            v += Input.GetAxis("rVertical") * yspeed * 0.02f * invLookY;

            v = clampAngle(v, ymin, ymax);
        }        
    }

    public Vector3 camStats()
    {
        return new Vector3(h, v, dist);
    }

    public void setCam(Vector3 stats)
    {
        h = stats.x;
        v = stats.y;
        dist = stats.z;
    }

	// Update is called once per frame
	void Update()
    {
        getInput();
	}
       
    void LateUpdate()
    {
        if(player.transform.position.y >= 0.0f)
        {
            //Build rotation using input
            rotation = Quaternion.Slerp(rotation, Quaternion.Euler(v, h, 0.0f), damp * Time.deltaTime);
            //rotation = Quaternion.Euler(v, h, 0.0f);
            transform.rotation = rotation;

            //Set desired dist
            dist = fDist;

            //Place and rotate camera transform based on player transform and user input
            transform.position = rotation * (new Vector3(0.0f, 0.0f, -dist)) + player.transform.position;
            //transform.position = Vector3.SmoothDamp(transform.position, rotation * (new Vector3(0.0f, 0.0f, -dist)) + player.transform.position, ref refVel, 0.01f);

            //Check for intersection along view ray and move camera if necessary
            if (Physics.Raycast(player.transform.position, transform.position - player.transform.position, out interAt))
            {
                //Ignore intersections with game pieces
                if (interAt.collider.gameObject.layer != 9)
                {
                    dist = Mathf.Clamp(interAt.distance - 0.1f, minDist, maxDist);
                    transform.position = rotation * (new Vector3(0.0f, 0.0f, -dist)) + player.transform.position;
                    //transform.position = Vector3.SmoothDamp(transform.position, rotation * (new Vector3(0.0f, 0.0f, -dist)) + player.transform.position, ref refVel, 0.01f);
                }
            }
        }
        else
        {
            transform.LookAt(player.transform.position);
        }                
    }
}