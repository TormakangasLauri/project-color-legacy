using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public enum HUDTextReplaceMethod
{
    Keep,
    Clear
}

public enum HUDTextFillMethod
{
    Single,
    Fill
}

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
            textMesh.text = "";
            if (i < 10)
                for (int j = 0; j < 10; j++)
                    textMesh.text += i.ToString();
            else for (int j = 0; j < 5; j++)
                    textMesh.text += i.ToString();

                textColumns.Add(textMesh);
            textContents.Add("");

            i++;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) SetText(0, "000000000");
    }

    public static void SetText(int line, string text, HUDTextReplaceMethod replaceMethod = HUDTextReplaceMethod.Keep)
    {
        inst.StartCoroutine(inst.ReplaceAllLines(line, text, replaceMethod));
    }

    public static void SetText(int[] lines, string[] texts, HUDTextReplaceMethod replaceMethod = HUDTextReplaceMethod.Keep, HUDTextFillMethod fill = HUDTextFillMethod.Single)
    {
        inst.StartCoroutine(inst.ReplaceAllLines(lines, texts, replaceMethod, fill));
    }

    IEnumerator ReplaceAllLines(int line, string text, HUDTextReplaceMethod replaceMethod)
    {
        for (int i = 0; i < textColumns.Count; i++)
        {
            if (line == i) yield return ReplaceLine(i, text);
            else if (replaceMethod == HUDTextReplaceMethod.Keep) yield return ReplaceLine(i);
            else if (replaceMethod == HUDTextReplaceMethod.Clear) yield return ReplaceLine(i, "");
        }
    }

    IEnumerator ReplaceAllLines(int[] lines, string[] texts, HUDTextReplaceMethod replaceMethod = HUDTextReplaceMethod.Keep, HUDTextFillMethod fillMethod = HUDTextFillMethod.Single)
    {
        int textIndex = 0;
        bool fill = false;
        string fillText = null;
        for (int i = 0; i < textColumns.Count; i++)
        {
            if (lines.Contains(i))
            {
                if (!fill) fillText = texts[textIndex]; // If fill is not on, set a new fill text

                yield return ReplaceLine(i, fillText); // Use the existing fill text for replacing on set indexes

                if (fill || fillMethod == HUDTextFillMethod.Single) // If fill is on or fill is not used (default), turn it off and increment fill index to use the next text in the parameter array
                {
                    textIndex++;
                    fill = false;
                }
                else fill = true; // Otherwise turn fill on
            }
            else
            {
                if (fill) yield return ReplaceLine(i, fillText); // If fill is on, use fill text set above
                else if (replaceMethod == HUDTextReplaceMethod.Keep) yield return ReplaceLine(i); // Otherwise replace with the same text or clear depending on settings
                else if (replaceMethod == HUDTextReplaceMethod.Clear) yield return ReplaceLine(i, "");
            }
        }
    }

    IEnumerator ReplaceLine(int line, string text = null)
    {
        if (text == null) text = textColumns[line].text;
        StartCoroutine(ClearLine(line));
        yield return new WaitForSecondsRealtime(timeForChar * waitForCharactersOnSwap);
        StartCoroutine(SetLine(line, text));
    }

    IEnumerator ClearLine(int line)
    {
        TextMeshProUGUI textMesh = textColumns[line];
        float elapsedTime = 0;
        int lastIndex = -1;

        yield return new WaitUntil(() =>
        {
            char[] updatedText = textMesh.text.ToCharArray();
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

        yield return new WaitUntil(() =>
        {
            string oldText = textMesh.text;
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
        SetText(new[]{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14}, textContents.ToArray(), HUDTextReplaceMethod.Keep);
    }

}
