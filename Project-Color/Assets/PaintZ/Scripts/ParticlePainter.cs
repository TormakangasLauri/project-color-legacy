using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePainter : MonoBehaviour
{
    public Brush brush;
    public bool RandomChannel = false;

    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents;

    private void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) brush.splatChannel = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) brush.splatChannel = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) brush.splatChannel = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) brush.splatChannel = 3;
        if (Input.GetKeyDown(KeyCode.Alpha5)) brush.splatChannel = 4;
    }

    private void OnParticleCollision(GameObject other)
    {
        PaintTarget paintTarget = other.GetComponent<PaintTarget>();
        if (paintTarget != null)
        {
            if (RandomChannel) brush.splatChannel = Random.Range(0, 4);

            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
            for (int i = 0; i < numCollisionEvents; i++)
            {
                PaintTarget.PaintObject(paintTarget, collisionEvents[i].intersection, collisionEvents[i].normal, brush);
            }
        }
    }
}