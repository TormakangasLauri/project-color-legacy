using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            GameObject paintObj = new GameObject("PaintObj"); // Paint splat object
            paintObj.transform.position = p.point;
            paintObj.transform.rotation = Quaternion.LookRotation(p.normal);
            
            // paintObj.hideFlags = HideFlags.HideInHierarchy;

            BoxCollider paintCol = paintObj.AddComponent<BoxCollider>(); // Create a collider for the paint splat
            paintCol.isTrigger = true;
            paintCol.size = new Vector3(p.scale, p.scale, 0.2f);
            
            uncheckedPaintColliders.Add(paintCol);
            
            StartCoroutine(DestroyCollider(paintObj)); // Destroy paintObj if it's not in the paint area
        }
        IEnumerator DestroyCollider(GameObject paint)
        {
            float t = 1f;
            bool destroy = true;
            yield return new WaitUntil(() =>
            {
                if (paint.name == "PaintOnObjective") destroy = false;
                t -= Time.deltaTime;
                return t <= 0;
            });
            if (destroy) Destroy(paint); // Destroy object if it's not in the paint area
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (uncheckedPaintColliders.Contains(other))
        {
            other.gameObject.name = "PaintOnObjective";
        }
    }
}
