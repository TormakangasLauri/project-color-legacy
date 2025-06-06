using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    CanvasGroup cg;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        cg = GetComponentInChildren<CanvasGroup>();
    }

    public void Load(int level)
    {
        StartCoroutine(LoadAsync(level));
    }

    IEnumerator LoadAsync(int level)
    {
        int lastScene = SceneManager.GetActiveScene().buildIndex;
        if (lastScene != 0) // Fade to black in normal levels
        {
            float i = 0;
            yield return new WaitUntil(() =>
            {
                i += Time.unscaledDeltaTime;
                cg.alpha = i;
                return i >= 1;
            });
        }
        else cg.alpha = 1;

        PaintTarget.ClearAllPaint();
        PaintController.ClearAllPaintObjects();

        AsyncOperation operation = SceneManager.LoadSceneAsync(level);

        yield return new WaitUntil(() => { return operation.isDone; });

        yield return new WaitForSecondsRealtime(0.5f);

        if (SceneManager.GetActiveScene().buildIndex != 0) // Fade from black in normal levels
        {
            float i = 1;
            yield return new WaitUntil(() =>
            {
                i -= Time.unscaledDeltaTime;
                cg.alpha = i;
                return i <= 0;
            });
        }
        else cg.alpha = 0;
    }
}
