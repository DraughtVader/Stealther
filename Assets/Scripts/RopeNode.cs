using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeNode : MonoBehaviour
{
    [SerializeField]
    protected float correctionForce = 1.0f;

    protected Rigidbody2D rigidBody2D;
    public Rigidbody2D Rigidbody2D
    {
        get{  return rigidBody2D;}
    }

    protected AnchoredJoint2D anchoredJoint2D;
    public AnchoredJoint2D AnchoredJoint2D
    {
        get{ return anchoredJoint2D;}
    }

    public RopeController RopeController { get; set; }

    public void CutRope(Hazard cutter)
    {
        if (RopeController == null)
        {
            return;
        }
        var star = cutter as NinjaStarController;
        if (star != null && star.Thrower == RopeController.AttachedNinja)
        {
            return;
        }

        RopeController.CutRope(this);
        Destroy();
    }

    private void Update()
    {
        if (anchoredJoint2D.connectedBody == null)
        {
            return;
        }
        float jointDistance = (anchoredJoint2D as DistanceJoint2D).distance,
            distance = Vector2.Distance(transform.position, anchoredJoint2D.connectedBody.position);
        if (distance > 2 * jointDistance)
        {
            Vector2 direction = (anchoredJoint2D.connectedBody.position - (Vector2)transform.position).normalized,
                force = direction * correctionForce * distance / jointDistance;
            rigidBody2D.AddForce(force, ForceMode2D.Impulse);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        anchoredJoint2D = GetComponent<AnchoredJoint2D>();
    }
}
