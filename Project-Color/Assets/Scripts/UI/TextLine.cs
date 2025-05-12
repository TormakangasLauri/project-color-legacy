using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextLine : MonoBehaviour
{
    TextMeshProUGUI textMesh;
    HUDText hudtext;

    public int line;
    public string textContent;
    public string savedText = "";

    private float timeSinceSet;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        hudtext = GetComponentInParent<HUDText>();
    }

    private void Update()
    {
        timeSinceSet += Time.unscaledDeltaTime;
    }

    private void LateUpdate()
    {
        if (HUDText.textContents[line] != textContent) HUDText.textContents[line] = textContent;
    }

    public IEnumerator ReplaceAndWait(string text = null) // Replace text and wait for the replace or a set amount of time
    {
        if (hudtext.waitBeforeNextLine < 0)
        {
            if (timeSinceSet < 1) ClearImmediate();
            timeSinceSet = 0;
            yield return ReplaceLine(text);
        }
        else
        {
            if (timeSinceSet < 1 / hudtext.changeCharsPerSec * hudtext.waitForCharactersOnSwap) ClearImmediate();
            timeSinceSet = 0;
            StartCoroutine(ReplaceLine(text));
            yield return new WaitForSecondsRealtime(hudtext.waitBeforeNextLine);
        }
    }

    IEnumerator ReplaceLine(string text = null) // Clear text and set new after a wait
    {
        if (text == null) text = textContent;
        StartCoroutine(ClearLine());
        yield return new WaitForSecondsRealtime((1 / hudtext.changeCharsPerSec) * hudtext.waitForCharactersOnSwap);
        StartCoroutine(SetLine(text));
    }

    IEnumerator SetLine(string text) // Write new text
    {
        float elapsedTime = 0;
        int lastIndex = -1;

        yield return new WaitUntil(() =>
        {
            string oldText = textMesh.text;
            int newIndex = (int)(elapsedTime / (1 / hudtext.changeCharsPerSec));
            for (int i = lastIndex + 1; i <= newIndex && i <= text.Length; i++)
            {
                textMesh.text = text.Substring(0, i);
                if (i < oldText.Length) textMesh.text += oldText.Substring(i);
            }
            lastIndex = newIndex;
            elapsedTime += Time.unscaledDeltaTime;
            return lastIndex >= text.Length;
        });
        textMesh.text = textMesh.text.TrimEnd(' '); // Remove unwanted emtpy spaces from the end
    }

    public IEnumerator ClearLine() // Clear text (replace characters with an empty space)
    {
        float elapsedTime = 0;
        int lastIndex = -1;

        yield return new WaitUntil(() =>
        {
            char[] updatedText = textMesh.text.ToCharArray();
            int newIndex = (int)(elapsedTime / (1 / hudtext.changeCharsPerSec));
            for (int i = lastIndex + 1; i <= newIndex; i++) // Go through all new characters in the existing text
                if (i < updatedText.Length) updatedText[i] = ' ';
            textMesh.text = updatedText.ArrayToString();
            lastIndex = newIndex;
            elapsedTime += Time.unscaledDeltaTime;
            return lastIndex >= textMesh.text.Length;
        });
    }

    void ClearImmediate() // Stop everything and clear the text
    {
        StopAllCoroutines();
        textMesh.text = "";
    }

    public void SaveLine() { savedText = textContent; }
}
