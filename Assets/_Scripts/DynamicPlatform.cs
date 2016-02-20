using UnityEngine;
using System.Collections;

public enum PlatformSwitchBehavior { TOGGLE, ONEWAY }
public enum PlatformRotationBehavior { CONTINUOUS, OSCILATE, ROTATETO }

public class DynamicPlatform : MonoBehaviour
{
    public PlatformSwitchBehavior switchBehavior;
    public PlatformRotationBehavior rotationBehavior;
    public Vector3 startPos, tarPos, rotAxis, pivPos;
    public float tSpeed, rSpeed, rMin, rMax, rTar, epsilon;
    public bool frozen;

    private Rigidbody rb;
    private float rotAng;
    private int tDir;

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        tDir = 1;        	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void FixedUpdate()
    {
        if(!frozen)
        {
            //Debug.Log("not frozen!");
            //Debug.Log("tSpeed = " + tSpeed.ToString());
            //Debug.Log("rSpeed = " + rSpeed.ToString());
            //Debug.Log("rTar = " + rTar.ToString());
            translate();
            rotate();
        }        
    }

    void translate()
    {        
        //Check dist from tar            
        float dist = (tarPos - transform.position).magnitude;
        if (dist <= epsilon)
        {
            Debug.Log("flipping start and endpoints!");
            Vector3 temp = startPos;
            startPos = tarPos;
            tarPos = temp;
            dist = (tarPos - transform.position).magnitude;
            //tDir = -tDir;

            if(switchBehavior == PlatformSwitchBehavior.ONEWAY)
            {
                //Wont move until switch unfreezes platform
                frozen = true;
            }
        }

        Vector3 rail = tarPos - startPos;
        Vector3 trav = rail.normalized * Mathf.Min(tSpeed, dist) * Time.deltaTime * tDir;
        rb.MovePosition(transform.position + trav);
    }

    void rotate()
    {        
        switch (rotationBehavior)
        {
            case PlatformRotationBehavior.CONTINUOUS:
                rotAng += rSpeed * Time.deltaTime;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(rotAxis * rSpeed * Time.deltaTime));
                break;
            case PlatformRotationBehavior.OSCILATE:
                rotAng += rSpeed * Time.deltaTime;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(rotAxis * rSpeed * Time.deltaTime));
                if (rotAng <= rMin || rotAng >= rMax)
                {
                    rSpeed = -rSpeed;
                }
                break;
            case PlatformRotationBehavior.ROTATETO:
                rotAng += Mathf.Min(rSpeed, rTar - rotAng) * Time.deltaTime;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(rotAxis * Mathf.Min(rSpeed, rTar - rotAng) * Time.deltaTime));                
                break;
            default:
                break;                    
        }
        clampAngle(rotAng, rMin, rMax);
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
}
