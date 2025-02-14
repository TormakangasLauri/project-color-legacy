using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PaintObjective : Objective
{
    public GameObject pointIndicator;
    public Material mat;

    private PaintController paintController;
    
    public List<Collider> paintAreaColliders = new List<Collider>();
    private GameObject paintColliders;
    private List<PaintTarget.PaintPoint> paintPoints = new List<PaintTarget.PaintPoint>();
    private List<Collider> uncheckedPaintColliders = new List<Collider>();
    
    private List<Vector3> paintAreaPoints = new List<Vector3>();
    private List<Vector3> paintAreaPointsCovered = new List<Vector3>();
    private List<GameObject> paintAreaIndicators = new List<GameObject>();

    private int paint;
    private int paintLastFrame;

    public int paintPercentage;
    public int percentageToComplete = 90;
    
    void Start()
    {
        paintController = GameObject.Find("GameController").GetComponent<PaintController>();
        paintAreaColliders.AddRange(GetComponentsInChildren<Collider>());
        paintAreaColliders.Remove(GetComponent<Collider>()); // Remove objective trigger
        paintColliders = GameObject.Find("PaintColliders");
        paintPoints = PaintTarget.paintWorldPositions;

        // Create paint check points for the paint area
        foreach (Collider col in paintAreaColliders)
        {
            Vector3 scale = col.transform.lossyScale;
            bool sphere = col is SphereCollider;
            
            for (float y = -scale.y / 2; y <= scale.y / 2; y += 0.2f)
            {
                float xLimit = sphere ? Mathf.Cos(MathF.Asin(y / (scale.y / 2))) * (scale.x / 2) : scale.x / 2; // Define x-coordinate limits for box and sphere colliders
                for (float x = -xLimit; x <= xLimit; x += 0.2f)
                {
                    RaycastHit hit;
                    Vector3 rayPoint = col.transform.position + col.transform.up * y + col.transform.right * x + col.transform.forward * col.transform.localScale.z/2;
                    Physics.Raycast(rayPoint, -col.transform.forward, out hit, col.transform.localScale.z, LayerMask.GetMask("Terrain"));
            
                    Vector3 point = hit.point;
                    paintAreaPoints.Add(point);

                    // Indicator, REMOVE AFTER TESTING
                    GameObject PI = Instantiate(pointIndicator, point, Quaternion.identity);
                    PI.hideFlags = HideFlags.HideInHierarchy;
                    paintAreaIndicators.Add(PI);
                }
            }
            
            Destroy(col.gameObject);
        }
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     foreach (Vector3 point in paintAreaPoints)
    //         Gizmos.DrawSphere(point, 0.05f);
    // }

    private void OnTriggerEnter(Collider other)
    {
        // Start objective on trigger collision
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Objective());
            GetComponent<Collider>().enabled = false;
        }
    }

    IEnumerator Objective()
    {
        active = true;
        objectives.NewObjective(this);
        Debug.Log("Paint objective started");
        yield return new WaitUntil(() => paintPercentage > percentageToComplete);
        active = false;
        completed = true;
        Debug.Log("Paint objective complete");
        objectives.RemoveObjective(this);
    }

    private void Update()
    {
        paint = paintPoints.Count;
        if (paint > paintLastFrame) CheckCollision(paint - paintLastFrame);
        paintLastFrame = paint;
    }

    void CheckCollision(int newPaint)
    {
        //foreach (GameObject paintObj in paintController.paintObjects.GetRange(paintController.paintObjects.Count-1-newPaint, newPaint)) // Go through all new paint objects
        foreach (GameObject paintObj in paintController.paintObjects)
        {
            if (paintObj.IsDestroyed() || paintObj == null) continue; // Skip non-valid paint objects

            List<Vector3> pointsInCol = new List<Vector3>();

            bool destroy = true;
            foreach (Vector3 point in paintAreaPoints) // Check whether the collider is in the paint area
            {
                Collider paintCol = paintObj.GetComponent<Collider>();
                if (paintCol.bounds.Contains(point))
                {
                    destroy = false;
                    pointsInCol.Add(point);
                }
            }
            if (destroy) Destroy(paintObj);
            else UpdatePaintArea(pointsInCol);
        }
    }

    private void UpdatePaintArea(List<Vector3> points)
    {
        foreach (Vector3 point in points)
            if (!paintAreaPointsCovered.Contains(point))
            {
                paintAreaPointsCovered.Add(point);
                
                // Indicator, REMOVE AFTER TESTING
                GameObject ind = paintAreaIndicators[paintAreaPoints.IndexOf(point)];
                ind.GetComponent<MeshRenderer>().material = mat;
            }
        paintPercentage = (int)((float)(paintAreaPointsCovered.Count)/(float)(paintAreaPoints.Count) * 100);
    }
}
