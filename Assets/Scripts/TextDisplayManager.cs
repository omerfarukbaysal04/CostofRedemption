using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplayManager : MonoBehaviour
{
    public List<string> messages;
    public TextMeshProUGUI uiText;
    private int currentIndex = 0;
    public UnityEngine.UI.Image backgroundImage;

    void Start()
    {
        uiText.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
    }

    public void ShowNextMessages()
    {
        if (currentIndex < messages.Count)
        {
            backgroundImage.gameObject.SetActive(true);
            uiText.gameObject.SetActive(true);

            uiText.text = messages[currentIndex];
            currentIndex++;
        }

        else
        {
            HideMessage();
        }
    }

    public void HideMessage()
    {
        backgroundImage.gameObject.SetActive(false);
        uiText.gameObject.SetActive(false);
    }
}
