using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    // ------ Declare Vars ------
    // Health
    public int absoluteMaxHealth = 3;
    int maxHealth = 3;
    int currHealth = 3;

    // Stamina
    public int maxStamina = 100;
    int currStamina = 100;

    // Grapple
    public bool grappleStatus = true;

    // Haungs mode
    private bool haungsMode = false;

    // ------ Declare methods ------
    
    // Setup
    void Start() {
        Invoke("LateStart", 0.01f);
        //Debug.Log("!!");
    }

    public void LateStart() {
        // Set health
        SetHealth(maxHealth);
    }
    
    // Health

    // Event called when health changes
    public event System.Action HealthChangedEvent = delegate { };

    public void SetHealth(int amt) {
        // Clamp between 0 and maxHealth
        if (!haungsMode) {
            currHealth = Clamp(amt, maxHealth, 0);
        } else {
            currHealth = maxHealth;
        }
        HealthChangedEvent();
    }

    public void ChangeHealth(int amt) {
        // Clamp between 0 and maxHealth
        if (!haungsMode) {
            currHealth = Clamp(currHealth + amt, maxHealth, 0);
        } else {
            currHealth = maxHealth;
        }
        HealthChangedEvent();
    }

    public void ChangeMaxHealth(int amt) {
        // Clamp between 0 and maxHealth
        maxHealth = Clamp(maxHealth + amt, absoluteMaxHealth, 0);
        currHealth = Clamp(currHealth + amt, maxHealth, 0);
        HealthChangedEvent();
    }

    public int GetHealth() {
        return currHealth;
    }

    public int GetMaxHealth() {
        return maxHealth;
    }

    public float GetHealthRatio() {
        return (float)currHealth / (float)maxHealth;
    }

    // Stamina

    // Event called when health changes
    public event System.Action StaminaChangedEvent = delegate { };

    public void SetStamina(int amt) {
        // Clamp between 0 and maxHealth
        if (!haungsMode) {
            currStamina = Clamp(amt, maxStamina, 0);
        } else {
            currStamina = maxStamina;
        }
        StaminaChangedEvent();
    }

    public void ChangeStamina(int amt) {
        // Clamp between 0 and maxStamina
        if (!haungsMode) {
            currStamina = Clamp(currStamina + amt, maxStamina, 0);
        } else {
            currStamina = maxStamina;
        }
        StaminaChangedEvent();
    }

    public int GetStamina() {
        return currStamina;
    }

    public float GetStaminaRatio() {
        return currStamina / maxStamina;
    }

    // Haungs Mode

    public void SetHaungsMode(bool val) {
        //Debug.Log("haungs " + val);
        haungsMode = val;
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
