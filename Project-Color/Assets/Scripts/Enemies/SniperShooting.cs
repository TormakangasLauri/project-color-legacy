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
        if (!Physics.Linecast(transform.position, target.transform.position, LayerMask.GetMask("Terrain")))
            LOSToTarget = true;
        else
            LOSToTarget = false;

        if (LOSToTarget) t -= Time.deltaTime;
        else t = shootCooldown;
        
        if (LOSToTarget && t < 0)
        {
            t = shootCooldown;
            
            // Shooting with bullets
            // Shoot();
            
            // Shooting with raycast
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
            if (EnemyController.inst.AllEnemies.Contains(hitObj))
            {
                // Enemy hit
                StartCoroutine(BulletTrail(2));
            }
            else if (hitObj.CompareTag("Player"))
            {
                // Player hit
                StartCoroutine(BulletTrail(1));
            }

        }
        else StartCoroutine(BulletTrail(0));
    }

    private IEnumerator BulletTrail(int hit)
    {
        Vector3 targetDir = target.transform.position - shootPoint.position;
        GameObject trail = Instantiate(bulletTrail, shootPoint.position + targetDir / 2, Quaternion.LookRotation(targetDir));
        trail.transform.Rotate(new Vector3(90,0,0));
        trail.transform.localScale = new Vector3(0.05f, targetDir.magnitude / 2, 0.05f);
        
        if (hit == 1) trail.GetComponent<MeshRenderer>().material.color = Color.red;
        else if (hit == 2) trail.GetComponent<MeshRenderer>().material.color = Color.yellow;

        Color currentColor = trail.GetComponent<MeshRenderer>().material.color;
        for (float i = 1; i > 0; i -= 0.1f)
        {
            trail.GetComponent<MeshRenderer>().material.color = new Color(currentColor.r, currentColor.g, currentColor.b, i);
            yield return new WaitForSeconds(0.1f);
        }
        
        Destroy(trail);
    }
}
