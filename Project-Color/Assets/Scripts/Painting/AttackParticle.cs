using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class AttackParticle : MonoBehaviour
{
    public Collider brushCol;
    public Collider cBrushCol;
    public Collider brushCol2;
    public Collider cBrushCol2;

    public GameObject BC;

    public float duration = 0.1f;

    public LayerMask layer;

    public Brush brush;

    public IEnumerator Rotate(bool startFromRight, bool charged)
    {
        StartCoroutine(moveHB(startFromRight, charged));

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

        //if (!charged) brushCol.enabled = true;
        //else cBrushCol.enabled = true;

        while (elapsed < duration)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        //if (!charged) brushCol.enabled = false;
        //else cBrushCol.enabled = false;

        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public IEnumerator moveHB(bool startFromRight, bool charged)
    {
        Vector3 startPosition;
        Vector3 endPosition;

        if (!charged) startPosition = new Vector3(2, 0.8f, 0); //Normal
        else startPosition = new Vector3(4, 1, 0); //Charged
        if (!startFromRight) startPosition.x *= -1;

        BC.transform.localPosition = startPosition;
        endPosition = startPosition * -1;
        
        float elapsed = 0;

        if (!charged) brushCol2.enabled = true;
        else cBrushCol2.enabled = true;

        while (elapsed < duration)
        {
            BC.transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (!charged) brushCol2.enabled = false;
        else cBrushCol2.enabled = false;

        BC.transform.localPosition = new Vector3(0, 0, 0);
    }
}