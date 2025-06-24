using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndPos : MonoBehaviour
{
    // Loads the next level (see build settings) when trigger is entered.
    private void OnTriggerEnter(Collider other)
    {
        GameController.LoadLevel(SceneManager.GetActiveScene().buildIndex+1);
    }
}
