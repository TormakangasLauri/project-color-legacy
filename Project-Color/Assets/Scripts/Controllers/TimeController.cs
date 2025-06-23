using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEditor;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static float timeScale { get; private set; }
    public static bool paused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y)) SetTimeScale(0);
        if (Input.GetKeyDown(KeyCode.U)) SetTimeScale(1);
    }

    public static void SetTimeScale(float scale)
    {
        timeScale = scale >= 0 ? scale : 0; // Clamp minimum to 0
        if (timeScale == 0) paused = true;
        else paused = false;
            foreach (GameObject enemy in EnemyController.all.activeList) enemy.GetComponent<EnemyType>().timeScale = scale;
        foreach (GameObject enemy in EnemyController.all.inactiveList) enemy.GetComponent<EnemyType>().timeScale = scale;
    }

    public static void Pause() { SetTimeScale(0); }
    public static void Unpause() { SetTimeScale(1); }
    public static void ResetTimeScale() { SetTimeScale(1); }
}
