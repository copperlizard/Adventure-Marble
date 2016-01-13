using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public float power, hopPower;
    public Text countText, winText;
    public GameObject cam;

    private Rigidbody rb;
    private int count;
    private bool grounded;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        count = 0;

        winText.text = "";
        setText();

        grounded = false;
    }

    // FixedUpdate is called once per timestep
    void FixedUpdate()
    {
        //Get user input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float a = Input.GetAxisRaw("Jump");

        //Desired push (normalized to cancel out "extra push")
        Vector3 push = new Vector3(x, 0.0f, z);
        push = push.normalized;
        push = push * power * Time.deltaTime;

        //Adjust push to account for camera rotation
        push = Quaternion.Euler( 0.0f, cam.transform.rotation.eulerAngles.y, 0.0f ) * push;

        //Apply push
        rb.AddForce(push);

        //Jump
        if( a > 0 && grounded == true )
        {
            rb.AddForce(Vector3.up * hopPower);
            grounded = false;
        }

        //Check for ball death/lost
        if (rb.position.y <= -5.0f)
        {
            winText.text = ":(";
            StartCoroutine(resetDelay());
        }
    }

    //Called once per frame for each touching body
    void OnCollisionStay(Collision other)
    {
        //0 == ground layer
        if(other.gameObject.layer == 0)
        {
            //Check ground not too steep (or a wall)
            foreach(ContactPoint contact in other.contacts )
            {
                Vector3 point = contact.point;
                Vector3 line = point - transform.position;
                line = line.normalized;

                //Grounded
                if( Vector3.Dot( Vector3.up, line) < 0.0 ) //decrease later...
                {
                    grounded = true;
                    break;
                }
            }
        }
    }

    //Called when the collider other enters a trigger
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            setText();
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
        SceneManager.LoadScene(0);
    }

    IEnumerator resetDelay()
    {

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
