using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // TODO: Add GameController related things

    public static bool paused;

    private void Start()
    {
        
    }

    public static void StartLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
}