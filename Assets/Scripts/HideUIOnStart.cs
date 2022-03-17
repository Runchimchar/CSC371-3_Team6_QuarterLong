using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUIOnStart : MonoBehaviour
{
    void Start()
    {
        GameController.instance.EnableUI(false);
    }
}
