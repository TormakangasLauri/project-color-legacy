using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackParticle : MonoBehaviour
{
    public float duration = 0.1f;

    public IEnumerator RotateRL()
    {
        transform.localRotation = Quaternion.Euler(-20, 60, 0);
        Quaternion startRotation = transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(20, -60, 0);
        float elapsed = 0;

        while (elapsed < duration)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0, 0, 0);

        yield return null;
    }

    public IEnumerator RotateLR()
    {
        transform.localRotation = Quaternion.Euler(-20, -60, 0);
        Quaternion startRotation = transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(20, 60, 0);
        float elapsed = 0;

        while (elapsed < duration)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0, 0, 0);

        yield return null;
    }
}