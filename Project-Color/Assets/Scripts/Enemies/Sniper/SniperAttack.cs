using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

public class SniperAttack : EnemyAttack
{
    public GameObject bullet;
    public GameObject bulletTrail;
    public Transform shootPoint;
    public float bulletSpeed;

    public bool LOSToTarget;
    public bool moving = false;

    private void Start()
    {
        cooldownTimer = attackCooldown;
    }

    protected override void Update()
    {
        base.Update();

        Vector3 position = transform.position;
        Vector3 tPosition = target.transform.position;
        LOSToTarget = !Physics.Raycast(position, tPosition - position, Vector3.Distance(tPosition, position), LayerMask.GetMask("Terrain"));

        if (!LOSToTarget || moving) StartCooldown();


        if (LOSToTarget && !onCooldown)
        {            
            // Shooting with bullets
            // Collision detection is a bit questionable so we're using raycasts for now
            // Shoot();
            
            // Shooting with raycast
            Attack();
            StartCooldown();
        }
    }

    private void Shoot()
    {
        Vector3 targetDirection = (target.transform.position - shootPoint.position).normalized;
        GameObject newBullet = Instantiate(bullet, shootPoint.position, Quaternion.LookRotation(targetDirection), GetComponentInParent<Transform>());
        newBullet.GetComponent<Rigidbody>().linearVelocity = targetDirection * bulletSpeed;
        newBullet.GetComponent<Bullet>().direction = targetDirection;
        newBullet.GetComponent<Bullet>().shooter = gameObject;
    }

    public override void Attack()
    {
        attacking = true;
        StartCoroutine(Shoot());

        IEnumerator Shoot()
        {
            Vector3 targetDirection = (target.transform.position - shootPoint.position).normalized;
            yield return new WaitForSeconds(0.1f);

            RaycastHit[] allHits = Physics.RaycastAll(shootPoint.position, targetDirection, 1000, ~0, QueryTriggerInteraction.Ignore);
            List<RaycastHit> hits = new List<RaycastHit>();
            hits.AddRange(allHits);

            if (hits.Count > 0)
            {
                hits.Sort((hit1, hit2) => { return Vector3.Distance(hit1.point, transform.position).CompareTo(Vector3.Distance(hit2.point, transform.position)); }); // Sort by distance

                GameObject hitObj = hits[0].transform.gameObject;
                if (EnemyController.all.activeList.Contains(hitObj) && hitObj != gameObject)
                {
                    // Enemy hit
                    StartCoroutine(BulletTrail(2));
                }
                else if (hitObj.CompareTag("PlayerRoot") || hitObj.CompareTag("Player"))
                {
                    // Player hit
                    hitObj.GetComponentInParent<Health>().Damage(damage);
                    StartCoroutine(BulletTrail(1));
                }
                else
                {
                    StartCoroutine(BulletTrail(0));
                }
            }
            else Debug.Log("No hits");
        }
        attacking = false;
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
