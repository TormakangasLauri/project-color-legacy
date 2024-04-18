using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperShooting : MonoBehaviour
{
    public GameObject target;
    
    public GameObject bullet;
    public Transform shootPoint;
    public float bulletSpeed;

    public float shootCooldown;
    private float t;
    private bool LOSToTarget;

    private void Start()
    {
        target = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        t -= Time.deltaTime;
        if (!Physics.Linecast(transform.position, target.transform.position, LayerMask.GetMask("Terrain")) && !Physics.Linecast(transform.position, target.transform.position, LayerMask.GetMask("Enemy")))
            LOSToTarget = true;
        else LOSToTarget = false;
        
        if (LOSToTarget && t < 0)
        {
            t = shootCooldown;
            Shoot();
        }

        Vector3 Rot = target.transform.position - transform.position;
        GetComponent<Rigidbody>().MoveRotation(Quaternion.LookRotation(new Vector3(Rot.x, 0, Rot.z)));
    }

    private void Shoot()
    {
        GameObject newBullet = Instantiate(bullet, shootPoint.position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody>().velocity = (target.transform.position - shootPoint.position).normalized * bulletSpeed;
    }
}
