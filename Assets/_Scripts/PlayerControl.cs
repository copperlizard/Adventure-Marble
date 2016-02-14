using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{    
    public GameObject cam;

    [HideInInspector]
    public string powerUp;
    [HideInInspector]    
    public int PUcount;

    [System.Serializable]
    public class MarblePhys
    {
        public float power, hopPower, airPower, rPfactor, rPslide, rVfactor, radius, epsilon;        
    }

    [System.Serializable]
    public class PowerProps
    {
        public int fPushes;
        public float fPush, fPushCDTime, fPushRCTime, superJumpPower, gravMarbleDur, gravMarbleGrav, recallDur;
    }

    public MarblePhys phys = new MarblePhys();
    public PowerProps powps = new PowerProps();

    public AudioSource moveSounds, SFXSounds;

    [System.Serializable]
    public class audioClips
    {
        public AudioClip pickupSound, rolling1, collision1, jump1, superJumpcol1, superJump1, gravMarbleCol1, gravMarble1, recallCol1, recallTimer1, recallWarp1, fPush1, noPow1, noPush1;
    }
    public audioClips sounds = new audioClips();

    public List<Material> marbleMats = new List<Material>();

    private Rigidbody rb;
    private Renderer rend;
    private Vector3 push, groundAt;    
    private float x, z, volume, pitch;    
    private bool a, lBump, rBump, grounded, maybeAir, gravMarbleActive, recallActive, fPushLock;

    // Use this for initialization
    void Start()
    {
        //Hide cursor
        Cursor.visible = false; //Maybe move this to hud script...

        //Get player's rigid body
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 40;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; //Maybe make this optional for old computers?!?!        

        //Assign material if not default
        int matToUse = PlayerPrefs.GetInt("MarbleMat", -1);
        if ( matToUse != -1 || matToUse != 0)
        {
            matToUse = Mathf.Clamp(matToUse, 0, marbleMats.Count - 1); //Ensure in list range
            rend = gameObject.GetComponent<Renderer>();
            rend.material = marbleMats[matToUse];
        }        

        //Set up start vars
        PUcount = 0;
        grounded = true;
        gravMarbleActive = false;
        recallActive = false;
        fPushLock = false;
        powerUp = "none";        
    }

    void getInput()
    {
        //Get user input
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            a = true;
        }
        if (Input.GetButtonUp("Jump"))
        {
            a = false;
        }
        if (Input.GetButtonDown("Power"))
        {
            lBump = true;
        }
        if (Input.GetButtonUp("Power"))
        {
            lBump = false;
        }
        if (Input.GetButtonDown("ForcePush"))
        {
            rBump = true;
        }
        if (Input.GetButtonUp("ForcePush"))
        {
            rBump = false;
        }                       
    }

    void setPower(object[] parameters)
    {
        powerUp = parameters[0].ToString();

        StartCoroutine(respawnTimer((GameObject)parameters[2], (float)parameters[1]));

        if(powerUp == "SuperJump")
        {
            SFXSounds.PlayOneShot(sounds.superJumpcol1);            
        }
        else if(powerUp == "GravityMarble")
        {
            SFXSounds.PlayOneShot(sounds.gravMarbleCol1);
        }
        else if(powerUp == "Recall")
        {
            SFXSounds.PlayOneShot(sounds.recallCol1);
        }
    }

    IEnumerator resetDelay()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator respawnTimer(GameObject col, float t)
    {
        col.SetActive(false);
        yield return new WaitForSeconds(t);
        col.SetActive(true);
    }

    IEnumerator fPushCoolDowns()
    {
        yield return new WaitForSeconds(powps.fPushCDTime);
        fPushLock = false;        
        yield return new WaitForSeconds(powps.fPushRCTime);
        powps.fPushes++;        
    }

    IEnumerator gravityMarble()
    {
        rb.useGravity = false;
        gravMarbleActive = true;
        SFXSounds.clip = sounds.gravMarble1;
        SFXSounds.loop = true;
        SFXSounds.Play();
        float startTime = Time.time;
        while(Time.time < startTime + powps.gravMarbleDur)
        {
            rb.AddForce(groundAt * powps.gravMarbleGrav);            
            yield return null;
        }
        rb.useGravity = true;
        gravMarbleActive = false;
        SFXSounds.loop = false;
        SFXSounds.Stop();        
    }

    IEnumerator recall()
    {
        recallActive = true;
        float startTime = Time.time;

        Vector3 recallPos = transform.position;
        Quaternion recallRot = transform.rotation;

        Vector3 camStats = cam.GetComponent<CamControl>().camStats();       

        SFXSounds.clip = sounds.recallTimer1;
        SFXSounds.loop = true;
        SFXSounds.Play();

        while (startTime + powps.recallDur >= Time.time)
        {
            yield return null;
        }

        SFXSounds.loop = false;
        SFXSounds.Stop();

        SFXSounds.PlayOneShot(sounds.recallWarp1);

        transform.position = recallPos;
        transform.rotation = recallRot;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        cam.GetComponent<CamControl>().setCam(camStats);
        recallActive = false;        
    }

    // Update is called once per frame
    void Update()
    {
        getInput();
    }

    // FixedUpdate is called once per timestep
    void FixedUpdate()
    {
        //Desired push (normalized to cancel out "extra push")
        push = new Vector3(x, 0.0f, z);
        push = push.normalized;
        push = push * Mathf.SmoothStep( 0.0f, 1.0f, push.magnitude);

        //Account for grav marble
        if(gravMarbleActive)
        {
            push = Quaternion.Euler(0.0f, 0.0f, -Vector3.Angle(Vector3.down, groundAt)) * push;
            if(groundAt.y > 0.9f)
            {
                push.z = -push.z;
            }         
        }

        //Apply torque (Adjust push to account for camera rotation)
        rb.AddTorque(Quaternion.Euler(0.0f, cam.transform.rotation.eulerAngles.y + 90.0f, 0.0f) * (push * phys.power));

        if(maybeAir)
        {
            //Check if touching somewhere else...
            if(!Physics.CheckSphere(transform.position, phys.radius + phys.epsilon, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore)) //assuming a spherical marble
            {
                //Left the ground
                grounded = false;
            }
        }

        //If airborne, add aftertouch
        if(!grounded)
        {
            rb.AddForce(Quaternion.Euler(0.0f, cam.transform.rotation.eulerAngles.y, 0.0f) * (push * phys.airPower));
        }

        //Jump
        if (a && grounded == true)
        {
            a = false;            
            rb.AddForce(-groundAt * phys.hopPower);                       
            SFXSounds.PlayOneShot(sounds.jump1);            
        }

        //Force push
        if(rBump)
        {
            if(!fPushLock && (push.magnitude != 0.0f))
            {                
                if (powps.fPushes > 0)
                {
                    rBump = false;               
                    if(gravMarbleActive)
                    {
                        push = Quaternion.Euler(-Vector3.Angle(Vector3.down, groundAt), 0.0f, Vector3.Angle(Vector3.down, groundAt)) * push;                   
                    }

                    rb.AddForce(Quaternion.Euler(0.0f, cam.transform.rotation.eulerAngles.y, 0.0f) * (push * powps.fPush));
                    SFXSounds.PlayOneShot(sounds.fPush1);
                    powps.fPushes--;
                    fPushLock = true;
                    StartCoroutine(fPushCoolDowns());
                }
                else
                {
                    SFXSounds.PlayOneShot(sounds.noPush1);
                }
            }
            else
            {
                SFXSounds.PlayOneShot(sounds.noPush1);
            }                     
        }

        //Deployables
        if (lBump)
        {
            lBump = false;
            if (powerUp == "none")
            {
                SFXSounds.PlayOneShot(sounds.noPow1);
            }
            else if (powerUp == "SuperJump")
            {
                rb.AddForce(-groundAt * powps.superJumpPower);
                SFXSounds.PlayOneShot(sounds.superJump1);
                powerUp = "none";
            }
            else if (powerUp == "GravityMarble")
            {
                StartCoroutine(gravityMarble());
                powerUp = "none";
            }
            else if (powerUp == "Recall")
            {
                if(!recallActive)
                {
                    StartCoroutine(recall());
                    powerUp = "none";
                }
                else
                {
                    SFXSounds.PlayOneShot(sounds.noPush1);
                }                
            }
        }

        //Rolling noise
        if (grounded)
        {
            if(!moveSounds.isPlaying)
            {
                if(moveSounds.clip != sounds.rolling1)
                {
                    moveSounds.clip = sounds.rolling1;
                }
                moveSounds.Play();
            }

            pitch = Mathf.Lerp(pitch, Random.Range(0.2f, 1.0f), phys.rPslide);
            //volume = rb.velocity.magnitude * phys.rVfactor;
            //Debug.Log(rb.GetRelativePointVelocity(transform.position).magnitude.ToString());
            //volume = (rb.GetRelativePointVelocity(transform.position).magnitude / 50.0f) * phys.rVfactor;
            volume = (rb.angularVelocity.magnitude / 50.0f) * phys.rVfactor;
            moveSounds.pitch = pitch;
            moveSounds.volume = volume;
        }        
        else
        {
            if(moveSounds.isPlaying && moveSounds.clip == sounds.rolling1 )
            {
                moveSounds.Pause();
            }                       
        }
                
    }

    void OnCollsionEnter(Collision other)
    {
       if(other.gameObject.layer == 0)
        {
            Debug.Log("GROUND COLLISION ENTER!!!!!!!!!!!!!!THIS BASICALLY NEVER HAPPENS!!!!!!!!");         
            grounded = true;
            maybeAir = false;
            moveSounds.volume = Mathf.Clamp(moveSounds.volume, 0.5f, 0.7f);
            moveSounds.pitch = Mathf.Clamp(moveSounds.pitch, 0.5f, 1.0f);
            moveSounds.PlayOneShot(sounds.collision1);            
        }
    }

    //Called once per frame for each touching body
    void OnCollisionStay(Collision other)
    {
        if(!grounded)
        {
            //Debug.Log("GROUND COLLISION STAY!");
            moveSounds.volume = Mathf.Clamp(moveSounds.volume, 0.7f, 1.0f);
            moveSounds.pitch = Mathf.Clamp(moveSounds.pitch, 0.7f, 1.0f);
            moveSounds.PlayOneShot(sounds.collision1);
        }        

        //0 == ground layer
        if(other.gameObject.layer == 0)
        {
            //Check ground not too steep
            foreach(ContactPoint contact in other.contacts)
            {
                Vector3 point = contact.point;
                Vector3 line = point - transform.position;
                line = line.normalized;

                //Ground at
                float g = Vector3.Dot(Vector3.up, line);
                if(g < 0.0f && g >= -0.5f) 
                {
                    //Debug.Log("On Wall!");
                    grounded = true;
                    maybeAir = false;
                    groundAt = line;
                    break;
                }
                else if(g < -0.5f)
                {
                    //Debug.Log("On Ground!");
                    grounded = true;
                    maybeAir = false;
                    groundAt = line;
                    break;
                }
                else
                {
                    //Debug.Log("On Ceiling!");
                    if(gravMarbleActive)
                    {
                        grounded = true;
                        maybeAir = false;
                        groundAt = line;
                        break;
                    }
                }
            }          
        }
    }

    //TODO: WHY DOESN'T THIS WORK????
    void OnCollisionExit(Collision other)
    {        
        //0 == ground layer
        if(other.gameObject.layer == 0)
        {
            maybeAir = true;         
        }
    }    

    //Called when the collider other enters a trigger
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            SFXSounds.PlayOneShot(sounds.pickupSound);
            PUcount++;            
        }
        else if(other.gameObject.CompareTag("PowerUp"))
        {
            other.gameObject.BroadcastMessage("collected");
        }
    }
}