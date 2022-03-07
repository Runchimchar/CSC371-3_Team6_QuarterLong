using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTriggerController : MonoBehaviour
{
    public MessageController.MessageDesc[] messages;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            // Add all stored messages in order
            for (int i = 0; i < messages.Length; i++) {
                GameController.messageController.QueueMessage(messages[i]);
            }
            // Our job is done
            Destroy(gameObject);
        }
    }
}