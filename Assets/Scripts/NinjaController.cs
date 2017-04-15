using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaController : MonoBehaviour
{
    [SerializeField]
    protected AnchoredJoint2D anchoredJoint2D;

    [SerializeField]
    protected float ropeWindSpeed = 2.5f,
                    ropeSwaySpeed = 2.5f,
                    projectileSpeed = 5.0f;

    [SerializeField]
    protected RopeController ropeControllerPrefab;


    [SerializeField]
    protected GameObject projectilePrefab;

    protected RopeController ropeController;
    private new Rigidbody2D rigidbody;

    protected void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            var rayHits = Physics2D.Raycast(transform.position, direction, 10.0f, 1 << LayerMask.NameToLayer("Ropables"));
            if (rayHits.transform != null)
            {
                ropeController = Instantiate(ropeControllerPrefab, transform.position, Quaternion.identity) as RopeController;
                ropeController.AttachRope(rayHits.point, anchoredJoint2D);
                anchoredJoint2D.enabled = true;
                rigidbody.AddForce(direction.normalized);
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (ropeController != null)
            {
                ropeController.DetachRope();
                ropeController = null;
            }
        }

        if (ropeController != null)
        {
            float horizontal = Input.GetAxis("Horizontal");
            var force = ropeController.FirstRopeNode.right;
            force.x *= horizontal * ropeSwaySpeed;
            rigidbody.AddForce(force);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            var projectile = Instantiate(projectilePrefab, transform.position + direction * 1.5f, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().AddForce(direction * projectileSpeed, ForceMode2D.Impulse);
        }
    }

    protected void OnCollisionEnter2D(Collision2D other)
    {
        RopeNode ropeNode = other.gameObject.GetComponent<RopeNode>();
        if (ropeNode != null)
        {

        }
    }

    protected void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
}
