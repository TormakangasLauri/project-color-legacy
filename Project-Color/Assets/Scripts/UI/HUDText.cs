using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class HUDText : MonoBehaviour
{
    public float fontSize = 1;
    public float yOffset = 0;
    public float timeForChar;
    public int waitForCharactersOnSwap = 4;

    public static List<TextMeshProUGUI> textColumns = new List<TextMeshProUGUI>();
    public static List<string> textContents = new List<string>();

    static HUDText inst;

    private void OnValidate()
    {
        UpdateTextContainer();
        inst = this;
    }

    private void Awake()
    {
        UpdateTextContainer();
    }

    private void Start()
    {
        UpdateTextContainer();
    }

    void UpdateTextContainer()
    {
        textColumns.Clear();
        textContents.Clear();

        float fullHeight = transform.parent.GetComponent<RectTransform>().rect.height;
        float fullWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        int lines = transform.childCount;
        float lineHeight = fullHeight / lines;

        int i = 0;
        foreach (Transform textLine in transform)
        {
            RectTransform rectTransform = textLine.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(5, -i * lineHeight + yOffset);
            rectTransform.sizeDelta = new Vector2(fullWidth, lineHeight);

            TextMeshProUGUI textMesh = textLine.GetComponent<TextMeshProUGUI>();
            textMesh.fontSize = lineHeight * fontSize;
            textMesh.text = i.ToString();

            textColumns.Add(textMesh);
            textContents.Add("");

            i++;
        }
    }

    public static void SetText(int line, string text)
    {
        inst.StartCoroutine(inst.ReplaceAllLines(line, text));
    }

    IEnumerator ReplaceAllLines(int line, string text)
    {
        for (int i = 0; i < textColumns.Count; i++)
        {
            if (line == i) yield return ReplaceLine(i, text);
            else yield return ReplaceLine(i, textColumns[i].text);
        }
    }

    IEnumerator ReplaceLine(int line, string text)
    {
        StartCoroutine(ClearLine(line));
        yield return new WaitForSecondsRealtime(timeForChar * waitForCharactersOnSwap);
        StartCoroutine(SetLine(line, text));
    }

    IEnumerator ClearLine(int line)
    {
        TextMeshProUGUI textMesh = textColumns[line];
        float elapsedTime = 0;
        int lastIndex = -1;
        char[] updatedText = textMesh.text.ToCharArray();

        yield return new WaitUntil(() =>
        {
            int newIndex = (int)(elapsedTime / timeForChar);
            for (int i = lastIndex + 1; i <= newIndex; i++) // Go through all new characters in the existing text
                if (i < updatedText.Length) updatedText[i] = ' ';
            textMesh.text = updatedText.ArrayToString();
            lastIndex = newIndex;
            elapsedTime += Time.unscaledDeltaTime;
            return lastIndex >= textMesh.text.Length;
        });
    }

    IEnumerator SetLine(int line, string text)
    {
        TextMeshProUGUI textMesh = textColumns[line];
        float elapsedTime = 0;
        int lastIndex = -1;
        string oldText = textMesh.text;

        yield return new WaitUntil(() =>
        {
            int newIndex = (int)(elapsedTime / timeForChar);
            for (int i = lastIndex + 1; i <= newIndex && i <= text.Length; i++)
            {
                textMesh.text = text.Substring(0, i);
                if (i < oldText.Length) textMesh.text += oldText.Substring(i);
            }
            lastIndex = newIndex;
            elapsedTime += Time.unscaledDeltaTime;
            return lastIndex >= text.Length;
        });
    }

    public static void ClearAllText()
    {
        inst.StartCoroutine(Clear());
        IEnumerator Clear()
        {
            for (int i = 0; i < textColumns.Count; i++)
                yield return inst.ClearLine(i);
        }
    }

    public static void SaveLine(int line) { textContents[line] = textColumns[line].text; }

    public static void RetreiveLine(int line) { SetText(line, textContents[line]); }

    public static void SaveAllText()
    {
        for (int i = 0; i < textColumns.Count; i++)
            SaveLine(i);
    }

    public static void RetreiveAllText()
    {
        inst.StartCoroutine(Retreive());
        IEnumerator Retreive()
        {
            for (int i = 0; i < textColumns.Count; i++)
            {
                yield return inst.ReplaceLine(i, textContents[i]);
            }
        }
    }

}
