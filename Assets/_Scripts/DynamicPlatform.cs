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
    private Vector3 pivot;    
    private float rotAng;    

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody>();

        //pivot = location in world space; pivPos = location in local space
        pivot = transform.position + transform.rotation * pivPos;                           	
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
            //Debug.Log("flipping start and endpoints!");
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
        //rb.MovePosition(transform.position + trav); //Moved move to end of rotate...

        //Update pivot
        pivot += trav;
    }

    void rotate()
    {
        clampAngle(rotAng, rMin, rMax);

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
                float dif = rTar - rotAng;

                float rotChange = ((dif > 0.0f) ? Mathf.Min(rSpeed, dif) : Mathf.Max(-rSpeed, dif)) * Time.deltaTime;
                rotAng += rotChange;
                
                rb.MoveRotation(rb.rotation * Quaternion.Euler(rotAxis * rotChange));
                break;
            default:
                break;                    
        }
        
        Vector3 toPiv = rb.rotation * pivPos;
        rb.MovePosition(pivot - toPiv);                
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
