using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ButtonControls : MonoBehaviour
{
    // This is for a simple button where user can press it to activate some event
    bool isToggled;
    public float toggleDelay = 2f;

    Renderer rend;
    public Material inactiveMat;
    public Material activeMat;

    public event Action OnButtonActivate = delegate { };
    public event Action OnButtonDeactivate = delegate { };

    // Start is called before the first frame update
    void Start()
    {
        isToggled = false;
        rend = GetComponent<Renderer>();

    }

    /*
    private void Update()
    {
        Keyboard kb = InputSystem.GetDevice<Keyboard>();
        if (kb.spaceKey.wasPressedThisFrame)
        {
            toggleButtonOn();
        }
    }
    */
    public void Interact()
    {
        toggleButtonOn();
    }

    public void toggleButtonOn()
    {
        if (!isToggled)
        {
            // this prevents spamming
            isToggled = true;
            rend.material = activeMat;
            // TODO call related event here
            print("ACTIVATED");
            OnButtonActivate();
            StartCoroutine(toggleButtonOff(toggleDelay));
        }
    }
    IEnumerator toggleButtonOff(float time)
    {
        yield return new WaitForSeconds(time);

        OnButtonDeactivate();
        isToggled = false;
        rend.material = inactiveMat;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Player enters range of physical button press
            PlayerMovement pm = other.transform.parent.GetComponent<PlayerMovement>();
            pm.InteractEvent += toggleButtonOn;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Player leaves range of physical button press
            PlayerMovement pm = other.transform.parent.GetComponent<PlayerMovement>();
            pm.InteractEvent -= toggleButtonOn;
        }
    }
}
