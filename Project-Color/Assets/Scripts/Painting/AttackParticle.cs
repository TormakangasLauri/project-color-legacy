using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackParticle : MonoBehaviour
{
    public Collider brushCol;
    public Collider cBrushCol;

    public float duration = 0.1f;

    public LayerMask layer;

    public Brush brush;

    public IEnumerator Rotate(bool startFromRight, bool charged)
    {
        Quaternion startRotation;
        Quaternion endRotation;
        if (startFromRight) // Right to left
        {
            transform.localRotation = Quaternion.Euler(-20, 65, 0);
            startRotation = transform.localRotation;
            endRotation = Quaternion.Euler(20, -60, 0);
        }
        else // Left to right
        {
            transform.localRotation = Quaternion.Euler(-20, -65, 0);
            startRotation = transform.localRotation;
            endRotation = Quaternion.Euler(20, 60, 0);
        }
        
        float elapsed = 0;

        if (!charged) brushCol.enabled = true;
        else cBrushCol.enabled = true;

        while (elapsed < duration)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!charged) brushCol.enabled = false;
        else cBrushCol.enabled = false;

        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}