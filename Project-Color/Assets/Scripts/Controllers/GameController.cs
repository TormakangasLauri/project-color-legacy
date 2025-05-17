using System.Collections;
using System.Collections.Generic;
using AASave;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int levelIndex;
    public int currentCheckpoint;

    private static SaveSystem saveSystem;
    public static bool paused;

    public static GameController inst;

    private void Awake() { inst = this; }

    private void OnValidate()
    {
        saveSystem = GetComponent<SaveSystem>();
        levelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) EndLevel();
    }

    public void SaveLevel()
    {
        GameData.SaveLevelData(levelIndex, currentCheckpoint);
    }

    public static void StartLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
    
    public void EndLevel()
    {
        GameData.SaveLevelData(levelIndex, currentCheckpoint, true);
        GameData.SaveAllDataToFile(saveSystem);
        HUDText.SetText(new[]{0}, new[]{"Level complete !!!"}, HUDTextFill.Fill);
        HUDText.stopUpdates = true;
        StartCoroutine(End());
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

    private void OnApplicationQuit()
    {
        GameData.SaveAllDataToFile(saveSystem);
    }
}