using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    [System.Serializable]
    public class moveProperties
    {
        public enum translationBehavior { LINEAR, SMOOTH, NONE }
        public enum rotationBehavior { ROTATE, OSCILATE, NONE };
        public translationBehavior tranBehavior;
        public rotationBehavior rotBehavior;
        public Vector3 tranAxis, rotAxis;
        public float tSpeed, tDist, tPad, rSpeed, rMin, rMax;        
    }
    public moveProperties moveProps = new moveProperties();

    private Rigidbody platformBody;
    private Vector3 startPos, tarPos, midPos, refVel;
    private float tarVel, rotAng, rotCounter;
    private int rotDir;    

	// Use this for initialization
	void Start ()
    {
        //Normalize axes
        moveProps.tranAxis = moveProps.tranAxis.normalized;
        moveProps.rotAxis = moveProps.rotAxis.normalized;

        //Find/store pos's
        startPos = transform.position;
        tarPos = transform.position + (moveProps.tranAxis * moveProps.tDist);
        Vector3 line = tarPos - startPos;
        line /= 2.0f;
        midPos = startPos + line;

        //Set up tran vars
        tarVel = moveProps.tSpeed;
        
        //Set up rot vars        
        rotAng = 0.0f;
        rotCounter = 0.0f;
        rotDir = 1;

        //Get platform rigid body
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
        //d from mid pos        
        float d = (transform.position - midPos).magnitude;
        float dFar = ((moveProps.tDist / 2.0f) - moveProps.tPad);
        //Debug.Log(gameObject.name + " d == " + d.ToString());

        //If too far from d
        if(d >= dFar)
        {
            //Find line pointing to midPos
            Vector3 line = midPos - transform.position;
            line.Normalize();

            //If line and tranAxis pointing different directions
            if(Vector3.Dot(line, moveProps.tranAxis) < 1)
            {
                //Reverse tranAxis
                moveProps.tranAxis = -moveProps.tranAxis;
            }
        }

        switch (moveProps.tranBehavior)
        {            
            case moveProperties.translationBehavior.LINEAR:
                platformBody.MovePosition(transform.position + (moveProps.tranAxis * moveProps.tSpeed));                
                break;
            case moveProperties.translationBehavior.SMOOTH:                                     //THIS DOES NOT WORK WELL!!!
                //tarVel = moveProps.tSpeed * Mathf.Clamp(1.0f - d / dFar, 0.3f, 1.0f);
                tarVel = moveProps.tSpeed * Mathf.Clamp(curve(d, dFar), 0.3f, 1.0f);
                platformBody.MovePosition(transform.position + (moveProps.tranAxis * tarVel));
                break;
            case moveProperties.translationBehavior.NONE:
                break;
            default:
                break;                
        }
               
    }

    float curve(float d, float dFar)
    {
        float t = d / dFar;
        return 1.0f - (-((t - 1.0f) * (t - 1.0f)) + 1.0f);
    }

    void rotate()
    {        
        switch (moveProps.rotBehavior)
        {
            case moveProperties.rotationBehavior.ROTATE:
                rotAng = moveProps.rSpeed * Time.deltaTime * rotDir;
                rotCounter += rotAng;
                platformBody.MoveRotation(platformBody.rotation * Quaternion.Euler(moveProps.rotAxis * rotAng));
                break;
            case moveProperties.rotationBehavior.OSCILATE:
                rotAng = moveProps.rSpeed * Time.deltaTime * rotDir;
                rotCounter += rotAng;
                platformBody.MoveRotation(platformBody.rotation * Quaternion.Euler(moveProps.rotAxis * rotAng));
                if(rotCounter < moveProps.rMin || rotCounter > moveProps.rMax)
                {
                    Debug.Log("Flipping Rotation!");
                    rotDir = -rotDir;
                }                
                break;
            case moveProperties.rotationBehavior.NONE:
                break;
            default:
                break;

        }

        clampAngle(rotCounter, -360.0f, 360.0f);        
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
