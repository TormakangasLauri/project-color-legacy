using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using static UnityEditor.PlayerSettings;

public class EnemyMovement : BaseEnemyMovement
{ 
    private enum states { inactive, idle, navmesh, los, attack };
    private states currentState;
    
    private void FixedUpdate()
    {
        // Change to inactive
        if (!_enemyType.active) currentState = states.inactive;
        if (_enemyAttack.attacking) currentState = states.attack;

        // States
        //  inactive: while enemy is inactive (not spawned / dead)
        //  idle: not moving / wandering, will finish later
        //  Move: move with force towards the next corner of NavMeshPath
        //  attack: attacking
        switch (currentState)
        {
            case states.inactive: Inactive(); break;
            case states.idle: Idle(); break;
            case states.navmesh: NavMeshMovement(); break;
            case states.los: LOSMovement(); break;
            case states.attack: Attack(); break;
        }

        // Gravity
        if (rb.velocity.y < 0) rb.AddForce(Vector3.down * 20);
    }

    private void Inactive()
    {
        if (_enemyType.active) StartCoroutine(StateToNav());
        IEnumerator StateToNav()
        {
            yield return new WaitForSeconds(0.1f * _enemyType.timeScale);
            currentState = states.navmesh;
        }
    }

    private void Idle()
    {
        
    }

    private void NavMeshMovement()
    {
        if (gameObject.activeSelf && _enemyType.timeActive > 0.1 && path.corners.Length > 2)
        {
            /*Vector3 targetPos = target.transform.position;
            Vector3 cornerPos = path.corners[1];
            Vector3 pos = transform.position;
            Vector3 directionToTarget = new Vector3(cornerPos.x - pos.x, 0, cornerPos.z - pos.z).normalized;
            Vector3 movement = directionToTarget * (speed * 10);
            float distOnXZ = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(targetPos.x, 0, targetPos.z));

            // Rotate to face the player
            if (_enemyType.timeScale != 0) rb.MoveRotation(Quaternion.LookRotation(directionToTarget));

            // Moving when not in stopping distance of the target
            if (distOnXZ > stopDistance + stopDistance / 2) rb.AddForce(movement);
            // Slow down enemy when in stopping distance
            else if (rb.velocity.magnitude > 0.5 * _enemyType.timeScale) rb.AddForce(-rb.velocity * 2);
            // Speed limit
            if (rb.velocity.magnitude > speed * _enemyType.timeScale) rb.AddForce(-movement);
            // Move away from the target when too close
            if (distOnXZ < 2)
            {
                rb.AddForce(-movement / 3);
                if (rb.velocity.magnitude > speed * _enemyType.timeScale) rb.AddForce(movement);
            }*/

            Move(path.corners[1]);
        }

        RaycastHit hit;
        Physics.Raycast(target.transform.position, Vector3.down, out hit, 100, terrainLayer);

        // State change check
        if (LOSToTarget && (path.corners.Length <= 2 || hit.point.y <= transform.position.y - transform.localScale.y / 2))
        {
            currentState = states.los;
        }
    }

    private void LOSMovement()
    {
        /*Vector3 targetPos = target.transform.position;
        Vector3 pos = transform.position;
        Vector3 directionToTarget = new Vector3(targetPos.x - pos.x, 0, targetPos.z - pos.z).normalized;
        Vector3 movement = directionToTarget * (speed * 10);
        float distOnXZ = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(targetPos.x, 0, targetPos.z));

        // Rotate to face the player
        if (_enemyType.timeScale != 0) rb.MoveRotation(Quaternion.LookRotation(directionToTarget));

        // Moving when not in stopping distance of the target
        if (distOnXZ > stopDistance + stopDistance / 2) rb.AddForce(movement);
        // Slow down enemy when in stopping distance
        else
        {
            if (stopDistance < 3f) _enemyAttack.Attack(); // Make only the closest enemies attack
            if (rb.velocity.magnitude > 0.5 * _enemyType.timeScale) rb.AddForce(-rb.velocity * 2);
        }
        // Speed limit
        if (rb.velocity.magnitude > speed * _enemyType.timeScale) rb.AddForce(-movement);
        // Move away from the target when too close
        if (distOnXZ < 2)
        {
            rb.AddForce(-movement / 3);
            if (rb.velocity.magnitude > speed * _enemyType.timeScale) rb.AddForce(movement);
        }*/

        Vector3 pos = transform.position;
        Vector3 targetPos = target.transform.position;
        float distOnXZ = Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(targetPos.x, 0, targetPos.z));

        if (distOnXZ > stopDistance + 0.5) Move(targetPos, true);
        else if (distOnXZ < 2) Move(targetPos, true, -0.6f);

        Rotate(); // Rotate to face the target (player)

        // State change check
        RaycastHit hit;
        Physics.Raycast(target.transform.position, Vector3.down, out hit, 100, terrainLayer);
        RaycastHit enemyHit;
        Physics.Raycast(transform.position, Vector3.down, out enemyHit, 100, terrainLayer);
        if ((path.corners.Length > 2 || !LOSToTarget) && hit.point.y + 0.1 >= enemyHit.point.y && grounded)
        {
            currentState = states.navmesh;
        }
    }

    private void Attack()
    {
        if (!_enemyAttack.attacking) currentState = LOSToTarget ? states.los : states.navmesh;
    }
}
