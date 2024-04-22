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

    void OnParticleCollision(GameObject other)
    {
        Paintable paintable = other.gameObject.GetComponent<Paintable>();
        
        // foreach (ParticleCollisionEvent collisionEvent in collisionEvents)
        // {
        //     if (paintable)
        //     {
        //         paintable.AddPaint();
        //     }
        // }

        // Following code made by non other than mkeyos
        // Only works if the other object has a meshcollider, otherwise colUV is always (0, 0)
        
        int colEvents = ps.GetCollisionEvents(other, collisionEvents);
        for (int i = 0; i < colEvents; i++)
        {
            Vector3 colNormal = collisionEvents[i].normal;
            RaycastHit hit;
            Physics.Raycast(collisionEvents[i].intersection, -colNormal, out hit);
            if (hit.collider.GetComponent<Renderer>() != null)
            {
                Vector2 colUV = hit.textureCoord;
                Debug.Log(colUV);
                // paintable.AddPaint(colUV, "texture", "color");
            }
        }
    }
}

