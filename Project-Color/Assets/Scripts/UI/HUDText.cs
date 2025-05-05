using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

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
    public static List<string> savedText = new List<string>();

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
        savedText.Clear();

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
            textContents.Add(textMesh.text);
            savedText.Add("");

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
            if (ts.single) // Single line
            {
                UpdateTextContents(ts.line, ts.text, ts.replace);
                inst.StartCoroutine(ReplaceAllLines(ts.line, ts.text, ts.replace, ts.update));
            }
            else // Multiple lines
            {
                UpdateTextContents(ts.lines, ts.texts, ts.replace, ts.fill);
                inst.StartCoroutine(ReplaceAllLines(ts.lines, ts.texts, ts.replace, ts.update, ts.fill));
            }
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

    // Move single line down
    public static void MoveTextDown(int line, int moveSpaces = 1) { bufferList.Add(new TextSettings(new[] {line,line + moveSpaces}, new[] {"",textContents[line]})); }
    // Move multiple lines down
    public static void MoveTextDown(int[] lines, int moveSpaces = 1)
    {
        List<string> texts = new List<string>();
        List<int> newLines = new List<int>();
        for (int i = lines.Length-1; i >= 0; i--)
        {
            int line = lines[i];
            texts.Insert(0, textContents[line]);
            newLines.Insert(0, line + moveSpaces);
            if (!lines.Contains(line - moveSpaces))
            {
                texts.Insert(0, ""); // If the line above is not going to be rewritten, make it empty
                newLines.Insert(0, line);
            }
        }
        bufferList.Add(new TextSettings(newLines.ToArray(), texts.ToArray()));
    }
    // Move single line up
    public static void MoveTextUp(int line, int moveSpaces = 1) { bufferList.Add(new TextSettings(new[] {line - moveSpaces, line}, new[] {textContents[line], ""})); }
    // Move multiple lines up
    public static void MoveTextUp(int[] lines, int moveSpaces = 1)
    {
        List<string> texts = new List<string>();
        List<int> newLines = new List<int>();
        for (int i = 0; i < lines.Length; i++)
        {
            int line = lines[i];
            texts.Add(textContents[line]);
            newLines.Add(line - moveSpaces);
            if (!lines.Contains(line + moveSpaces))
            {
                texts.Add(""); // If the line below is not going to be rewritten, make it empty
                newLines.Add(line);
            }
        }
        bufferList.Add(new TextSettings(newLines.ToArray(), texts.ToArray()));
    }

    // Replace all for setting a single line
    IEnumerator ReplaceAllLines(int line, string text, HUDTextReplace replace = default, HUDTextUpdate update = default)
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
    IEnumerator ReplaceAllLines(int[] lines, string[] texts, HUDTextReplace replace = default, HUDTextUpdate update = default, HUDTextFill fill = default)
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

    void UpdateTextContents(int line, string text, HUDTextReplace replace = default)
    {
        for (int i = 0; i < textColumns.Count; i++)
        {
            if (line == i) textContents[i] = text; // Update the specified line
            else if (replace == HUDTextReplace.Clear) textContents[i] = ""; // If replace method is clear, update other lines to be empty
        }
    }
    
    void UpdateTextContents(int[] lines, string[] texts, HUDTextReplace replace = default, HUDTextFill fill = default)
    {
        int textIndex = 0;
        bool fillSet = false;
        string fillText = null;
        for (int i = 0; i < textColumns.Count; i++)
        {
            if (lines.Contains(i))
            {
                if (!fillSet) fillText = texts[textIndex]; // If fill is not on, set a new fill text

                textContents[i] = fillText; // Use the existing fill text to update specified indexes

                if (fillSet || fill == HUDTextFill.Single) // If fill is on or fill is not used (default), turn it off and increment fill index to use the next text in the parameter array
                {
                    textIndex++;
                    fillSet = false;
                }
                else fillSet = true; // Otherwise turn fill on
            }
            else if (fillSet) textContents[i] = fillText; // If fill is on, use fillText set above
            else if (replace == HUDTextReplace.Clear) textContents[i] = ""; // If replace method is clear, update other lines to be empty
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

    public static void SaveLine(int line) { savedText[line] = textContents[line]; }

    public static void RetrieveLine(int line) { SetText(line, savedText[line]); }

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
        SetText(lines, savedText.ToArray(), HUDTextReplace.Keep);
    }

}
