using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public float speed;
    public Text countText;
    public Text winText;

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

        float moveH = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float moveV = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        Vector3 move = new Vector3(moveH, 0.0f, moveV);
        move = move.normalized;
        move *= speed;

        rb.AddForce(move);

        if (rb.position.y <= -5.0f)
        {

            winText.text = ":(";
            StartCoroutine(resetDelay());
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("PickUp"))
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
