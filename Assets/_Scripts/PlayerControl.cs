using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public float power;
    public Text countText, winText;
    public GameObject cam;

    private Rigidbody rb;
    private int count;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        count = 0;

        winText.text = "";
        setText();
    }

    // FixedUpdate is called once per timestep
    void FixedUpdate()
    {
        //Get user input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Desired push (normalized to cancel out "extra push")
        Vector3 push = new Vector3(x, 0.0f, z);
        push = push.normalized;
        push = push * power * Time.deltaTime;

        push = Quaternion.Euler( 0.0f, cam.transform.rotation.eulerAngles.y, 0.0f ) * push;

        rb.AddForce(push);

        //Check for ball death/lost
        if (rb.position.y <= -5.0f)
        {
            winText.text = ":(";
            StartCoroutine(resetDelay());
        }
    }

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
