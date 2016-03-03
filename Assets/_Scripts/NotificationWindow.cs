using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NotificationWindow : MonoBehaviour
{
    private Animator ani;
    private CanvasGroup cgroup;
    private InputField infield;
    private string playername, startTxt;
    private bool shouldOpen;

    IEnumerator checkDelay()
    {
        //DOESN'T STOP UNTIL OBJECT DESTORYED
        while (true)
        {
            yield return new WaitForSeconds(3);
            playername = PlayerPrefs.GetString("PlayerName", "");
            if (playername == "" && !shouldOpen)
            {
                infield.text = startTxt;

                Debug.Log("shouldOpen == true");
                shouldOpen = true;                
            }
            else if(playername != "" && shouldOpen)
            {
                Debug.Log("shouldOpen == false");
                shouldOpen = false;
            }            
        }        
    }

    public bool isOpen
    {
        get { return ani.GetBool("isOpen"); }
        set { ani.SetBool("isOpen", value); shouldOpen = value; }
    }    

    // Called when loaded
    public void awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        ani = GetComponent<Animator>();
        cgroup = GetComponentInChildren<CanvasGroup>();

        //Reset canvas position to screen center
        RectTransform rect = GetComponent<RectTransform>();
        rect.offsetMax = Vector2.zero;
        rect.offsetMin = Vector2.zero;

        infield = GetComponentInChildren<InputField>();
        startTxt = infield.text;

        playername = PlayerPrefs.GetString("PlayerName", "");

        if (playername == "")
        {
            shouldOpen = true;
        }
        else
        {
            shouldOpen = false;
        }
        StartCoroutine(checkDelay());
    }

    // Update is called once per frame
    void Update ()
    {        
        if (shouldOpen && !isOpen)
        {
            isOpen = true;
            Debug.Log("isOpen == true");
        }
        else if(!shouldOpen && isOpen)
        {
            isOpen = false;
            Debug.Log("isOpen == false");
        }
        

        if (!ani.GetCurrentAnimatorStateInfo(0).IsName("open"))
        {
            cgroup.blocksRaycasts = false;
            cgroup.interactable = false;
        }
        else
        {
            cgroup.blocksRaycasts = true;
            cgroup.interactable = true;
        }
    }
}
