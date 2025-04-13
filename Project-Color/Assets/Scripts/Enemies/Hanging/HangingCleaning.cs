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

        // Check if paint objects should be removed
        RaycastHit hit;
        Physics.Raycast(transform.position, hanging.targetDirection, out hit, 5, LayerMask.GetMask("Terrain"));
        foreach (Transform paint in targetPaintGroup.transform.GetComponentsInChildren<Transform>())
            if (Vector3.Distance(paint.position, hit.point) < currentBrushSize/6 && paint != transform)
            {
                Destroy(paint.gameObject);
            }

        if (currentBrushSize >= cleaningArea)
        {
            PaintController.CleanPaintGroup(targetPaintGroup);
            cleaningComplete = true;
        }
    }
}
