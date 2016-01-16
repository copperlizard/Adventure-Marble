using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public float power, hopPower, pfactor, vfactor;
    public Text countText, winText;
    public GameObject cam;
    public GameObject PauseScreen;
    public AudioSource pickupSound;
    public AudioSource rolling1;
    public AudioSource collision1;

    private Rigidbody rb;
    private Vector3 push, groundAt;
    private float x, z, volume, pitch;
    private int count;
    private bool a, grounded, pause;

    // Use this for initialization
    void Start()
    {
        //Hide cursor
        Cursor.visible = false;

        //Get player's rigid body
        rb = GetComponent<Rigidbody>();

        count = 0;

        winText.text = "";
        setText();

        grounded = false;
        pause = false;
        PauseScreen.SetActive(false);
    }

    void getInput()
    {
        //Get user input
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        a = Input.GetButtonDown("Jump");
        if(Input.GetButtonDown("Pause"))
        {
            if( !pause )
            {
                pause = true;
            }
            else
            {
                pause = false;
            }
        }
    }

    void pauseGame()
    {
        Time.timeScale = 0.0f;
        PauseScreen.SetActive(true);
        Cursor.visible = true;
        rolling1.Pause();
        collision1.Pause();       
    }

    void resumeGame()
    {
        Time.timeScale = 1.0f;
        PauseScreen.SetActive(false);
        Cursor.visible = false;
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

    IEnumerator JumpSoundDelay()
    {
        //Debug.Log("Called JumpSoundDelay()!");
        while( !grounded )
        {
            //Debug.Log("Waiting to Land!");
            yield return null;
        }

        pitch = Mathf.Lerp(pitch, Random.value * pfactor, 0.01f);
        volume = rb.velocity.magnitude * vfactor;
        collision1.pitch = pitch;
        collision1.volume = volume;

        collision1.PlayOneShot(collision1.clip);
    }

    // Update is called once per frame
    void Update()
    {
        getInput();

        //Pause/UnPause game if necessary
        if(pause && Time.timeScale != 0.0f)
        {
            pauseGame();
        }
        else if(!pause && Time.timeScale == 0.0f)
        {
            resumeGame();
        }
    }

    // FixedUpdate is called once per timestep
    void FixedUpdate()
    {
        //Desired push (normalized to cancel out "extra push")
        push = new Vector3(x, 0.0f, z);
        push = push.normalized;
        push = push * power * Time.deltaTime;

        //Adjust push to account for camera rotation
        push = Quaternion.Euler( 0.0f, cam.transform.rotation.eulerAngles.y, 0.0f ) * push;

        //Apply push
        rb.AddForce(push);

        //Jump
        if( a && grounded == true )
        {
            rb.AddForce(-groundAt * hopPower * Time.deltaTime);
            //rb.AddForce(Vector3.up * hopPower * Time.deltaTime);
            //grounded = false;
            //Debug.Log("Jumping!" + groundAt.ToString()); //REMOVE LATER!!!
        }

        //Rolling noise
        if( grounded )
        {
            if( !rolling1.isPlaying )
            {
                rolling1.Play();
            }

            pitch = Mathf.Lerp( pitch, Random.value * pfactor, 0.01f);
            volume = rb.velocity.magnitude * vfactor;
            rolling1.pitch = pitch;
            rolling1.volume = volume;
        }
        else
        {
            rolling1.Pause();
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
       //I think this only works on colliders with rigid bodies (not the level...)
    }

    //Called once per frame for each touching body
    void OnCollisionStay(Collision other)
    {
        //0 == ground layer
        if(other.gameObject.layer == 0)
        {
            //Check ground not too steep
            foreach(ContactPoint contact in other.contacts )
            {
                Vector3 point = contact.point;
                Vector3 line = point - transform.position;
                line = line.normalized;

                //Ground at
                if( Vector3.Dot( Vector3.up, line) < 0.0 ) //decrease 0.0(90deg) later...
                {
                    grounded = true;
                    groundAt = line;
                    //Debug.Log("Grounded!"); //REMOVE LATER!!!
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
            //Debug.Log("Falling!"); //REMOVE LATER!!!

            StartCoroutine(JumpSoundDelay());
        }
    }

    //Called when the collider other enters a trigger
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            pickupSound.PlayOneShot(pickupSound.clip);
            count++;
            setText();
        }
    }
}
