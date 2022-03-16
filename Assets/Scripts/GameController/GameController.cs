using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;
    public static PlayerStatus playerStatus = null;
    public static MessageController messageController = null;
    public static SceneController sceneController = null;

    public GameObject UI = null;

    public void EnableUI(bool enabled) {
        CanvasGroup cg = UI.GetComponent<CanvasGroup>();
        MenuController mc = UI.GetComponent<MenuController>();

        if (enabled) {
            cg.alpha = 1f;
            mc.isControllable = true;
        } else {
            cg.alpha = 0f;
            mc.isControllable = false;
            mc.Resume();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        // Make singleton
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Convenient access to other scripts
        playerStatus = GetComponent<PlayerStatus>();
        messageController = GetComponent<MessageController>();
        sceneController = GetComponent<SceneController>();
    }
}
