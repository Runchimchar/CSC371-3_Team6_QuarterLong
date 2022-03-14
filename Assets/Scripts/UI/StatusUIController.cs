using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUIController : MonoBehaviour
{
    public Gradient healthColor;
    public Color disabledColor;
    public GameObject[] healthCells;
    public Slider staminaBar;

    private PlayerStatus stat;

    private int currHealth;
    private float currStamina;

    // Setup
    void Start() {
        // Ease of use
        stat = GameController.playerStatus;

        // Setup events
        stat.HealthChangedEvent += UpdateHealth;
        stat.StaminaChangedEvent += UpdateStamina;
    }

    // Cleanup
    private void OnDestroy() {
        // Remove events
        GameController.playerStatus.HealthChangedEvent -= UpdateHealth;
        GameController.playerStatus.StaminaChangedEvent -= UpdateStamina;
    }

    // Update Health UI
    public void UpdateHealth() {
        currHealth = stat.GetHealth();

        // Set color
        for (int i = 0; i < healthCells.Length; i++) {
            if (i < currHealth) {
                healthCells[i].GetComponent<Image>().color = healthColor.Evaluate(stat.GetHealthRatio());
            } else {
                healthCells[i].GetComponent<Image>().color = disabledColor;
            }
        }

        // Disable extra health
        for (int i = 0; i < healthCells.Length; i++) {
            if (i < stat.GetMaxHealth()) {
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
