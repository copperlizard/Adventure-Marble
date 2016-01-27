using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    [System.Serializable]
    public class moveProperties
    {
        public enum translationBehavior { SMOOTH, LINEAR, NONE }
        public enum rotationBehavior { ROTATE, OSCILATE, NONE };
        public translationBehavior tranBehavior;
        public rotationBehavior rotBehavior;
        public Vector3 tranAxis, rotAxis;
        public float tTime, tSpeed, tDist, rSpeed, rMin, rMax;        
    }
    public moveProperties moveProps = new moveProperties();

    private Rigidbody platformBody;
    private Vector3 startPos, tarPos, refVel, lastMove;
    private float rotAng;
    private int rotDir;

    private static float epsilon = 0.175f;

	// Use this for initialization
	void Start ()
    {
        moveProps.tranAxis = moveProps.tranAxis.normalized;
        moveProps.rotAxis = moveProps.rotAxis.normalized;
        startPos = transform.position;
        tarPos = transform.position + (moveProps.tranAxis * moveProps.tDist);        
        rotAng = 0.0f;
        rotDir = 1;
        platformBody = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //translate();
        //rotate();
    }

    void FixedUpdate()
    {
        translate();
        rotate();
        //Debug.Log("platform velocity == " + platformBody.velocity.ToString());
    }

    void LateUpdate()
    {
        //translate();
        //rotate();
    }

    void translate()
    {        
        float d = (transform.position - tarPos).magnitude;
        Debug.Log(gameObject.name + " d == " + d.ToString());

        if(d <= epsilon)
        {
            //Debug.Log(gameObject.name + " reversing direction!");
            moveProps.tranAxis = -moveProps.tranAxis;
            Vector3 temp = tarPos;
            tarPos = startPos;
            startPos = temp;
        }

        switch (moveProps.tranBehavior)
        {
            case moveProperties.translationBehavior.SMOOTH:
                //platformBody.velocity = moveProps.tranAxis * moveProps.tSpeed * smooth(0.0f, moveProps.tDist, d);                
                break;
            case moveProperties.translationBehavior.LINEAR:
                //platformBody.velocity = moveProps.tranAxis * moveProps.tSpeed;
                platformBody.AddForce(moveProps.tranAxis * (moveProps.tSpeed - platformBody.velocity.magnitude), ForceMode.VelocityChange);
                break;
            case moveProperties.translationBehavior.NONE:
                break;
            default:
                break;                
        }
               
    }

    void rotate() //change this to use torque...
    {
        /*
        switch (moveProps.rotBehavior)
        {
            case moveProperties.rotationBehavior.ROTATE:
                rotAng += moveProps.rSpeed * Time.deltaTime * rotDir;
                transform.Rotate(moveProps.rotAxis * moveProps.rSpeed * Time.deltaTime);
                break;
            case moveProperties.rotationBehavior.OSCILATE:
                rotAng += moveProps.rSpeed * Time.deltaTime * rotDir;
                transform.Rotate(moveProps.rotAxis * moveProps.rSpeed * Time.deltaTime * rotDir);
                if(rotAng < moveProps.rMin || rotAng > moveProps.rMax)
                {
                    rotDir = -rotDir;
                }                
                break;
            case moveProperties.rotationBehavior.NONE:
                break;
            default:
                break;

        }

        clampAngle(rotAng, -360.0f, 360.0f);
        */
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

    float smooth(float a, float b, float t)
    {
        b -= a;
        t -= a;
        a = 0.0f;
        t /= b;
        b /= b;
        return 1.0f - Mathf.Abs(t - 0.5f);
    }

    void OnCollisionStay(Collision other)
    {
        //Platforms "carry" player
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {            
           
        }
    }

    void OnCollisionExit(Collision other)
    {
        //Platforms "carry" player
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            
        }
    }
}
