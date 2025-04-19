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
        GameController.StartLevel(1);
    }

    public void Levels()
    {
        Debug.Log("Levels");
    }
}
