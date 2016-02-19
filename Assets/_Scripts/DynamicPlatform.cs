using UnityEngine;
using System.Collections;

public enum PlatformSwitchBehavior { TOGGLE, ONEWAY }
public enum PlatformRotationBehavior { ONEWAY, OSCILATE }

public class DynamicPlatform : MonoBehaviour
{
    public PlatformSwitchBehavior switchBehavior;
    public PlatformRotationBehavior rotationBehavior;
    public Vector3 startPos, tarPos, rotAxis, pivPos;
    public float tSpeed, rSpeed, rMin, rMax, epsilon;
    public bool frozen;

    private Rigidbody rb;
    private float rotAng;

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody>();	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void FixedUpdate()
    {
        if(!frozen)
        {
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
            Vector3 temp = startPos;
            startPos = tarPos;
            tarPos = temp;
            dist = (tarPos - transform.position).magnitude;

            if(switchBehavior == PlatformSwitchBehavior.ONEWAY)
            {
                //Wont move until switch unfreezes platform
                frozen = true;
            }
        }

        Vector3 rail = tarPos - startPos;
        Vector3 trav = rail.normalized * Mathf.Min(tSpeed, dist) * Time.deltaTime;
        rb.MovePosition(transform.position + trav);
    }

    void rotate()
    {
        rotAng += rSpeed * Time.deltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotAxis * rotAng));
        switch (rotationBehavior)
        {
            case PlatformRotationBehavior.ONEWAY:                
                break;
            case PlatformRotationBehavior.OSCILATE:
                if(rotAng <= rMin || rotAng >= rMax)
                {
                    rSpeed = -rSpeed;
                }
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
