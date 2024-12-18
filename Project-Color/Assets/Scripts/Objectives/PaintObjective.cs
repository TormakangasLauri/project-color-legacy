using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PaintObjective : Objective
{
    private Collider paintArea;
    private GameObject paintColliders;
    public List<PaintTarget.PaintPoint> paintPoints = new List<PaintTarget.PaintPoint>();
    private List<Collider> uncheckedPaintColliders = new List<Collider>();
    
    private List<Vector3> paintAreaPoints = new List<Vector3>();
    private List<Vector3> paintAreaPointsCovered = new List<Vector3>();

    private int paint;
    private int paintLastFrame;

    public int paintPercentage;
    
    void Start()
    {
        paintArea = GetComponent<Collider>();
        paintColliders = GameObject.Find("PaintColliders");
        paintPoints = PaintTarget.paintWorldPositions;

        // Create paint check points for the paint area
        Vector3 scale = transform.localScale;
        for (float y = -scale.y/2; y <= scale.y/2; y += 0.2f)
            for (float x = -scale.x/2; x <= scale.x/2; x += 0.2f)
            {
                Vector3 point = transform.position + transform.up * y + transform.right * x;
                paintAreaPoints.Add(point);
            }
    }

    private void Update()
    {
        paint = paintPoints.Count;
        if (paint > paintLastFrame) CheckCollision();
        paintLastFrame = paint;
    }

    void CheckCollision()
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
            
            uncheckedPaintColliders.Add(paintCol);

            List<Vector3> pointsInCol = new List<Vector3>();
            bool destroy = true;
            foreach (Vector3 point in paintAreaPoints) // Check wheter the collider is in the paint area
                if (paintCol.bounds.Contains(point))
                {
                    destroy = false;
                    pointsInCol.Add(point);
                }
            if (destroy) Destroy(paintObj);
            else UpdatePaintArea(pointsInCol);
        }
    }

    private void UpdatePaintArea(List<Vector3> points)
    {
        foreach (Vector3 point in points)
            if (!paintAreaPointsCovered.Contains(point))
                paintAreaPointsCovered.Add(point);
        paintPercentage = (int)((float)(paintAreaPointsCovered.Count)/(float)(paintAreaPoints.Count) * 100);
    }
}
