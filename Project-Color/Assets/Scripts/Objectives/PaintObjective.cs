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
            Vector3 scale = col.bounds.size;
            
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

        foreach (GameObject paintObj in paintController.paintObjects) CheckPaintCollision(paintObj); // Check all existing paint splats at the start

        yield return new WaitUntil(() => paintPercentage > percentageToComplete);
        active = false;
        completed = true;
        Debug.Log("Paint objective complete");
        objectives.RemoveObjective(this);
    }

    private void Update()
    {
        paint = paintController.paintObjects.Count; // Get paintobjects from paintcontroller
        if (paint > paintLastFrame) CheckNewPaint(paint - paintLastFrame);
        paintLastFrame = paint;
    }

    void CheckNewPaint(int newPaint) // Go through all new paint objects
    {
        foreach (GameObject paintObj in paintController.paintObjects.GetRange(paintController.paintObjects.Count-newPaint, newPaint))
        {
            CheckPaintCollision(paintObj);
        }
    }

    void CheckPaintCollision(GameObject paintObj) // Check whether the collider is in the paint area (or whether any of the area points are inside the collider)
    {
        paintObj.SetActive(true);

        List<Vector3> pointsInCol = new List<Vector3>();
        foreach (Vector3 point in paintAreaPoints)
        {
            Collider paintCol = paintObj.GetComponent<Collider>();
            if (paintCol.bounds.Contains(point))
            {
                pointsInCol.Add(point);
            }
        }
        paintObj.SetActive(false);
        UpdatePaintArea(pointsInCol);
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
