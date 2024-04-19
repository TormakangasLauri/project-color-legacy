using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PaintSplash : MonoBehaviour
{
    [FormerlySerializedAs("part")] public ParticleSystem ps;
    public List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // void OnParticleCollision(GameObject other)  // Does not work.
    // {
    //     Paintable paintable = other.gameObject.GetComponent<Paintable>();
    //
    //     foreach (ParticleCollisionEvent collisionEvent in collisionEvents)
    //     {
    //         if (paintable)
    //         {
    //             paintable.AddPaint();
    //         }
    //     }
    // }
}

