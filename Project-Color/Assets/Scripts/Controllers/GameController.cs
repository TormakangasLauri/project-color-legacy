using System.Collections;
using System.Collections.Generic;
using AASave;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static SaveSystem saveSystem;

    public static int currentLevelIndex;

    public static bool paused;

    public static GameController inst;

    private void Awake() { inst = this; }

    private void OnValidate()
    {
        saveSystem = GetComponent<SaveSystem>();
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) EndLevel();
    }

    public static void StartLevel(int level)
    {
        SceneManager.LoadScene(level);
        currentLevelIndex = level;
    }

    public static void EndLevel()
    {
        GameData.SaveLevelData(currentLevelIndex, true);
        GameData.SaveAllDataToFile(saveSystem);
        HUDText.SetText(new[]{0}, new[]{"Level complete !!!"}, HUDTextFill.Fill);
        HUDText.stopUpdates = true;
        inst.StartCoroutine(End());
        IEnumerator End()
        {
            float timer = 2;
            yield return new WaitUntil(() =>
            {
                timer -= Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Max(0, timer/2);
                return timer <= 0;
            });
            HUDText.stopUpdates = false;
            StartLevel(0);
            Time.timeScale = 1;
        }
    }
}