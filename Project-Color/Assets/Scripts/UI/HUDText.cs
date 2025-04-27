using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDText : MonoBehaviour
{
    public GameObject textLine;

    public int lines = 7;
    public float spacing = 0;

    private List<GameObject> lineList = new List<GameObject>();

    private void OnValidate()
    {
        
    }

    private void Start()
    {

        ////foreach (Transform child in GetComponent<RectTransform>()) // Add all existing children
        ////    DestroyImmediate(child.gameObject);
        ////int linesBefore = lineList.Count;

        //for (int i = 0; i < lines; i++)
        //{
        //    GameObject line = Instantiate(textLine);

        //    line.transform.parent = transform;

        //    RectTransform rectTransform = line.GetComponent<RectTransform>();
        //    rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
        //    rectTransform.localScale = Vector3.one;
        //    rectTransform.rect.Set(5, -i * lineHeight, fullWidth, lineHeight);
        //    rectTransform.anchoredPosition = new Vector3(5, -i * lineHeight, 0);

        //    TextMeshProUGUI textMesh = line.GetComponent<TextMeshProUGUI>();
        //    textMesh.fontSize = lineHeight;
        //    textMesh.text = $"Line {i + 1}";

        //    line.name = $"Line{i + 1}";

        //    lineList.Add(line);
        //}

        float fullHeight = transform.parent.GetComponent<RectTransform>().rect.height;
        float fullWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        int lines = transform.childCount;
        float lineHeight = fullHeight / lines;

        int i = 0;
        foreach (Transform textLine in transform)
        {
            RectTransform rectTransform = textLine.GetComponent<RectTransform>();
            rectTransform.rect.Set(5, -i*lineHeight, fullWidth-10, lineHeight);

            TextMeshProUGUI textMesh = textLine.GetComponent<TextMeshProUGUI>();
            textMesh.fontSize = lineHeight;
            textMesh.text = $"Line {i+1}";

            i++;
        }
    }
}
