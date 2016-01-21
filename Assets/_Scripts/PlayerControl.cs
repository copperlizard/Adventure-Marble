using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerControl : MonoBehaviour
{    
    public GameObject cam;

    [HideInInspector]
    public string powerUp;
    public int PUcount;

    [System.Serializable]
    public class MarblePhys
    {
        public float power, hopPower, airPower, rPfactor, rVfactor, superJumpPower, gravMarbleDur, gravMarbleGrav, fPush, fPushCDTime, fPushRCTime;
        public int fPushes;
    }
    public MarblePhys phys = new MarblePhys();

    [System.Serializable]
    public class audioSources
    {
        public AudioSource pickupSound;
        public AudioSource rolling1;
        public AudioSource collision1;
        public AudioSource jump1;
        public AudioSource superJumpcol1;
        public AudioSource superJump1;
        public AudioSource gravMarbleCol1;
        public AudioSource gravMarble1;
        public AudioSource fPush1;
        public AudioSource noPow1;
        public AudioSource noPush1;
    }
    public audioSources sounds = new audioSources();

    private Rigidbody rb;
    private Vector3 push, groundAt;    
    private float x, z, volume, pitch;    
    private bool a, lBump, rBump, grounded, gravMarbleActive, fPushLock;

    // Use this for initialization
    void Start()
    {
        //Hide cursor
        Cursor.visible = false; //Maybe move this to hud script...

        //Get player's rigid body
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 40;

        PUcount = 0;        

        grounded = false;
        gravMarbleActive = false;
        fPushLock = false;

        powerUp = "none";
    }

    void getInput()
    {        
        //Get user input
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        a = Input.GetButtonDown("Jump");
        lBump = Input.GetButtonDown("Power");
        rBump = Input.GetButtonDown("ForcePush");               
    }

    void setPower(object[] parameters)
    {
        powerUp = parameters[0].ToString();

        StartCoroutine(respawnTimer((GameObject)parameters[2], (float)parameters[1]));

        if(powerUp == "SuperJump")
        {
            sounds.superJumpcol1.PlayOneShot(sounds.superJumpcol1.clip);
        }
        else if(powerUp == "GravityMarble")
        {
            sounds.gravMarbleCol1.PlayOneShot(sounds.gravMarbleCol1.clip);
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
        yield return new WaitForSeconds(phys.fPushCDTime);
        fPushLock = false;        
        yield return new WaitForSeconds(phys.fPushRCTime);
        phys.fPushes++;        
    }

    IEnumerator gravityMarble()
    {
        rb.useGravity = false;
        gravMarbleActive = true;
        sounds.gravMarble1.Play();
        float startTime = Time.time;
        while( Time.time < startTime + phys.gravMarbleDur)
        {
            rb.AddForce(groundAt * phys.gravMarbleGrav);            
            yield return null;
        }
        rb.useGravity = true;
        gravMarbleActive = false;
        sounds.gravMarble1.Stop();
    }

    IEnumerator JumpSoundDelay()
    {        
        while( !grounded )
        {            
            yield return null;
        }

        pitch = Mathf.Lerp(pitch, Random.value * phys.rPfactor, 0.01f);
        volume = rb.velocity.magnitude * phys.rVfactor;
        sounds.collision1.pitch = Mathf.Clamp(pitch, 0.2f, 1.5f);
        sounds.collision1.volume = Mathf.Clamp(volume, 0.2f, 1.5f);
        sounds.collision1.PlayOneShot(sounds.collision1.clip);
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
        }

        //Apply torque (Adjust push to account for camera rotation)
        rb.AddTorque(Quaternion.Euler(0.0f, cam.transform.rotation.eulerAngles.y + 90.0f, 0.0f) * (push * phys.power));

        //If airborne, add aftertouch
        if(!grounded)
        {
            rb.AddForce(Quaternion.Euler(0.0f, cam.transform.rotation.eulerAngles.y, 0.0f) * (push * phys.airPower));
        }

        //Jump
        if (a && grounded == true)
        {
            rb.AddForce(-groundAt * phys.hopPower);
            sounds.jump1.PlayOneShot(sounds.jump1.clip);
        }

        //Deployables
        if(lBump)
        {
            if(powerUp == "none")
            {
                if(!sounds.noPow1.isPlaying)
                {
                    sounds.noPow1.PlayOneShot(sounds.noPow1.clip);
                }                
            }
            else if(powerUp == "SuperJump")
            {
                rb.AddForce(-groundAt * phys.superJumpPower);
                sounds.superJump1.PlayOneShot(sounds.superJump1.clip);
                powerUp = "none";
            }
            else if(powerUp == "GravityMarble")
            {
                StartCoroutine(gravityMarble());
                powerUp = "none";
            }            
        }

        //Force push
        if(rBump)
        {
            if(!fPushLock && (push.magnitude != 0.0f))
            {                
                if (phys.fPushes > 0)
                {               
                    if(gravMarbleActive)
                    {
                        push = Quaternion.Euler(-Vector3.Angle(Vector3.down, groundAt), 0.0f, Vector3.Angle(Vector3.down, groundAt)) * push;                   
                    }

                    rb.AddForce(Quaternion.Euler(0.0f, cam.transform.rotation.eulerAngles.y, 0.0f) * (push * phys.fPush));
                    sounds.fPush1.PlayOneShot(sounds.fPush1.clip);
                    phys.fPushes--;
                    fPushLock = true;
                    StartCoroutine(fPushCoolDowns());
                }
                else
                {
                    sounds.noPow1.PlayOneShot(sounds.noPush1.clip);
                }
            }
            else
            {
                sounds.noPow1.PlayOneShot(sounds.noPush1.clip);
            }                     
        }

        //Rolling noise
        if(grounded)
        {
            if(!sounds.rolling1.isPlaying)
            {
                sounds.rolling1.Play();
            }

            pitch = Mathf.Lerp(pitch, ( Random.value / 2.0f ) + 0.25f, phys.rPfactor);
            volume = rb.velocity.magnitude * phys.rVfactor;
            sounds.rolling1.pitch = pitch;
            sounds.rolling1.volume = volume;
        }
        else
        {
            sounds.rolling1.Pause();           
        }        
    }

    void OnCollsionEnter(Collision other)
    {
       if(other.gameObject.layer == 0)
        {
            grounded = true;
            
            sounds.collision1.PlayOneShot(sounds.collision1.clip);
        }
    }

    //Called once per frame for each touching body
    void OnCollisionStay(Collision other)
    {
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
                    groundAt = line;
                    break;
                }
                else if(g < -0.5f)
                {
                    //Debug.Log("On Ground!");
                    grounded = true;
                    groundAt = line;
                    break;
                }
            }          
        }
    }

    void OnCollisionExit(Collision other)
    {
        //0 == ground layer
        if(other.gameObject.layer == 0)
        {
            //Left the ground
            grounded = false; 
            StartCoroutine(JumpSoundDelay());
        }
    }

    //Called when the collider other enters a trigger
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            sounds.pickupSound.PlayOneShot(sounds.pickupSound.clip);
            PUcount++;            
        }
        else if(other.gameObject.CompareTag("PowerUp"))
        {
            other.gameObject.BroadcastMessage("collected");
        }
    }
}