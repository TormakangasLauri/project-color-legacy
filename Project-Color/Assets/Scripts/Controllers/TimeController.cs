using System.Collections;
using System.Collections.Generic;
using Controllers;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static float timeScale { get; private set; }
    public static bool paused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) SetTimeScale(paused ? 1 : 0);
    }

    public static void SetTimeScale(float scale)
    {
        timeScale = scale >= 0 ? scale : 0; // Clamp minimum to 0
        if (timeScale == 0) paused = true;
        else paused = false;
            foreach (GameObject enemy in EnemyController.all.activeList) if (enemy != null && !enemy.IsDestroyed()) enemy.GetComponent<EnemyType>().timeScale = scale;
        foreach (GameObject enemy in EnemyController.all.inactiveList) if (enemy != null && !enemy.IsDestroyed()) enemy.GetComponent<EnemyType>().timeScale = scale;
    }

    public static void Pause() { SetTimeScale(0); }
    public static void Unpause() { SetTimeScale(1); }
    public static void ResetTimeScale() { SetTimeScale(1); }
}
