using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeNode : MonoBehaviour
{
    [SerializeField]
    protected float correctionForce = 1.0f;

    protected Rigidbody2D rigidBody2d;
    public Rigidbody2D Rigidbody2D
    {
        get
        {
            if (rigidBody2d == null)
            {
                rigidBody2d = GetComponent<Rigidbody2D>();
            }
            return rigidBody2d;
        }
    }

    protected AnchoredJoint2D anchoredJoint2D;
    public AnchoredJoint2D AnchoredJoint2D
    {
        get
        {
            if (anchoredJoint2D == null)
            {
                anchoredJoint2D = GetComponent<AnchoredJoint2D>();
            }
            return anchoredJoint2D;
        }
    }

    public RopeController RopeController { get; set; }

    public void CutRope()
    {
        if (RopeController != null)
        {
            RopeController.CutRope(this);
            Destroy();
        }
    }

    private void Update()
    {
        if (anchoredJoint2D.connectedBody != null)
        {
            float jointDistance = (anchoredJoint2D as DistanceJoint2D).distance,
                  distance = Vector2.Distance(transform.position, anchoredJoint2D.connectedBody.position);
            if (distance > 2 * jointDistance)
            {
                Vector2 direction = (anchoredJoint2D.connectedBody.position - (Vector2)transform.position).normalized,
                        force = direction * correctionForce * distance / jointDistance;
                rigidBody2d.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
