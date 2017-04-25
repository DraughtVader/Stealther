using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaController : MonoBehaviour
{
    public enum NinjaState
    {
        NotPlaying, WaitingToJoin, WaitingToPlay, Alive, Dead
    }

    [SerializeField]
    protected AnchoredJoint2D anchoredJoint2D;

    public AnchoredJoint2D AnchoredJoint2D
    {
        get { return anchoredJoint2D; }
    }

    [SerializeField]
    protected float ropeWindSpeed = 2.5f,
                    ropeSwaySpeed = 2.5f,
                    projectileSpeed = 5.0f;

    [SerializeField]
    protected RopeController ropeControllerPrefab;

    [SerializeField]
    protected NinjaInputManager input;
    public int PlayerNumber
    {
        get { return input.PlayerNum; }
    }

    [SerializeField]
    protected GameObject projectilePrefab;

    [SerializeField]
    protected float verticalTime = 0.5f,
                    verticalRopeLengthTime = 1.0f;

    [SerializeField]
    protected Transform aimingTransform;

    [SerializeField]
    protected string ninjaName;
    public string NinjaName
    {
        get { return ninjaName; }
    }

    [SerializeField]
    protected SpriteRenderer headSprite;
    public Color NinjaColor
    {
        get { return headSprite.color; }
    }

    private float verticalUp, verticalDown;
    private RopeController ropeController,
                           lastRopeController;
    private new Rigidbody2D rigidbody;
    private float grabCooldown;
    private RopeNode currentRopeNode;

    public NinjaState State{ get; set;}

    public void RemoveRopeController()
    {
        ropeController = null;
    }

    public void Killed()
    {
        if (State == NinjaState.Alive)
        {
            gameObject.SetActive(false);
            State = NinjaState.Dead;
        }
    }

    public void SetToJoinable()
    {
        gameObject.SetActive(true);
        State = NinjaState.WaitingToJoin;
    }

    protected void Update()
    {
        switch (State)
        {
            case NinjaState.Alive:
                Roping();
                Attacking();
                Aiming();
                break;
            case NinjaState.Dead:
                break;
            case NinjaState.WaitingToJoin:
                if (input.Jumped)
                {
                    GameManager.Instance.AddPlayer(this);
                    State = NinjaState.WaitingToPlay;
                }
                break;
            case NinjaState.WaitingToPlay:
                break;
        }
    }

    private void Aiming()
    {
        aimingTransform.gameObject.SetActive(input.RightStick.magnitude > 0.1f);
        float angle = Mathf.Atan2(input.InputDevice.RightStickX, input.InputDevice.RightStickY) * 180.0f / Mathf.PI;
        aimingTransform.eulerAngles = new Vector3(0, 0, -angle);
    }

    private void Attacking()
    {
        if (!input.Attacked)
        {
            return;
        }

        Vector2 direction = input.RightStick.normalized;

        if (direction.magnitude < 0.25f)
        {
            return;
        }

        var projectile = Instantiate(projectilePrefab, (Vector2)transform.position + direction, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().AddForce(direction * projectileSpeed, ForceMode2D.Impulse);
    }

    private void Roping()
    {
        if (input.Roped)
        {
            var direction = input.RightStick;
            var rayHits = Physics2D.Raycast(transform.position, direction, 10.0f, 1 << LayerMask.NameToLayer("Ropables"));
            if (rayHits.transform != null)
            {
                Detach();
                ropeController = Instantiate(ropeControllerPrefab, transform.position, Quaternion.identity);
                ropeController.AttachRope(rayHits.point, this);
                anchoredJoint2D.enabled = true;
                rigidbody.AddForce(direction.normalized);
            }
        }
        else if (input.Jumped)
        {
            Detach();
        }

        if (ropeController != null)
        {
            float horizontal = input.LeftStick.x;
            if (horizontal != 0)
            {
                Vector2 ropeDirection = (transform.position - ropeController.LastRopeNode.position).normalized,
                        swingDirection = Quaternion.Euler(0, 0, 90 * Mathf.Sign(horizontal)) * ropeDirection;

                Vector2 force = (ropeDirection + swingDirection) * ropeSwaySpeed;
                rigidbody.AddForceAtPosition(force, transform.position + new Vector3(0, -1));
            }
        }
        else
        {
            grabCooldown += Time.deltaTime;
        }

        if (currentRopeNode != null || (ropeController != null && ropeController.IsAttached))
        {
            float vertical = input.LeftStick.y;
            if (vertical != 0 && Mathf.Abs(vertical) > 0.75f)
            {
                float check,
                      timeCheck = currentRopeNode != null ? verticalTime : verticalRopeLengthTime;
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
                if (check >= timeCheck)
                {
                    VerticalInput(vertical);
                    verticalUp = 0.0f;
                    verticalDown = 0.0f;
                }
            }
        }
    }

    private void Detach()
    {
        if (ropeController != null)
        {
            lastRopeController = ropeController;
            ropeController.DetachRope();
            ropeController = null;
            grabCooldown = -0.25f;
        }
        currentRopeNode = null;
    }

    private void VerticalInput(float vericalAxis)
    {
        if (currentRopeNode != null)
        {
            var next = currentRopeNode.RopeController.GetNextRopeNode(currentRopeNode, vericalAxis);
            if (next != null)
            {
                AttachToRopeNode(next);
            }
        }
        else
        {
            ropeController.RopeLengthChange(vericalAxis);
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        RopeNode ropeNode = other.gameObject.GetComponent<RopeNode>();
        if (input.IsJumping && ropeNode != null && ropeNode.RopeController != null && (lastRopeController != ropeNode.RopeController || grabCooldown > 0))
        {
            AttachToRopeNode(ropeNode);
        }
    }

    private void AttachToRopeNode(RopeNode ropeNode)
    {
        ropeController = ropeNode.RopeController;
        ropeController.AttachNinja(this);
        anchoredJoint2D.connectedBody = ropeNode.Rigidbody2D;
        anchoredJoint2D.enabled = true;
        currentRopeNode = ropeNode;
    }

    protected void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        if (input.InputDevice == null)
        {
            Destroy(gameObject);
        }
        else
        {
            State = NinjaState.WaitingToJoin;
        }
    }
}
