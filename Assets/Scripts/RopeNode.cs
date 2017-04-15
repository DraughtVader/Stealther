using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeNode : MonoBehaviour
{
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

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
