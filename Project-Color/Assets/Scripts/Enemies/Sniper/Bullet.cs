using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    [HideInInspector] public GameObject shooter;
    [HideInInspector] public Vector3 direction;
    private void Start() { rb = GetComponent<Rigidbody>(); }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != shooter)
        {
            Rigidbody otherRb = other.gameObject.GetComponent<Rigidbody>();
            if (otherRb != null) otherRb.AddForce(direction * 1000);

            Destroy(gameObject);
        }
    }
}
