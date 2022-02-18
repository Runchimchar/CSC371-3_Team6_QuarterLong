using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    // ------ Declare Vars ------
    // Health
    public int maxHealth = 3;
    int currHealth = 3;

    // Stamina
    public int maxStamina = 100;
    int currStamina = 100;

    // ------ Declare methods ------
    // Health
    public void SetHealth(int amt) {
        // Clamp between 0 and maxHealth
        currHealth = Clamp(amt, maxHealth, 0);
    }

    public void ChangeHealth(int amt) {
        // Clamp between 0 and maxHealth
        currHealth = Clamp(currHealth + amt, maxHealth, 0);
    }

    public int GetHealth() {
        return currHealth;
    }

    public float GetHealthRatio() {
        return currHealth / maxHealth;
    }

    // Stamina
    public void SetStamina(int amt) {
        // Clamp between 0 and maxHealth
        currStamina = Clamp(amt, maxStamina, 0);
    }

    public void ChangeStamina(int amt) {
        // Clamp between 0 and maxStamina
        currStamina = Clamp(currStamina + amt, maxStamina, 0);
    }

    public int GetStamina() {
        return currStamina;
    }

    public float GetStaminaRatio() {
        return currStamina / maxStamina;
    }

    // ------ Helpers ------
    // Returns val clamped between values of max and min
    private static int Clamp(int val, int max, int min) {
        if (val > max) {
            return max;
        } else if (val < min) {
            return min;
        } else {
            return val;
        }
    }
}
