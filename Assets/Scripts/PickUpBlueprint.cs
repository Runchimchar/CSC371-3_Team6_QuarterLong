using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBlueprint : MonoBehaviour
{
    SceneController sc;

    private void Start()
    {
        sc = GameController.sceneController;
    }

    void PickUp()
    {
        sc.ChangeScene("EndScene");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PickUp();
        }
    }
}
