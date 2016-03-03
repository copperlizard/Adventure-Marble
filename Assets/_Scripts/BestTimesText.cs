using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BestTimesText : MonoBehaviour
{
    public int numLvls;
    private int curLvl, dispLvl;

    private Text messageText;

	// Use this for initialization
	void Start ()
    {
        messageText = GetComponent<Text>();

        curLvl = 0;
        dispLvl = -1;

        if (!DataManager.loaded)
        {
            DataManager.load();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(curLvl != dispLvl)
        {
            dispLvl = curLvl;

            //Build message
            string scoreboard = "Level #" + (curLvl + 1).ToString();

            for(int i = 0; i < 10; i++)
            {
                scoreboard += "\n#" + (i + 1).ToString() + ": " + DataManager.saves[curLvl].names[i] + " - " + DataManager.saves[curLvl].times[i].ToString();
            }

            messageText.text = scoreboard;
        }	
	}

    public void nextLevel()
    {
        curLvl = Mathf.Clamp(curLvl + 1, 0, numLvls - 1);
    }

    public void prevLevel()
    {
        curLvl = Mathf.Clamp(curLvl - 1, 0, numLvls - 1);
    }
}
