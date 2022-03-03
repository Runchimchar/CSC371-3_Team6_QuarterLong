using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageController : MonoBehaviour {
    private static int NUMBER_INDEX = 1;
    private static int SENDER_INDEX = 2;
    private static int MESSAGE_INDEX = 3;

    public Transform messageBox;
    public GameObject messagePrefab;

    private Queue<MessageDesc> messageQueue = new Queue<MessageDesc>();
    private GameObject currentMessage = null;

    // Struct containing all parts of a text message
    public readonly struct MessageDesc {
        public MessageDesc(int number, string sender, string message) {
            this.number = number;
            this.sender = sender;
            this.message = message;
        }

        public int number { get; }
        public string sender { get; }
        public string message { get; }
    }

    // Setup
    void Start() {
        GetBox();
        Invoke("TestMessages", 2f);
    }

    // Testing messages
    private void TestMessages() {
        Debug.Log("Test start!");

        MessageDesc test1 = new MessageDesc(1, "Little Nerd", "Can I get uhhhhh, uhh uhhhhh uh uhhhh uhh uhhh uhhhhhhh");
        MessageDesc test2 = new MessageDesc(2, "Little Nerd", "MDonals hmamburgur");

        QueueMessage(test1);
        QueueMessage(test2);
    }

    // Add new message to the message queue
    public void QueueMessage(MessageDesc message) {
        if (currentMessage == null) {
            CreateMessage(message);
        } else {
            messageQueue.Enqueue(message);
        }
    }
    public void QueueMessage(int number, string sender, string message) {
        QueueMessage(new MessageDesc(number, sender, message));
    }

    // Look for text message box until found
    private void GetBox() {
        messageBox = GameObject.FindGameObjectWithTag("Notification Box").transform;
        if (messageBox == null) {
            Invoke("GetBox", 0.1f);
        }
    }

    // Create message UI and fill contents
    private void CreateMessage(MessageDesc message) {
        // Create message clone
        currentMessage = Instantiate(messagePrefab, messageBox);
        currentMessage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // Setup cleanup event
        currentMessage.GetComponent<TextMessageDestroyer>().DestroyEvent += CurrentMessageDestroyed;

        // Configure message contents
        Transform bounds = currentMessage.transform.GetChild(0);
        bounds.GetChild(NUMBER_INDEX).GetComponent<TMP_Text>().text = message.number.ToString();
        bounds.GetChild(SENDER_INDEX).GetComponent<TMP_Text>().text = message.sender;
        bounds.GetChild(MESSAGE_INDEX).GetComponent<TMP_Text>().text = message.message;
    }

    // Event fired on the end of a text message
    private void CurrentMessageDestroyed() {
        // Cleanup event
        currentMessage.GetComponent<TextMessageDestroyer>().DestroyEvent -= CurrentMessageDestroyed;
        currentMessage = null;

        // Prepare next message
        if (messageQueue.Count > 0) {
            CreateMessage(messageQueue.Dequeue());
        }
    }
}
