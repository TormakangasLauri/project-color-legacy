using UnityEngine;
using UnityEngine.Rendering;

public class CollisionPainter : MonoBehaviour
{
    public Brush brush;
    public bool RandomChannel = false;

    private void Start()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        HandleCollision(collision);
    }

    private void OnTriggerEnter(Collider collider)
    {
        HandleTrigger(collider);
    }

    private void OnTriggerStay(Collider collider)
    {
        HandleTrigger(collider);
    }

    private void HandleCollision(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            PaintTarget paintTarget = contact.otherCollider.GetComponent<PaintTarget>();
            if (paintTarget != null)
            {
                if (RandomChannel) brush.splatChannel = Random.Range(0, 4);
                PaintTarget.PaintObject(paintTarget, contact.point, contact.normal, brush);
            }
        }
    }

    private void HandleTrigger(Collider col)
    {
        PaintTarget paintTarget = col.GetComponent<PaintTarget>();
        if (paintTarget != null)
        {
            if (RandomChannel) brush.splatChannel = Random.Range(0, 4);
            PaintTarget.PaintObject(paintTarget, col.ClosestPoint(transform.position), (transform.position - col.ClosestPoint(transform.position)).normalized, brush);
        }
    }
}