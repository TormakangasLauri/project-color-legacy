using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintObjective : MonoBehaviour
{
    public List<PaintTarget.PaintPoint> paintPoints = new List<PaintTarget.PaintPoint>();
    
    void Start()
    {
        paintPoints = PaintTarget.paintWorldPositions;
    }
}
