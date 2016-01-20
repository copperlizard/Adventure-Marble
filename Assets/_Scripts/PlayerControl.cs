using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public float power, hopPower, airPower, rPfactor, rVfactor, superJumpPower, gravMarbleDur, gravMarbleGrav;
    public Text countText, winText;
    public GameObject cam;

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
    }
    public audioSources sounds = new audioSources();

    private Rigidbody rb;
    private Vector3 push, groundAt;
    private string powerUp;
    private float x, z, volume, pitch;
    private int count;
    private bool a, lb, grounded;

    // Use this for initialization
    void Start()
    {
        //Hide cursor
        Cursor.visible = false; //Maybe move this to hud script...

        //Get player's rigid body
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 40;

        count = 0;

        winText.text = "";
        setText();

        grounded = false;        

        powerUp = "none";
    }

    void getInput()
    {        
        //Get user input
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        a = Input.GetButtonDown("Jump");
        lb = Input.GetButtonDown("Power");               
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

    void setText()
    {
        countText.text = "Count: " + count.ToString();

        if (count >= 12)
        {
            winText.text = "You Win!";

            StartCoroutine(EndDelay());
        }
    }

    IEnumerator EndDelay()
    {
        yield return new WaitForSeconds(5);
        Cursor.visible = true;
        SceneManager.LoadScene(0);
    }

    IEnumerator resetDelay()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator respawnTimer(GameObject col, float t)
    {
        col.SetActive(false);
        yield return new WaitForSeconds(t);
        col.SetActive(true);
    }

    IEnumerator gravityMarble()
    {
        rb.useGravity = false;
        sounds.gravMarble1.Play();
        float startTime = Time.time;
        while( Time.time < startTime + gravMarbleDur)
        {
            rb.AddForce(groundAt * gravMarbleGrav);
            yield return null;
        }
        rb.useGravity = true;
        sounds.gravMarble1.Stop();
    }

    IEnumerator JumpSoundDelay()
    {
        //Debug.Log("Called JumpSoundDelay()!");
        while( !grounded )
        {
            //Debug.Log("Waiting to Land!");
            yield return null;
        }

        pitch = Mathf.Lerp(pitch, Random.value * rPfactor, 0.01f);
        volume = rb.velocity.magnitude * rVfactor;
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

        //Apply torque (Adjust push to account for camera rotation)
        rb.AddTorque(Quaternion.Euler(0.0f, cam.transform.rotation.eulerAngles.y + 90.0f, 0.0f) * (push * power) );

        if(!grounded)
        {
            rb.AddForce(Quaternion.Euler(0.0f, cam.transform.rotation.eulerAngles.y, 0.0f) * (push * airPower));
        }

        //Jump
        if (a && grounded == true)
        {
            rb.AddForce(-groundAt * hopPower);
            sounds.jump1.PlayOneShot(sounds.jump1.clip);
        }

        //Deployables
        if(lb)
        {
            if(powerUp == "none")
            {
                //Maybe play funny sound
            }
            else if(powerUp == "SuperJump")
            {
                rb.AddForce(-groundAt * superJumpPower);
                sounds.superJump1.PlayOneShot(sounds.superJump1.clip);
                powerUp = "none";
            }
            else if(powerUp == "GravityMarble")
            {
                StartCoroutine(gravityMarble());
                powerUp = "none";
            }            
        }

        //Rolling noise
        if(grounded)
        {
            if(!sounds.rolling1.isPlaying)
            {
                sounds.rolling1.Play();
            }

            pitch = Mathf.Lerp(pitch, Random.value / 2.0f, rPfactor);
            volume = rb.velocity.magnitude * rVfactor;
            sounds.rolling1.pitch = pitch;
            sounds.rolling1.volume = volume;
        }
        else
        {
            sounds.rolling1.Pause();           
        }

        //Check for ball death/lost
        if (rb.position.y <= -5.0f)
        {
            winText.text = ":(";
            StartCoroutine(resetDelay());
        }
    }

    void OnCollsionEnter(Collision other)
    {
       if(other.gameObject.layer == 0)
        {
            grounded = true;
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
                if(Vector3.Dot(Vector3.up, line) < 0.0) //0.0 -> 90deg
                {
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
            count++;
            setText();
        }
        else if(other.gameObject.CompareTag("PowerUp"))
        {
            other.gameObject.BroadcastMessage("collected");
        }
    }
}