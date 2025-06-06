using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintController : MonoBehaviour
{
    private List<PaintPoint> paintPoints = new List<PaintPoint>();

    public static List<GameObject> paintObjects = new List<GameObject>();
    private List<GameObject> newPaintbjects = new List<GameObject>();
    public static List<GameObject> paintGroups = new List<GameObject>();
    public static List<GameObject> paintGroupsInPaintArea = new List<GameObject>();
    public static List<GameObject> paintGroupsOccupied = new List<GameObject>();

    [HideInInspector] public int paint;
    [HideInInspector] public int paintLastFrame;

    private float paintGroupRadius = 4;

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
            if (paint > paintLastFrame)
            {
                CreatePaintObjects(); // Check for new paint
                UpdatePaintGroups();
            }
            paintLastFrame = paint;
        }
    }

    public void CreatePaintObjects()
    {
        int newPaint = paint - paintLastFrame;
        foreach (PaintPoint p in paintPoints.GetRange(paintPoints.Count - newPaint, newPaint))
        {
            GameObject paintObj = new GameObject("PaintObj"); // Paint splat object
            paintObj.transform.position = p.point;
            paintObj.transform.rotation = Quaternion.LookRotation(p.normal);
            paintObj.hideFlags = HideFlags.HideInHierarchy;

            BoxCollider paintCol = paintObj.AddComponent<BoxCollider>(); // Create a collider for the paint splat
            paintCol.isTrigger = true;
            paintCol.size = new Vector3(p.scale, p.scale, 0.2f);

            paintObjects.Add(paintObj);
            newPaintbjects.Add(paintObj);

            paintObj.SetActive(false); // Optimization shit
        }
    }

    void UpdatePaintGroups()
    {
        foreach (GameObject p in newPaintbjects)
        {
            bool inGroup = false;
            foreach (GameObject group in paintGroups)
                if (Vector3.Distance(p.transform.position, group.transform.position) <= paintGroupRadius)
                {
                    p.transform.parent = group.transform; // Add paintobject to an existing group
                    p.hideFlags = HideFlags.None;
                    inGroup = true;
                    break;
                }

            if (!inGroup) CreatePaintGroup(p);
        }

        newPaintbjects.Clear();
    }

    void CreatePaintGroup(GameObject paintObj)
    {
        GameObject group = new GameObject();
        group.transform.position = paintObj.transform.position;
        group.transform.rotation = paintObj.transform.rotation;
        group.name = "PaintGroup";

        paintObj.transform.parent = group.transform;
        paintObj.hideFlags = HideFlags.None;

        SphereCollider col = group.AddComponent<SphereCollider>();
        col.radius = paintGroupRadius;
        col.isTrigger = true;

        paintGroups.Add(group);
    }

    public static void CleanPaintGroup(GameObject paintGroup)
    {
        paintGroupsOccupied.Remove(paintGroup);
        Destroy(paintGroup);
    }

    public static void OccupyPaintGroup(GameObject paintGroup)
    {
        if (paintGroups.Contains(paintGroup)) paintGroups.Remove(paintGroup);
        if (paintGroupsInPaintArea.Contains(paintGroup)) paintGroupsInPaintArea.Remove(paintGroup);

        paintGroupsOccupied.Add(paintGroup);
    }

    public static void ClearAllPaintObjects()
    {
        for (int i = 0; i < paintObjects.Count; i++) Destroy(paintObjects[i]);
        paintObjects.Clear();
        for (int i = 0; i < paintGroups.Count; i++) Destroy(paintGroups[i]);
        paintGroups.Clear();
    }
}
