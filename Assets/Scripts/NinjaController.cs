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

    [SerializeField]
    protected float verticalTime = 0.5f;

    private float verticalUp, verticalDown;
    private RopeController ropeController,
                           lastRopeController;
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
                Detach();
                ropeController = Instantiate(ropeControllerPrefab, transform.position, Quaternion.identity) as RopeController;
                ropeController.AttachRope(rayHits.point, anchoredJoint2D);
                anchoredJoint2D.enabled = true;
                rigidbody.AddForce(direction.normalized);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Detach();
        }

        if (ropeController != null)
        {
            float horizontal = Input.GetAxis("Horizontal");
            if (horizontal != 0)
            {
                Vector2 ropeDirection = (transform.position - ropeController.LastRopeNode.position).normalized,
                        swingDirection = Quaternion.Euler(0, 0, 90 * Mathf.Sign(horizontal)) * ropeDirection;

                Vector2 force = (ropeDirection + swingDirection) * ropeSwaySpeed;
                rigidbody.AddForceAtPosition(force, transform.position + new Vector3(0, -1));
            }
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

        if (current != null)
        {
            float vertical = Input.GetAxis("Vertical");
            if (vertical != 0)
            {
                float check;
                if (vertical < 0)
                {
                    check = verticalDown += Time.deltaTime;
                    verticalUp = 0.0f;
                }
                else
                {
                    check = verticalUp += Time.deltaTime;
                    verticalDown = 0.0f;
                }
                if (check >= verticalTime)
                {
                    var next = current.RopeController.GetNextRopeNode(current, vertical);
                    if (next != null)
                    {
                        AttachToRopeNode(next);
                        verticalUp = 0.0f;
                        verticalDown = 0.0f;
                    }
                }
            }
        }
    }

    private void Detach()
    {
        if (ropeController != null)
        {
            ropeController.DetachRope();
            lastRopeController = ropeController;
            ropeController = null;
            grabCooldown = -0.25f;
        }
        current = null;
    }

    //protected void OnCollisionEnter2D(Collision2D other)
    //{
    //    RopeNode ropeNode = other.gameObject.GetComponent<RopeNode>();
    //    if (grabCooldown > 0 && ropeController == null && ropeNode != null && ropeNode.RopeController != null)
    //    {
    //        ropeController = ropeNode.RopeController;
    //        anchoredJoint2D.connectedBody = ropeNode.Rigidbody2D;
    //        anchoredJoint2D.enabled = true;
    //    }
    //}

    RopeNode current;
    protected void OnTriggerEnter2D(Collider2D other)
    {
        RopeNode ropeNode = other.gameObject.GetComponent<RopeNode>();
        if (Input.GetKey(KeyCode.Space) && ropeController == null && ropeNode != null && ropeNode.RopeController != null && (lastRopeController != ropeNode.RopeController || grabCooldown > 0))
        {
            AttachToRopeNode(ropeNode);
        }
    }

    private void AttachToRopeNode(RopeNode ropeNode)
    {
        ropeController = ropeNode.RopeController;
        anchoredJoint2D.connectedBody = ropeNode.Rigidbody2D;
        anchoredJoint2D.enabled = true;
        current = ropeNode;
    }

    protected void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
}
