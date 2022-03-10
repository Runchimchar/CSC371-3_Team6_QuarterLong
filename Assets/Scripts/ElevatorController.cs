using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour {
    public GameObject doorCollider;
    public ButtonControls button;
    public bool startingElevator;

    public AudioSource dingAudio;
    public AudioSource humAudio;

    public MessageController.MessageDesc[] messages;

    private Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        button.toggleDelay = 99999f;
        button.OnButtonActivate += StartDoorChange;
    }

    // Change door state
    private void StartDoorChange() {
        dingAudio.Play();
        if (startingElevator) {
            anim.Play("OpenAnim");
        } else {
            anim.Play("CloseAnim");
            doorCollider.SetActive(true);
        }
    }

    private void FinishDoorChange() {
        // Remove invisible wall
        if (startingElevator) {
            doorCollider.SetActive(false);
        } else {
            humAudio.Play();
        }
        // Queue Messages
        for (int i = 0; i < messages.Length; i++)
            GameController.messageController.QueueMessage(messages[i]);
    }
}
