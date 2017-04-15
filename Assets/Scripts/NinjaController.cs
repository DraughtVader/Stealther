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
    private float grabCooldown;

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
        else if (Input.GetMouseButtonUp(1) || Input.GetKeyDown(KeyCode.Space))
        {
            if (ropeController != null)
            {
                ropeController.DetachRope();
                ropeController = null;
                grabCooldown = -0.5f;
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
            Vector2 direction = ((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position)).normalized;
            var projectile = Instantiate(projectilePrefab, (Vector2)transform.position + direction, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().AddForce(direction * projectileSpeed, ForceMode2D.Impulse);
        }

        if (ropeController == null)
        {
            grabCooldown += Time.deltaTime;
        }
    }

    protected void OnCollisionEnter2D(Collision2D other)
    {
        RopeNode ropeNode = other.gameObject.GetComponent<RopeNode>();
        if (grabCooldown > 0 && ropeController == null && ropeNode != null && ropeNode.RopeController != null)
        {
            ropeController = ropeNode.RopeController;
            anchoredJoint2D.connectedBody = ropeNode.Rigidbody2D;
            anchoredJoint2D.enabled = true;
        }
    }

    protected void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
}
