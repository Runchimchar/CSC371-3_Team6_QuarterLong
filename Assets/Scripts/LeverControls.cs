using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class LeverControls : MonoBehaviour
{
    // This is for a simple lever where user can press it to activate some event
    bool isToggled; // false means its down, true means its up
    bool isAnimating;
    public Animator anim;
    public bool canDisable; // user is allowed to turn off the lever

    public event Action OnLeverActivate = delegate { }; // user turns it on

    // Timed lever attributes
    bool isTiming;
    public float timerLength = 0;
    public event Action OnLeverDeactivate = delegate { }; // user or timer turns it off

    // Start is called before the first frame update
    void Start()
    {
        isToggled = false;
        isAnimating = false;
        isTiming = false;
        // anim = GetComponent<Animator>();
    }

    public void Interact()
    {
        toggleLever();
    }

    void toggleLever()
    {
        //print("PLAYER TRIES TO INTERACT");
        if (!isAnimating && !isTiming)
        {
            //print("PLAYER SHOULD TOGGLE");
            // Block all interaction during animation
            if (!isToggled)
            {
                //print("LEVER UP");
                LeverUp();
            }
            else if (isToggled && canDisable)
            {
                //print("LEVER DOWN");
                LeverDown();
            }
        }
    }

    void LeverUp()
    {
        isToggled = true;
        isAnimating = true;
        anim.SetTrigger("Up");
        //print("TRIGGER SET");
    }

    void LeverUpPostAnim()
    {
        //print("ANIM DELAY OVER");
        isAnimating = false;
        OnLeverActivate();

        if (timerLength != 0)
        {
            // This is a timed button
            isTiming = true;
            StartCoroutine(OnTimer());
        }
    }
           
    IEnumerator OnTimer() {
        yield return new WaitForSeconds(timerLength);
        isTiming = false;
        LeverDown();
    }

    void LeverDown()
    {
        isToggled = false;
        isAnimating = true;
        anim.SetTrigger("Down");
    }

    void LeverDownPostAnim() {
        isAnimating = false;
        OnLeverDeactivate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //print("PLAYER IN RANGE");
            // Player enters range of lever
            PlayerMovement pm = other.transform.parent.GetComponent<PlayerMovement>();
            pm.InteractEvent += toggleLever;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //print("PLAYER OUT OF RANGE");
            // Player leaves range of lever
            PlayerMovement pm = other.transform.parent.GetComponent<PlayerMovement>();
            pm.InteractEvent -= toggleLever;
        }
    }

    public void Reset()
    {
        // Resets the object to its default position and cancels all animations/coroutines
        isToggled = false;
        isAnimating = false;
        isTiming = false;
        // Handle coroutines
        if (isTiming)
            StopCoroutine("OnTimer");
        // Reset animation
        anim.SetTrigger("Reset");
    }
}
