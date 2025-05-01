using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public enum HUDTextReplace { Keep, Clear }
public enum HUDTextUpdate { All, Above, Single }
public enum HUDTextFill { Single, Fill }

public class HUDText : MonoBehaviour
{
    public float fontSize = 1;
    public float yOffset = 0;
    public float timeForChar;
    public int waitForCharactersOnSwap = 4;
    public float bufferWaitTime = 0.2f;
    public int maxBufferSize = 100;
    private float bufferTimer = 0;

    class TextSettings
    {
        public int line;
        public int[] lines;
        public string text;
        public string[] texts;
        public HUDTextReplace replace;
        public HUDTextUpdate update;
        public HUDTextFill fill;

        public bool single;

        public TextSettings(int line, string text, HUDTextReplace replace = default, HUDTextUpdate update = default, HUDTextFill fill = default) // Single
        {
            this.line = line;
            this.text = text;
            this.replace = replace;
            this.update = update;
            this.fill = fill;
            single = true;
        }
        public TextSettings(int[] lines, string[] texts, HUDTextReplace replace = default, HUDTextUpdate update = default, HUDTextFill fill = default) // Multiple
        {
            this.lines = lines;
            this.texts = texts;
            this.replace = replace;
            this.update = update;
            this.fill = fill;
            single = false;
        }
    }
    private static List<TextSettings> bufferList = new List<TextSettings>();

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
        bufferTimer += Time.unscaledDeltaTime;
        if (bufferTimer > bufferWaitTime && bufferList.Count > 0)
        {
            bufferTimer = 0;
            if (bufferList.Count > maxBufferSize) bufferList = bufferList.GetRange(0, maxBufferSize); // Trim buffer list
            TextSettings ts = bufferList[0];
            if (ts.single) inst.StartCoroutine(ReplaceAllLines(ts.line, ts.text, ts.replace, ts.update)); // Single
            else inst.StartCoroutine(ReplaceAllLines(ts.lines, ts.texts, ts.replace, ts.update, ts.fill)); // Multiple
            bufferList.RemoveAt(0);
        }
    }

    // SetText overloads:
    // Only line and text
    public static void SetText(int line, string text) { bufferList.Add(new TextSettings(line, text)); }
    // Replacement method
    public static void SetText(int line, string text, HUDTextReplace replace) { bufferList.Add(new TextSettings(line, text, replace)); }
    // Update method
    public static void SetText(int line, string text, HUDTextUpdate update) { bufferList.Add(new TextSettings(line, text, default, update)); }
    // Replacement and update methods
    public static void SetText(int line, string text, HUDTextReplace replace, HUDTextUpdate update) { bufferList.Add(new TextSettings(line, text, replace, update)); }

    // SetText Multiple overloads:
    // Only lines and texts
    public static void SetText(int[] lines, string[] texts) { bufferList.Add(new TextSettings(lines, texts)); }
    // Replacement method
    public static void SetText(int[] lines, string[] texts, HUDTextReplace replace) { bufferList.Add(new TextSettings(lines, texts, replace)); }
    // Update method
    public static void SetText(int[] lines, string[] texts, HUDTextUpdate update) { bufferList.Add(new TextSettings(lines, texts, default, update)); }
    // Fill method
    public static void SetText(int[] lines, string[] texts, HUDTextFill fill) { bufferList.Add(new TextSettings(lines, texts, default, default, fill)); }
    // Replacement and update methods
    public static void SetText(int[] lines, string[] texts, HUDTextReplace replace, HUDTextUpdate update) { bufferList.Add(new TextSettings(lines, texts, replace, update)); }
    // Replacement and fill methods
    public static void SetText(int[] lines, string[] texts, HUDTextReplace replace, HUDTextFill fill) { bufferList.Add(new TextSettings(lines, texts, replace, default, fill)); }
    // Update and fill methods
    public static void SetText(int[] lines, string[] texts, HUDTextUpdate update, HUDTextFill fill) { bufferList.Add(new TextSettings(lines, texts, default, update, fill)); }
    // All available methods (replace, update and fill)
    public static void SetText(int[] lines, string[] texts, HUDTextReplace replace, HUDTextUpdate update, HUDTextFill fill) { bufferList.Add(new TextSettings(lines, texts, replace, update, fill)); }

    // Replace all for setting a single line
    IEnumerator ReplaceAllLines(int line, string text, HUDTextReplace replace = default, HUDTextUpdate update = HUDTextUpdate.All)
    {
        for (int i = 0; i < textColumns.Count; i++)
        {
            if (line == i)
            {
                yield return ReplaceLine(i, text);
                if (update == HUDTextUpdate.Single || update == HUDTextUpdate.Above) break; // Break out of the loop if update method is not all
            }
            else if (update == HUDTextUpdate.All || update == HUDTextUpdate.Above) // Keep replacing if update method is all or above
            {
                if (replace == HUDTextReplace.Keep) yield return ReplaceLine(i); // Replace with the same text or clear depending on settings
                else if (replace == HUDTextReplace.Clear) yield return ReplaceLine(i, "");
            }
        }
    }

    // Replace all for setting multiple lines
    IEnumerator ReplaceAllLines(int[] lines, string[] texts, HUDTextReplace replace = HUDTextReplace.Keep, HUDTextUpdate update = HUDTextUpdate.All, HUDTextFill fill = HUDTextFill.Single)
    {
        int textIndex = 0;
        bool fillSet = false;
        string fillText = null;
        for (int i = 0; i < textColumns.Count; i++)
        {
            if (lines.Contains(i))
            {
                if (!fillSet) fillText = texts[textIndex]; // If fill is not on, set a new fill text

                yield return ReplaceLine(i, fillText); // Use the existing fill text for replacing on set indexes
                if ((update == HUDTextUpdate.Single || update == HUDTextUpdate.Above) && lines.Length == i + 1 && i % 2 == 1) break; // If update method is not all and this is the last item in lines, break

                if (fillSet || fill == HUDTextFill.Single) // If fill is on or fill is not used (default), turn it off and increment fill index to use the next text in the parameter array
                {
                    textIndex++;
                    fillSet = false;
                }
                else fillSet = true; // Otherwise turn fill on
            }
            else
            {
                if (fillSet) yield return ReplaceLine(i, fillText); // If fill is on, use fill text set above
                else if (update == HUDTextUpdate.All || update == HUDTextUpdate.Above) // Keep replacing if update method is all or above
                {
                    if (replace == HUDTextReplace.Keep) yield return ReplaceLine(i); // Replace with the same text or clear depending on settings
                    else if (replace == HUDTextReplace.Clear) yield return ReplaceLine(i, "");
                }
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

    public static void RetrieveLine(int line) { SetText(line, textContents[line]); }

    public static void SaveAllText()
    {
        for (int i = 0; i < textColumns.Count; i++)
            SaveLine(i);
    }

    public static void RetrieveAllText()
    {
        int[] lines = new int[textColumns.Count];
        for (int i = 0; i < textColumns.Count; i++)
            lines[i] = i;
        SetText(lines, textContents.ToArray(), HUDTextReplace.Keep);
    }

}
