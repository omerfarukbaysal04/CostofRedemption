using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem : MonoBehaviour
{
    public float healthIncrease = 10f;
    private AudioSource audioSource;
    public AudioClip audioClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MainCharacterHealth mainCharacterHealth = other.GetComponent<MainCharacterHealth>();
            if (mainCharacterHealth != null)
            {
                mainCharacterHealth.Heal(healthIncrease);
            }

            if (audioClip != null)
            {
                AudioSource.PlayClipAtPoint(audioClip, transform.position);
            }

            Destroy(gameObject);
        }
    }
}
