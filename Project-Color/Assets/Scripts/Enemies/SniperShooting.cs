using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperShooting : MonoBehaviour
{
    public GameObject target;
    
    public GameObject bullet;
    public GameObject bulletTrail;
    public Transform shootPoint;
    public float bulletSpeed;

    public float shootCooldown;
    public float t;
    public bool LOSToTarget;

    private void Start()
    {
        target = GameObject.FindWithTag("Player");
        t = shootCooldown;
    }

    void Update()
    {
        if (!Physics.Linecast(transform.position, target.transform.position, LayerMask.GetMask("Terrain"))
            || Physics.Linecast(transform.position, target.transform.position, LayerMask.GetMask("Enemy")))
            LOSToTarget = true;
        else LOSToTarget = false;

        if (LOSToTarget) t -= Time.deltaTime;
        else t = shootCooldown;
        
        if (LOSToTarget && t < 0)
        {
            t = shootCooldown;
            // Shoot();
            StartCoroutine(Shoot2());
        }

        Vector3 Rot = target.transform.position - transform.position;
        GetComponent<Rigidbody>().MoveRotation(Quaternion.LookRotation(new Vector3(Rot.x, 0, Rot.z)));
    }

    private void Shoot()
    {
        Vector3 targetDirection = (target.transform.position - shootPoint.position).normalized;
        GameObject newBullet = Instantiate(bullet, shootPoint.position, Quaternion.LookRotation(targetDirection), GetComponentInParent<Transform>());
        newBullet.GetComponent<Rigidbody>().velocity = targetDirection * bulletSpeed;
        newBullet.GetComponent<Bullet>().direction = targetDirection;
        newBullet.GetComponent<Bullet>().shooter = gameObject;
    }

    private IEnumerator Shoot2()
    {
        Vector3 targetDirection = (target.transform.position - shootPoint.position).normalized;
        yield return new WaitForSeconds(0.1f);
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, targetDirection, out hit))
        {
            GameObject hitObj = hit.transform.gameObject;
            if (hitObj.CompareTag("Player") || hitObj.layer == LayerMask.GetMask("Enemy"))
            {
                // Player or enemy hit
                StartCoroutine(BulletTrail(true));
            }

        }
        else StartCoroutine(BulletTrail(false));
    }

    private IEnumerator BulletTrail(bool hitPlayer)
    {
        Vector3 targetDir = target.transform.position - shootPoint.position;
        GameObject trail = Instantiate(bulletTrail, shootPoint.position + targetDir / 2, Quaternion.LookRotation(targetDir));
        trail.transform.Rotate(new Vector3(90,0,0));
        trail.transform.localScale = new Vector3(0.05f, targetDir.magnitude / 2, 0.05f);
        
        if (hitPlayer) trail.GetComponent<MeshRenderer>().material.color = Color.red;
        
        yield return new WaitForSeconds(1);
        Destroy(trail);
    }
}
