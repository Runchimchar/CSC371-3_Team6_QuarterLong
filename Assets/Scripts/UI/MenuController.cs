using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour {
    
    public static GameObject instance = null;

    public GameObject pauseMenu;
    public GameObject optionsMenu;

    private bool isPaused = false;

    // On creation
    public void Awake() {
        //Debug.Log("Awoke");
        // Make singleton
        if (instance == null) {
            instance = gameObject;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        // Add event listener
        PlayerMovement pm = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        //if (pm == null) Debug.Log("Was null");
        pm.PauseEvent += GetPause;
    }

    // Get inputs
    public void GetPause() {
        // Tab = menu 
        //Debug.Log("Got Pause");
        if (isPaused) {
            Resume();
        }
        else {
            Pause();
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
