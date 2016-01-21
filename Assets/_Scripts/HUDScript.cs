using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class HUDScript : MonoBehaviour
{

    public GameObject player;
    public int PUtot;

    [System.Serializable]
    public class readOuts
    {
        public Text speedometer;
        public Text powerText;
        public Text FRmeter;
        public Text PUcounter;
        public Text FPcounter;
        public Text centerText;
    }
    public readOuts HUD = new readOuts();

    private Rigidbody rb;
    private float deltaTime;

    IEnumerator EndDelay(int lvlToload)
    {
        yield return new WaitForSeconds(5);
        Cursor.visible = true;
        SceneManager.LoadScene(lvlToload);
    }

    // Use this for initialization
    void Start ()
    {
        deltaTime = 0.0f;
        HUD.centerText.text = "";
        HUD.speedometer.text = "";
        HUD.FRmeter.text = "";
        HUD.PUcounter.text = "";
        HUD.FPcounter.text = "";
        HUD.powerText.text = "";

        rb = player.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        HUD.speedometer.text = rb.GetRelativePointVelocity(Vector3.zero).magnitude.ToString();
        HUD.powerText.text = "PU:" + player.GetComponent<PlayerControl>().powerUp;
        HUD.PUcounter.text = "PickUps:" + player.GetComponent<PlayerControl>().PUcount.ToString();
        HUD.FPcounter.text = "Pushes:" +player.GetComponent<PlayerControl>().phys.fPushes.ToString();

        if(player.GetComponent<PlayerControl>().PUcount >= PUtot)
        {
            HUD.centerText.text = "You Win!";
            StartCoroutine(EndDelay(0));
        }
        else
        {
            //Check for ball death/lost
            if (rb.position.y <= -5.0f)
            {
                HUD.centerText.text = ":(";
                StartCoroutine(EndDelay(SceneManager.GetActiveScene().buildIndex));
            }
        }

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        HUD.FRmeter.text = "FPS: " + fps.ToString();
    }   
}
