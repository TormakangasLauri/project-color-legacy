using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HangingCleaning : MonoBehaviour
{
    private Hanging hanging;

    public float cleanSpeed = 10;
    public float cleaningArea = 16;

    public Brush brush;

    [HideInInspector] public bool cleaning = false;
    [HideInInspector] public bool cleaningComplete = false;
    private bool stopCleaning = false;

    void Start()
    {
        hanging = GetComponent<Hanging>();
    }

    public void StartCleaning()
    {
        cleaning = true;
        StartCoroutine(Clean());
    }

    public void StopCleaning()
    {
        stopCleaning = true;
    }

    IEnumerator Clean()
    {
        float currentBrushSize = 0;
        yield return new WaitUntil(() =>
        {
            currentBrushSize += cleanSpeed * Time.deltaTime;
            brush.splatScale = currentBrushSize;
            
            PaintTarget.PaintRay(new Ray(transform.position, hanging.targetDirection), brush, LayerMask.GetMask("Terrain"), 5);
            
            return currentBrushSize >= cleaningArea || stopCleaning;
        });
        cleaning = false;

        if (!stopCleaning) cleaningComplete = true;
        else stopCleaning = false;

    }
}
