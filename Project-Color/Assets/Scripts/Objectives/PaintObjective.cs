using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintObjective : MonoBehaviour
{
    private Collider paintArea;
    private GameObject paintColliders;
    public List<PaintTarget.PaintPoint> paintPoints = new List<PaintTarget.PaintPoint>();
    private List<Collider> uncheckedPaintColliders = new List<Collider>();

    private int paint;
    private int paintLastFrame;
    
    void Start()
    {
        paintArea = GetComponent<Collider>();
        paintColliders = GameObject.Find("PaintColliders");
        paintPoints = PaintTarget.paintWorldPositions;
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
            BoxCollider pCol = paintColliders.AddComponent<BoxCollider>(); // Create a collider for the paint splat
            pCol.center = p.point;
            pCol.size = Vector3.one * p.scale;
            
            uncheckedPaintColliders.Add(pCol);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (uncheckedPaintColliders.Contains(other))
        {
            
        }
    }
}
