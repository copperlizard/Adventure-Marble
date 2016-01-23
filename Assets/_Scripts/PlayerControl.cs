using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

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
        public float power, hopPower, airPower, rPfactor, rPslide, rVfactor, superJumpPower, gravMarbleDur, gravMarbleGrav, fPush, fPushCDTime, fPushRCTime;
        public int fPushes;
    }
    public MarblePhys phys = new MarblePhys();

    public AudioSource moveSounds;
    public AudioSource SFXSounds;

    [System.Serializable]
    public class audioClips
    {
        public AudioClip pickupSound;
        public AudioClip rolling1;
        public AudioClip collision1;
        public AudioClip jump1;
        public AudioClip superJumpcol1;
        public AudioClip superJump1;
        public AudioClip gravMarbleCol1;
        public AudioClip gravMarble1;
        public AudioClip fPush1;
        public AudioClip noPow1;
        public AudioClip noPush1;
    }
    public audioClips sounds = new audioClips();

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
            SFXSounds.PlayOneShot(sounds.superJumpcol1);            
        }
        else if(powerUp == "GravityMarble")
        {
            SFXSounds.PlayOneShot(sounds.gravMarbleCol1);
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
        moveSounds.clip = sounds.gravMarble1;
        moveSounds.Play();
        float startTime = Time.time;
        while(Time.time < startTime + phys.gravMarbleDur)
        {
            rb.AddForce(groundAt * phys.gravMarbleGrav);            
            yield return null;
        }
        rb.useGravity = true;
        gravMarbleActive = false;
        moveSounds.clip = sounds.rolling1;
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
            if(SFXSounds.clip != sounds.jump1 )
            {  
                SFXSounds.clip = sounds.jump1;
            }            
            SFXSounds.PlayOneShot(sounds.jump1);            
        }

        //Deployables
        if(lBump)
        {
            if(powerUp == "none")
            {
                if(!SFXSounds.isPlaying)
                {
                    SFXSounds.clip = sounds.noPow1; //PRIORITY SFX
                    SFXSounds.PlayOneShot(sounds.noPow1);
                }                
            }
            else if(powerUp == "SuperJump")
            {
                rb.AddForce(-groundAt * phys.superJumpPower);
                SFXSounds.PlayOneShot(sounds.superJump1);
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
                    SFXSounds.PlayOneShot(sounds.fPush1);
                    phys.fPushes--;
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

        //Rolling noise
        if(grounded)
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
            volume = rb.velocity.magnitude * phys.rVfactor;
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
            grounded = true;
            moveSounds.volume = Mathf.Clamp(moveSounds.volume, 0.5f, 0.7f);
            moveSounds.PlayOneShot(sounds.collision1);            
        }
    }

    //Called once per frame for each touching body
    void OnCollisionStay(Collision other)
    {
        if(!grounded)
        {
            moveSounds.volume = Mathf.Clamp(moveSounds.volume, 0.5f, 0.7f);
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
                else
                {
                    //Debug.Log("On Ceiling!");
                    if(gravMarbleActive)
                    {
                        grounded = true;
                        groundAt = line;
                        break;
                    }
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
            //StartCoroutine(JumpSoundDelay());
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