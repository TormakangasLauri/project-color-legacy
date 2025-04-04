using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HangingCleaning : MonoBehaviour
{
    private Hanging hanging;

    public float cleanSpeed = 10;
    public float cleaningArea = 16;

    private float currentBrushSize = 0;

    public Brush brush;

    [HideInInspector] public bool cleaning = false;
    [HideInInspector] public bool cleaningComplete = false;

    void Start()
    {
        hanging = GetComponent<Hanging>();
    }

    public void StopCleaning()
    {
        cleaning = false;
        currentBrushSize = 0;
    }

    public void Clean(GameObject targetPaintGroup)
    {
        cleaning = true;
        currentBrushSize += cleanSpeed * Time.deltaTime / 10;
        brush.splatScale = currentBrushSize;

        PaintTarget.PaintRay(new Ray(transform.position, hanging.targetDirection), brush, LayerMask.GetMask("Terrain"), 5);

        if (currentBrushSize >= cleaningArea)
        {
            cleaningComplete = true;
        }
    }
}
