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
    [SerializeField] private GameObject _crosshair = null;
    [SerializeField] private CanvasGroup _statsContainer = null;
    [SerializeField] private GameObject _levelMusic = null;
    [SerializeField] private GameObject _bossMusic = null;

    public void EnableUI(bool enabled) {
        CanvasGroup cg = UI.GetComponent<CanvasGroup>();
        MenuController mc = UI.GetComponent<MenuController>();

        if (enabled) {
            //cg.alpha = 1f;
            mc.isControllable = true;
            _crosshair.SetActive(true);
            _statsContainer.alpha = 1;

        } else {
            //cg.alpha = 0f;
            mc.isControllable = false;
            _crosshair.SetActive(false);
            _statsContainer.alpha = 0;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            //mc.Resume();
        }
    }

    public static void QuitGame()
    {
        Application.Quit();
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

    public void StartBossMusic()
    {
        _levelMusic.SetActive(false);
        _bossMusic.SetActive(true);
    }

    public void StartLevelMusic()
    {
        _levelMusic.SetActive(true);
        _bossMusic.SetActive(false);
    }
}
