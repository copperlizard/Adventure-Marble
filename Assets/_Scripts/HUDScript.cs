using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class HUDScript : MonoBehaviour
{

    public GameObject player;
    public int PUtot;
    public int thisLevel;
    [HideInInspector]
    public bool gameOver;

    [System.Serializable]
    public class readOuts
    {
        public Text speedometer, powerText, FRmeter, PUcounter, FPcounter, centerText, timerText;
    }
    public readOuts HUD = new readOuts();

    private Rigidbody rb;
    private float deltaTime, finishTime, hFPS, lFPS;        //GET RID OF FPS VARS LATER!!!

    IEnumerator EndDelay(int lvlToload, bool win = false)
    {
        int iat = -1;
        if(win)
        {
            HUD.centerText.text = "You Win!";
            yield return new WaitForSeconds(2);

            bool newRecord = false;

            //Check time records
            for (int i = 0; i < DataManager.saves[thisLevel].times.Length; i++)
            {
                //If new record
                if (finishTime < DataManager.saves[thisLevel].times[i])
                {
                    newRecord = true;
                    iat = i;

                    //Store recs to move
                    int oldrecCount = DataManager.saves[thisLevel].times.Length - i;
                    float[] oldrecTimes = new float[oldrecCount];
                    string[] oldrecNames = new string[oldrecCount];
                    for(int j = 0; j < oldrecCount; j++)
                    {
                        oldrecTimes[j] = DataManager.saves[thisLevel].times[i + j];
                        oldrecNames[j] = DataManager.saves[thisLevel].names[i + j];                        
                    }

                    //Store new rec
                    DataManager.saves[thisLevel].times[i] = finishTime;
                    DataManager.saves[thisLevel].names[i] = PlayerPrefs.GetString("PlayerName");
                    
                    //Store all but 1 of the old recs
                    for (int j = 0; j < oldrecCount - 1; j++)
                    {
                        DataManager.saves[thisLevel].times[i + j + 1] = oldrecTimes[j];
                        DataManager.saves[thisLevel].names[i + j + 1] = oldrecNames[j];
                    }

                    //Save data
                    DataManager.save();
                    break;
                }
            }

            //Build message
            string scoreboard = "";

            if (newRecord)
            {
                scoreboard = "NEW RECORD!";
            }

            for (int i = 0; i < 10; i++)
            {
                string colMod = "";
                if(i == iat)
                {
                    colMod = "<color=#FFFFFF>";
                }
                scoreboard += colMod + "\n#" + (i + 1).ToString() + ": " + DataManager.saves[thisLevel].names[i] + " - " + DataManager.saves[thisLevel].times[i].ToString() + ((colMod != "") ? "</color>":"");
            }

            HUD.centerText.text = scoreboard;

            yield return new WaitForSeconds(10);
            Cursor.visible = true;
            SceneManager.LoadScene(lvlToload);
        }
        else
        {
            HUD.centerText.text = "g g";
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene(lvlToload);
        }
    }

    // Use this for initialization
    void Start ()
    {
        deltaTime = 0.0f;
        finishTime = 0.0f;
        HUD.centerText.text = "";
        HUD.speedometer.text = "";
        HUD.FRmeter.text = "";
        HUD.PUcounter.text = "";
        HUD.FPcounter.text = "";
        HUD.powerText.text = "";

        thisLevel -= 1; //account for zero index

        gameOver = false;

        rb = player.GetComponent<Rigidbody>();

        if(!DataManager.loaded)
        {
            DataManager.load();
        }

        //GET RID OF THESE LATER!!!
        hFPS = 0.0f;
        lFPS = 60.0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        HUD.speedometer.text = rb.GetRelativePointVelocity(Vector3.zero).magnitude.ToString();
        HUD.powerText.text = "Power:" + player.GetComponent<PlayerControl>().powerUp;
        HUD.FPcounter.text = "Pushes:" + player.GetComponent<PlayerControl>().powps.fPushes.ToString();

        ///////////////////////
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        if (fps < lFPS && Time.deltaTime > 0.001f)
        {
            lFPS = fps;
        }
        else if (fps > hFPS && Time.deltaTime > 0.001f)
        {
            hFPS = fps;
        }
        HUD.FRmeter.text = "FPS: " + ((Time.deltaTime > 0.001f)?fps.ToString():"PAUSED") + "\nLow FPS: " + lFPS.ToString() + "\nHigh FPS: " + hFPS.ToString();
        ///////////////////////

        if (!gameOver)
        {
            
            HUD.PUcounter.text = "PickUps:" + player.GetComponent<PlayerControl>().PUcount.ToString() + "/" + PUtot.ToString();            
            HUD.timerText.text = "Time:" + Time.timeSinceLevelLoad.ToString();

            if (player.GetComponent<PlayerControl>().PUcount >= PUtot)
            {
                finishTime = Time.timeSinceLevelLoad;
                gameOver = true;
                StartCoroutine(EndDelay(0, true));
            }
            else
            {
                //Check for ball death/lost
                if (rb.position.y <= -5.0f)
                {                    
                    gameOver = true;
                    StartCoroutine(EndDelay(SceneManager.GetActiveScene().buildIndex, false));
                }
            }            
        }        
    }   
}
