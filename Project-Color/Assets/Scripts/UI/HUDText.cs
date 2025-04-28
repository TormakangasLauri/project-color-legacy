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
    public bool x;
    public float timeForChar;

    public static List<TextMeshProUGUI> textColumns = new List<TextMeshProUGUI>();

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

        float fullHeight = transform.parent.GetComponent<RectTransform>().rect.height;
        float fullWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        int lines = transform.childCount;
        float lineHeight = fullHeight / lines;

        int i = 0;
        foreach (Transform textLine in transform)
        {
            RectTransform rectTransform = textLine.GetComponent<RectTransform>();
            rectTransform.rect.Set(0, -i * lineHeight, fullWidth - 10, lineHeight);
            rectTransform.anchoredPosition = new Vector2(5, -i * lineHeight + lineHeight * 0.15f);
            rectTransform.sizeDelta = new Vector2(fullWidth, lineHeight);

            TextMeshProUGUI textMesh = textLine.GetComponent<TextMeshProUGUI>();
            textMesh.fontSize = lineHeight;
            textMesh.text = i.ToString();

            textColumns.Add(textMesh);

            i++;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) SetText(1, "testtest123456");
        if (Input.GetKeyDown(KeyCode.N)) StartCoroutine(SwapCharacters(textColumns[0]));
        if (Input.GetKeyDown(KeyCode.M)) ClearAllText();
    }

    public static void SetText(int line, string text)
    {
        TextMeshProUGUI textMesh = textColumns[line - 1];
        textMesh.text = text;

        inst.StartCoroutine(Text());
        IEnumerator Text()
        {
            float elapsedTime = 0;

            yield return new WaitUntil(() =>
            {
                elapsedTime += Time.unscaledDeltaTime;
                // Get a substring from textContent with the amount of characters calculated from the time it should take for one char to appear and time elapsed in the process
                textMesh.text = text[..Mathf.Clamp((int)(elapsedTime / inst.timeForChar), 0, text.Length)];
                return textMesh.text.Length == text.Length;
            });
        }
    }

    IEnumerator SwapCharacters(TextMeshProUGUI textMesh) // Not working correctly, characters don't get removed in a similar fashion as in SetText make them appear
    {
        float elapsedTime = 0;
        int waitForCharsToClear = 3;
        int lastIndex = -1;
        char[] updatedText = textMesh.text.ToCharArray();

        yield return new WaitUntil(() =>
        {
            // Get a substring from textContent with the amount of characters calculated from the time it should take for one char to appear and time elapsed in the process
            //textMesh.text = text[..Mathf.Clamp((int)(elapsedTime / inst.timeForChar), 0, text.Length)];

            for (int i = lastIndex + 1; i <= (int)(elapsedTime / timeForChar) && i < textMesh.text.Length; i++)
                updatedText[i] = ' ';
            textMesh.text = updatedText.ToString();
            lastIndex = (int)(elapsedTime / timeForChar);
            elapsedTime += Time.unscaledDeltaTime;
            return lastIndex >= textMesh.text.Length;
        });
    }

    public static void ClearAllText()
    {
        inst.StartCoroutine(Clear());
        IEnumerator Clear()
        {
            foreach (TextMeshProUGUI textMesh in textColumns)
            {
                float elapsedTime = 0;
                string textContent = "";
                foreach (char c in textMesh.text)
                    textContent += " ";

                yield return new WaitUntil(() =>
                {
                    elapsedTime += Time.unscaledDeltaTime;
                    // Get a substring from textContent with the amount of characters calculated from the time it should take for one char to appear and time elapsed in the process
                    textMesh.text = textContent[..Mathf.Clamp((int)(elapsedTime / inst.timeForChar), 0, textContent.Length)];
                    return textMesh.text.Length == textContent.Length;
                });

                textMesh.text = "";
            }
        }
    }
}
