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

    private static Dictionary<string, int> countTable = new Dictionary<string, int>();

    private Queue<MessageDesc> messageQueue = new Queue<MessageDesc>();
    private GameObject currentMessage = null;

    public event System.Action QueueClearedEvent = delegate { };

    // Struct containing all parts of a text message
    [System.Serializable]
    public struct MessageDesc {
        public MessageDesc(string sender, string message) {
            this.sender = sender;
            this.message = message;
        }
        public string sender;
        public string message;
    }

    // Setup
    void Start() {
        GetBox();
        //Invoke("TestMessages", 2f);
    }

    // Testing messages
    private void TestMessages() {
        Debug.Log("Test start!");

        MessageDesc test1 = new MessageDesc("Little Nerd", "Can I get uhhhhh, uhh uhhhhh uh uhhhh uhh uhhh uhhhhhhh");
        MessageDesc test2 = new MessageDesc("Little Nerd", "MDonals hmamburgur");

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
    public void QueueMessage(string sender, string message) {
        QueueMessage(new MessageDesc(sender, message));
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

        // Get text count
        int number = 0;
        if (countTable.ContainsKey(message.sender)) {
            number = countTable[message.sender];
        } else {
            countTable.Add(message.sender, 1);
            number = 1;
        }
        countTable[message.sender] = number + 1;

        // Configure message contents
        Transform bounds = currentMessage.transform.GetChild(0);
        bounds.GetChild(NUMBER_INDEX).GetComponent<TMP_Text>().text = number.ToString();
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
        } else {
            QueueClearedEvent();
        }
    }
}
