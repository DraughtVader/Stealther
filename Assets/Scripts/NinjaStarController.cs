using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaStarController : ProjectileController
{
    [SerializeField]
    protected float rotationSpeed = 1.0f;

    protected override void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, rotationSpeed);
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
        var ropeNode = other.gameObject.GetComponent<RopeNode>();
        if (ropeNode)
        {
            ropeNode.CutRope();
        }
        Destroy(gameObject);
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        var ropeNode = other.gameObject.GetComponent<RopeNode>();
        if (ropeNode)
        {
            ropeNode.CutRope();
        }
        Destroy(gameObject);
    }
}
