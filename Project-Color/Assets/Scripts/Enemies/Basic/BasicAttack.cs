using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BasicAttack : EnemyAttack
{
    Transform attackPivot;
    public float hLeapForce = 5;
    public float vLeapForce = 5;

    public GameObject targetInHitbox;

    private void Start()
    {
        attackPivot = transform.GetChild(3).transform;
    }

    public override void Attack()
    {
        if (!onCooldown && !attacking)
        {
            attacking = true;

            Vector3 targetDirection = (target.transform.position - transform.position).normalized;
            //attackPivot.rotation = Quaternion.Euler(Vector3.Angle(targetDirection, transform.forward), 0, 0); // Turn attack hitbox towards the target
            rb.AddForce(targetDirection * hLeapForce + Vector3.up * vLeapForce, ForceMode.Impulse);
            StartCoroutine(WaitForHit());

            IEnumerator WaitForHit()
            {
                float timer = 1f;
                yield return new WaitUntil(() => // Wait for the target to be in the attack hitbox
                {
                    transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(targetDirection, Vector3.up));
                    timer -= Time.deltaTime;
                    return timer <= 0 || targetInHitbox != null;
                });
                if (targetInHitbox != null) // Target hit
                {
                    targetInHitbox.GetComponent<Health>().Damage(damage, targetDirection * 14 + Vector3.up * 5);
                }
                attacking = false;
                StartCooldown();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) targetInHitbox = other.transform.parent.parent.gameObject;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) targetInHitbox = null;
    }
}
