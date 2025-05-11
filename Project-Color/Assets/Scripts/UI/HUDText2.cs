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

public class HUDText2 : MonoBehaviour
{
    public float fontSize = 1;
    public float yOffset = 0;
    public float xOffset = 0;
    public float heightOffset = 0;
    [Tooltip("The amount of characters to be changed in a second, represents text writing speed")]
    public float changeCharsPerSec = 100;
    [Tooltip("Wait for this many characters to be cleared before starting to write new text on the same line")]
    public int waitForCharactersOnSwap = 6;
    [Tooltip("The wait before moving on to the next line, negative values make it wait for the whole last line to finish")]
    public float waitBeforeNextLine = 0.2f;
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
            if (line >= 0) this.line = line;
            else this.line = textLines.Count + line; // If line is negative, get the writeable line from the bottom (-1 = last line, -2 = second last)
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

    public static List<TextLine> textLines = new List<TextLine>();
    public static List<string> textContents = new List<string>();
    public static List<string> savedText = new List<string>();

    static HUDText2 inst;

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
        textContents.Clear();
        savedText.Clear();

        float fullHeight = 1080;
        float fullWidth = 1920;
        if (transform.parent && transform.parent.GetComponent<RectTransform>())
        {
            fullHeight = transform.parent.GetComponent<RectTransform>().rect.height;
            fullWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        }
        int lines = transform.childCount;
        float height = fullHeight + heightOffset;
        float startHeight = (height - fullHeight) / 2;
        float lineHeight = height / lines;

        int i = 0;
        foreach (Transform textLineTransform in transform)
        {
            RectTransform rectTransform = textLineTransform.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(xOffset, startHeight - i * lineHeight + yOffset);
            rectTransform.sizeDelta = new Vector2(fullWidth, lineHeight);

            TextLine textLine = textLineTransform.GetComponent<TextLine>();
            textLine.line = i;

            TextMeshProUGUI textMesh = textLineTransform.GetComponent<TextMeshProUGUI>();
            textMesh.fontSize = lineHeight * fontSize;
            textMesh.text = "";
            if (i < 10)
                for (int j = 0; j < 10; j++)
                    textMesh.text += i.ToString();
            else for (int j = 0; j < 5; j++)
                    textMesh.text += i.ToString();

            textLines.Add(textLine);
            textContents.Add(textMesh.text);
            savedText.Add("");

            i++;
        }
    }

    private void Update()
    {
        bufferTimer += Time.unscaledDeltaTime;

        if (/*bufferTimer > bufferWaitTime &&*/ bufferList.Count > 0)
        {
            bufferTimer = 0;
            if (bufferList.Count > maxBufferSize) bufferList = bufferList.GetRange(0, maxBufferSize); // Trim buffer list
            TextSettings ts = bufferList[0];
            if (ts.single) // Single line
            {
                UpdateTextContents(ts.line, ts.text, ts.replace);
                inst.StartCoroutine(ReplaceAllLines(ts.line, ts.text, ts.replace, ts.update));
                bufferList.RemoveAt(0);
            }
            else // Multiple lines
            {
                UpdateTextContents(ts.lines, ts.texts, ts.replace, ts.fill);
                inst.StartCoroutine(ReplaceAllLines(ts.lines, ts.texts, ts.replace, ts.update, ts.fill));
                bufferList.RemoveAt(0);
            }
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
    public static void MoveTextDown(int line, int moveSpaces = 1) { bufferList.Add(new TextSettings(new[] { line, line + moveSpaces }, new[] { "", textContents[line] })); }
    // Move multiple lines down
    public static void MoveTextDown(int[] lines, int moveSpaces = 1)
    {
        List<string> texts = new List<string>();
        List<int> newLines = new List<int>();
        for (int i = lines.Length - 1; i >= 0; i--)
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
    public static void MoveTextUp(int line, int moveSpaces = 1) { bufferList.Add(new TextSettings(new[] { line - moveSpaces, line }, new[] { textContents[line], "" })); }
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
        for (int i = 0; i < textLines.Count; i++)
        {
            if (line == i)
            {
                yield return textLines[i].ReplaceAndWait(text);
                if (update == HUDTextUpdate.Single || update == HUDTextUpdate.Above) break; // Break out of the loop if update method is not all
            }
            else if (update == HUDTextUpdate.All || update == HUDTextUpdate.Above) // Keep replacing if update method is all or above
            {
                if (replace == HUDTextReplace.Keep) yield return textLines[i].ReplaceAndWait(); // Replace with the same text or clear depending on settings
                else if (replace == HUDTextReplace.Clear) yield return textLines[i].ReplaceAndWait("");
            }
        }
    }

    // Replace all for setting multiple lines
    IEnumerator ReplaceAllLines(int[] lines, string[] texts, HUDTextReplace replace = default, HUDTextUpdate update = default, HUDTextFill fill = default)
    {
        int textIndex = 0;
        bool fillSet = false;
        string fillText = null;
        for (int i = 0; i < textLines.Count; i++)
        {
            if (lines.Contains(i))
            {
                if (!fillSet) fillText = texts[textIndex]; // If fill is not on, set a new fill text

                yield return textLines[i].ReplaceAndWait(fillText); // Use the existing fill text for replacing on set indexes
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
                if (fillSet) yield return textLines[i].ReplaceAndWait(fillText); // If fill is on, use fill text set above
                else if (update == HUDTextUpdate.All || update == HUDTextUpdate.Above) // Keep replacing if update method is all or above
                {
                    if (replace == HUDTextReplace.Keep) yield return textLines[i].ReplaceAndWait(); // Replace with the same text or clear depending on settings
                    else if (replace == HUDTextReplace.Clear) yield return textLines[i].ReplaceAndWait("");
                }
            }
        }
    }

    void UpdateTextContents(int line, string text, HUDTextReplace replace = default)
    {
        for (int i = 0; i < textLines.Count; i++)
        {
            if (line == i) textLines[i].textContent = text; // Update the specified line
            else if (replace == HUDTextReplace.Clear) textLines[i].textContent = ""; // If replace method is clear, update other lines to be empty
        }
    }

    void UpdateTextContents(int[] lines, string[] texts, HUDTextReplace replace = default, HUDTextFill fill = default)
    {
        int textIndex = 0;
        bool fillSet = false;
        string fillText = null;
        for (int i = 0; i < textLines.Count; i++)
        {
            if (lines.Contains(i))
            {
                if (!fillSet) fillText = texts[textIndex]; // If fill is not on, set a new fill text

                textLines[i].textContent = fillText; // Use the existing fill text to update specified indexes

                if (fillSet || fill == HUDTextFill.Single) // If fill is on or fill is not used (default), turn it off and increment fill index to use the next text in the parameter array
                {
                    textIndex++;
                    fillSet = false;
                }
                else fillSet = true; // Otherwise turn fill on
            }
            else if (fillSet) textLines[i].textContent = fillText; // If fill is on, use fillText set above
            else if (replace == HUDTextReplace.Clear) textLines[i].textContent = ""; // If replace method is clear, update other lines to be empty
        }
    }

    public static void ClearAllText()
    {
        inst.StartCoroutine(Clear());
        IEnumerator Clear()
        {
            for (int i = 0; i < textLines.Count; i++)
                yield return textLines[i].ClearLine();
        }
    }

    public static void SaveLine(int line) { textLines[line].SaveLine(); }

    public static void RetrieveLine(int line) { SetText(line, savedText[line], HUDTextUpdate.Single); }

    public static void SaveAllText()
    {
        for (int i = 0; i < textLines.Count; i++)
            SaveLine(i);
    }

    public static void RetrieveAllText()
    {
        int[] lines = new int[textLines.Count];
        string[] savedTexts = new string[textLines.Count];
        for (int i = 0; i < textLines.Count; i++)
        {
            lines[i] = i;
            savedTexts[i] = textLines[i].savedText;
        }
        SetText(lines, savedTexts, HUDTextReplace.Keep);
    }

}
