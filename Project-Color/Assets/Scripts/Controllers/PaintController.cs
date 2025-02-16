using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintController : MonoBehaviour
{
    private List<PaintTarget.PaintPoint> paintPoints = new List<PaintTarget.PaintPoint>();

    public List<GameObject> paintObjects = new List<GameObject>();

    public int paint;
    public int paintLastFrame;

    float t = 0;

    void Start()
    {
        paintPoints = PaintTarget.paintWorldPositions;
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t > 0.2)
        {
            t = 0;
            paint = paintPoints.Count;
            if (paint > paintLastFrame) CreatePaintObjects(); // Check for new paint
            paintLastFrame = paint;
        }
    }

    public void CreatePaintObjects()
    {
        int newPaint = paint - paintLastFrame;
        foreach (PaintTarget.PaintPoint p in paintPoints.GetRange(paintPoints.Count - newPaint, newPaint))
        {
            GameObject paintObj = new GameObject("PaintObj"); // Paint splat object
            paintObj.transform.position = p.point;
            paintObj.transform.rotation = Quaternion.LookRotation(p.normal);
            paintObj.hideFlags = HideFlags.HideInHierarchy;

            BoxCollider paintCol = paintObj.AddComponent<BoxCollider>(); // Create a collider for the paint splat
            paintCol.isTrigger = true;
            paintCol.size = new Vector3(p.scale, p.scale, 0.2f);

            paintObjects.Add(paintObj);

            paintObj.SetActive(false); // Optimization shit
        }
    }
}
