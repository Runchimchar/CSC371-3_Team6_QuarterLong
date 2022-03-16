using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour {
    
    public static GameObject instance = null;

    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public bool isControllable = true;

    private bool isPaused = false;

    // On creation
    public void Awake() {
        //Debug.Log("Awoke");
        // Make singleton
        if (instance == null) {
            instance = gameObject;
            // No longer a root object
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }
        // Add event listener
        PlayerMovement.CreateEvent += GetPlayer;
        GetPlayer();
    }

    public void GetPlayer() {
        if (GameObject.FindWithTag("Player")) {
            PlayerMovement pm = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            pm.PauseEvent -= GetPause;
            pm.PauseEvent += GetPause;
        }
    }

    // Get inputs
    public void GetPause() {
        // Tab = menu 
        //Debug.Log("Got Pause");
        if (isControllable) {
            if (isPaused) {
                Resume();
            }
            else {
                Pause();
            }
        }
    }

    // Pause game and pull up menu
    public void Pause() {
        //Debug.Log("Paused");
        isPaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        OpenPause();
    }

    // Resume game and close all menus
    public void Resume() {
        //Debug.Log("Resume");
        isPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    // Open pause menu
    public void OpenPause() {
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    // Open options menu
    public void OpenOptions() {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    // Getter
    public bool GetPaused() {
        return isPaused;
    }
}
