using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public InteractableObject[] interactableObjects;

    public void ResetRoom()
    {
        foreach (var i in interactableObjects) {
            i.ResetObjectNow();
        }
    }
}
