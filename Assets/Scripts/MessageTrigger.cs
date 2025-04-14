using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTrigger : MonoBehaviour
{
    public TextDisplayManager textDisplayManager;
    public AudioClip soundEffect;
    private AudioSource audioSource;

    private void Start()
    {
        // audioSource = gameObject.AddComponent<AudioSource>();
        // audioSource.playOnAwake = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textDisplayManager.ShowNextMessages();
            //audioSource.PlayOneShot(soundEffect);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textDisplayManager.HideMessage();
        }
    }
}
