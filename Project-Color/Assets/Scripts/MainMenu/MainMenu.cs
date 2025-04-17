using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Scene level;

    public void Play()
    {
        Debug.Log("Play");
        SceneManager.LoadScene(2);
    }

    public void Levels()
    {
        Debug.Log("Levels");
    }
}
