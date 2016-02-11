using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    private Animator ani;
    private CanvasGroup cgroup;

    public bool isOpen
    {
        get { return ani.GetBool("isOpen"); }
        set { ani.SetBool("isOpen", value); }
    }

    // Called when loaded
    public void awake ()
    {
        
    }

	// Use this for initialization
	void Start ()
    {
        ani = GetComponent<Animator>();
        cgroup = GetComponent<CanvasGroup>();

        //Reset canvas position to screen center
        RectTransform rect = GetComponent<RectTransform>();
        rect.offsetMax = Vector2.zero;
        rect.offsetMin = Vector2.zero;
    }
	
	// Update is called once per frame
	public void Update ()
    {
        if(!ani.GetCurrentAnimatorStateInfo(0).IsName("Open"))
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
