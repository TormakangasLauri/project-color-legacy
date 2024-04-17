using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceGameObject : MonoBehaviour
{
    public GameObject swapFrom; // GameObject we want to be replaced
    public GameObject swapTo;  // GameObject we want to replace the other

    private bool hasSwapped = false;
    
    private Vector3 position = new Vector3();
    private Quaternion rotation = new Quaternion();
    

    private void OnTriggerEnter(Collider other) // Replaces a gameObject with another when entering trigger
    {
        if (!hasSwapped)
            if (other.CompareTag(swapFrom.tag))  // Shouldn't need to check if objects are null but something to keep in mind if crashes happen
            {
                // Storing of values for swapping
                position = swapFrom.transform.position;
                rotation = swapFrom.transform.rotation;
                
                // The actual swapping
                Destroy(swapFrom);
                Instantiate(swapTo, position, rotation);
                hasSwapped = true;
            }
                
    }
}
