using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void End1()
    {
        SceneManager.LoadScene("End1");
    }
    public void End2()
    {
        SceneManager.LoadScene("End2");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
