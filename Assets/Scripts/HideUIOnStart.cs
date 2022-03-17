using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUIOnStart : MonoBehaviour
{
    public void Start()
    {
        GameController.instance.EnableUI(false);

    }
    public void GameQuit()
    {
        Application.Quit();
    }
}
