using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class HUDScript : MonoBehaviour
{

    public GameObject player;
    public int PUtot;
    public int thisLevel;

    [System.Serializable]
    public class readOuts
    {
        public Text speedometer, powerText, FRmeter, PUcounter, FPcounter, centerText, timerText;
    }
    public readOuts HUD = new readOuts();

    private Rigidbody rb;
    private float deltaTime, finishTime;
    private bool gameOver;    

    IEnumerator EndDelay(int lvlToload)
    {
        HUD.centerText.text = "You Win!";
        yield return new WaitForSeconds(5);       

        bool newRecord = false;

        /*
        //Check time records
        for(int i = 0; i < DataManager.saves[thisLevel].times.Length; i++)
        {
            //If new record
            if(finishTime < DataManager.saves[thisLevel].times[i])
            {
                newRecord = true;

                //Store recs to move
                int oldrecCount = DataManager.saves[thisLevel].times.Length - i;
                float[] oldrecs = new float[oldrecCount];
                for(int j = 0; j < oldrecCount; j++)
                {
                    oldrecs[j] = DataManager.saves[thisLevel].times[i + j];
                }

                //Store new rec
                DataManager.saves[thisLevel].times[i] = finishTime;

                //Store all but 1 of the old recs
                for (int j = 0; j < oldrecCount - 1; j++)
                {
                    DataManager.saves[thisLevel].times[i + j] = oldrecs[j];
                }                

                break;
            }
        }
        */

        //Build message
        string scoreboard = "";

        if(newRecord)
        {
            scoreboard = "NEW RECORD!";
        }

        /*
        for(int i = 0; i < 10; i++)
        {
            scoreboard += "\n" + i.ToString() + " " + DataManager.saves[thisLevel].times[i].ToString();
        } 
        */       

        HUD.centerText.text = scoreboard;

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

        gameOver = false;

        rb = player.GetComponent<Rigidbody>();

        if(!DataManager.loaded)
        {
            DataManager.load();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        HUD.speedometer.text = rb.GetRelativePointVelocity(Vector3.zero).magnitude.ToString();
        HUD.powerText.text = "PU:" + player.GetComponent<PlayerControl>().powerUp;
        HUD.FPcounter.text = "Pushes:" + player.GetComponent<PlayerControl>().phys.fPushes.ToString();

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        HUD.FRmeter.text = "FPS: " + fps.ToString();

        if (!gameOver)
        {
            
            HUD.PUcounter.text = "PickUps:" + player.GetComponent<PlayerControl>().PUcount.ToString() + "/" + PUtot.ToString();            
            HUD.timerText.text = Time.timeSinceLevelLoad.ToString();

            if (player.GetComponent<PlayerControl>().PUcount >= PUtot)
            {
                finishTime = Time.timeSinceLevelLoad;
                gameOver = true;
                StartCoroutine(EndDelay(0));
            }
            else
            {
                //Check for ball death/lost
                if (rb.position.y <= -5.0f)
                {
                    HUD.centerText.text = ":(";
                    gameOver = true;
                    StartCoroutine(EndDelay(SceneManager.GetActiveScene().buildIndex));
                }
            }            
        }        
    }   
}
