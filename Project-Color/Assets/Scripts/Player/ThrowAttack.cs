using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowAttack : MonoBehaviour
{
    PlayerAttack PA;
    GameObject player;
    public GameObject brush;
    
    public float maxRange;
    public float radius;
    public float speed;
    public float floatTime;
    public float speedLossRadius;
    public LayerMask hitLayer;

    public float damage;
    public float knockback;

    void Start()
    {
        PA = GetComponent<PlayerAttack>();
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && PA.canAttack)
        {
            StartCoroutine(Throw());
        }
    }

    IEnumerator Throw()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 point;
        // Set point away from the hit surface the amount of radius or set it in the air if max distance reached
        if (Physics.Raycast(ray, out hit, maxRange, hitLayer))
            point = hit.point + (player.transform.position - hit.point).normalized * radius;
        else
            point = player.transform.position + Camera.main.transform.forward * maxRange;

        // Create the "brush" and assign variables
        GameObject b = Instantiate(brush, player.transform.position, Quaternion.Euler(90, 0, 0));
        Brush B = b.GetComponent<Brush>();
        
        B.point = point;
        B.radius = radius;
        B.speed = speed;
        B.floatTime = floatTime;
        B.speedLossRadius = speedLossRadius;
        B.damage = damage;
        B.knockback = knockback;

        yield return null;
    }
}
