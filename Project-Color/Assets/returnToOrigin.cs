using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class returnToOrigin : MonoBehaviour
{
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // vector3.zero on koordinaateissa (0, 0, 0) (origin)
                other.transform.position = Vector3.zero;
            }
        }
}
