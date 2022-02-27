using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUIController : MonoBehaviour
{
    public GameObject[] healthCells;
    public Slider staminaBar;

    private PlayerStatus stat;

    private int currHealth;
    private int currStamina;

    // Setup
    void Start() {
        // Ease of use
        stat = GameController.playerStatus;

        // Setup events
        GameController.playerStatus.HealthChangedEvent += UpdateHealth;
        GameController.playerStatus.StaminaChangedEvent += UpdateStamina;
    }

    // Cleanup
    private void OnDestroy() {
        // Remove events
        stat.HealthChangedEvent -= UpdateHealth;
        stat.StaminaChangedEvent -= UpdateStamina;
    }

    // Update Health UI
    public void UpdateHealth() {
        currHealth = stat.GetHealth();
        for (int i = 0; i < healthCells.Length; i++) {
            if (i < currHealth) {
                healthCells[i].SetActive(true);
            } else {
                healthCells[i].SetActive(false);
            }
        }
    }

    // Update Stamina UI
    public void UpdateStamina() {
        currStamina = stat.GetStamina();
        staminaBar.value = currStamina;
    }
}
