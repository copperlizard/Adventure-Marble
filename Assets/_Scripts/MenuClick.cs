using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuClick : MonoBehaviour {

    public Selectable btn;
    public int btns;

    private bool joypadding, locked;
    private int ind;

    // Use this for initialization
    void Start ()
    {
        joypadding = false;
        ind = 0;
	}

    IEnumerator switchDelay()
    {
        locked = true;
        yield return new WaitForSeconds(0.25f);
        locked = false;
    }

    // Update is called once per frame
    void Update ()
    {
        //float h = Input.GetAxisRaw("Horizontal");

        float v = Input.GetAxisRaw("Vertical");
        float a = Input.GetAxisRaw("Jump");
        if (!joypadding)
        {
            if(v != 0.0f)
            {
                //Debug.Log("Joypadding!");
                joypadding = true;
            }            
        }
        else
        {
            if(v > 0.0f && ind > 0 && !locked)
            {
                btn = btn.FindSelectableOnUp();
                ind -= 1;
                StartCoroutine(switchDelay());
            }
            else if(v < 0.0f && ind < btns - 1 && !locked)
            {
                btn = btn.FindSelectableOnDown();
                ind += 1;
                StartCoroutine(switchDelay());
            }
            btn.Select();

            if(a > 0.0f)
            {
                btn.Invoke("OnClick()", 0.0f);
            }
        }        
	}

    // Load Scene
    public void load( int ind )
    {
        SceneManager.LoadScene( ind );
    }

    // End application
    public void quitGame()
    {
        Application.Quit();
    }
}
