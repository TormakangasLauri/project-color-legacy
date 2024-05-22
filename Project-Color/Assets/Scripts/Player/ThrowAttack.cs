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
    public LayerMask hitLayer;

    bool thrown = false;

    Vector3 velocity = Vector3.zero;

    void Start()
    {
        PA = GetComponent<PlayerAttack>();
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !thrown)
        {
            StartCoroutine(Throw());
        }
    }

    IEnumerator Throw()
    {
        thrown = true;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, maxRange, hitLayer);
        Vector3 point;
        if (hit.transform != null)
        {
            point = hit.point + (player.transform.position - hit.point).normalized * radius;
        }
        else
        {
            point = player.transform.position + Camera.main.transform.forward * maxRange;
        }

        GameObject b = Instantiate(brush, player.transform.position, Quaternion.Euler(90, 0, 0));
        b.GetComponent<Brush>().point = point;

        yield return null;
        thrown = false;
    }
}
