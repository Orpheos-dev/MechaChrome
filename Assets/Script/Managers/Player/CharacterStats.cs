//using UnityEngine;
//using UnityEngine.Events;
//
//public class CharacterStats : MonoBehaviour
//{
//    public int maxHealth = 100; // Maximum health
//    private int currentHealth;  // Current health
//
//    public UIManager uiManager; // Reference to UIManager
//
//    public UnityEvent onCharacterDeath; // Event to trigger when the character dies
//	
//    public UnityEngine.UI.Slider healthSlider;
//
//    private void Start()
//    {
//        // Initialize current health
//        currentHealth = maxHealth;
//
//        // Initialize health bar
//        healthSlider.maxValue = maxHealth;
//        healthSlider.value = currentHealth;
//    }
//
//    public void TakeDamage(int damage)
//    {
//        // Decrease the health
//        currentHealth -= damage;
//        currentHealth = Mathf.Max(currentHealth, 0); // Ensure health doesn't go below 0
//
//        // Update health bar
//        healthSlider.value = currentHealth;
//
//        // If the player's health is zero, change the scene
//        if (currentHealth <= 0)
//        {
//            uiManager.ToggleCanvas(5);
//        }
//    }
//
//    public void Heal(int amount)
//    {
//        // Increase the health
//        currentHealth += amount;
//        currentHealth = Mathf.Min(currentHealth, maxHealth); // Ensure health doesn't exceed max
//
//        // Update health bar
//        healthSlider.value = currentHealth;
//    }
//
//    public bool IsDead()
//    {
//        return currentHealth <= 0;
//    }
//
//    public int GetCurrentHealth()
//    {
//        return currentHealth;
//    }
//}
//