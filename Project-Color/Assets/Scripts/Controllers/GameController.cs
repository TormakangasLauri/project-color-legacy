using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // TODO: Add GameController related things

    public static bool paused;

    public static GameController inst;

    private void Awake() { inst = this; }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) EndLevel();
    }

    public static void StartLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

    public static void EndLevel()
    {
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