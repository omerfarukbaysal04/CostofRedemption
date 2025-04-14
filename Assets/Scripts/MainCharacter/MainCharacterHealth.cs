using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCharacterHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    private MainCharacterHealthBar mainCharacterHealthBar;
    public GameObject damageEffectPrefab;
    public Transform effectSpawnPoint;


    void Awake()
    {
        mainCharacterHealthBar = GetComponentInChildren<MainCharacterHealthBar>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }
    public void Update()
    {
        HandleHealth();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        mainCharacterHealthBar.UpdateHealthBar(currentHealth, maxHealth);
        TriggerDamageEffect();


        if (currentHealth <= 0)
        {
            Die();
            RestartLevel();
        }
    }

    private void TriggerDamageEffect()
    {
        if (damageEffectPrefab != null && effectSpawnPoint != null)
        {
            GameObject effect = Instantiate(damageEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }

    void HandleHealth()
    {
        if (currentHealth > 100)
        {
            currentHealth = 100;
        }
    }

    private void Die()
    {
        Destroy(gameObject, 0.8f);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        mainCharacterHealthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
