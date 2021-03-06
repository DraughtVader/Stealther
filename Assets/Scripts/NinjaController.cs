﻿using System;
using UnityEngine;

public enum NinjaState
{
    NotPlaying, WaitingToJoin, WaitingToPlay, Alive, Dead, Stunned, Pregame, Ready, Spawning
}

public class NinjaController : MonoBehaviour
{
    public enum AmmoType
    {
        Normal, Phase, Multiple, Ricochet
    }

    public enum ShieldType
    {
        None, Phase, Block, Ricochet
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
                    projectileSpeed = 5.0f,
                    stunDuration = 1.0f,
                    minVelocityForStun = 7.0f;

    [SerializeField]
    protected RopeController ropeControllerPrefab;

    [SerializeField]
    protected NinjaInputManager input;
    public int PlayerNumber
    {
        get { return input.PlayerNum; }
    }

    [SerializeField]
    protected GameObject projectilePrefab,
        phaseProjectilePrefab;

    [SerializeField]
    protected float verticalTime = 0.5f,
                    verticalRopeLengthTime = 1.0f,
                    totalThrowStamina = 5,
                    throwStaminaDrain = 1,
                    staminaRecoverySpeed = 0.25f;

    [SerializeField]
    protected Transform aimingTransform;


    [SerializeField]
    protected ParticleSystem stunPfx,
        phasePfx,
        phaseShieldPfx,
        fatiguedPfx;

    [SerializeField]
    protected AudioClip rope,
        throwing,
        grunt,
        fatigued;

    public string NinjaName
    {
        get { return Description.Name; }
    }

    [SerializeField]
    protected SpriteRenderer headSprite,
        hatSprite,
        bodySprite,
        aimingTarget,
        invalidAimingTarget;


    public Color NinjaColor
    {
        get { return Description.Color; }
    }

    public SpriteRenderer HatSprite
    {
        get { return hatSprite; }
    }

    private float verticalUp, verticalDown;
    private RopeController ropeController,
                           lastRopeController;
    private new Rigidbody2D rigidbody;
    public Rigidbody2D Rigidbody
    {
        get { return rigidbody; }
    }
    private float grabCooldown, lastVelocity;
    private RopeNode currentRopeNode;
    private DateTime unstunTime;
    private AmmoType ammoType;
    private int specialAmmoCount;
    private float shieldUpTime;
    private float throwStamina;
    private bool staminaDepleted;
    private AudioSource audioSource;

    public NinjaState State{ get; set;}
    public NinjaDescription Description { get; set; }

    public ShieldType Shield { get; private set; }

    public bool IsKillable
    {
        get
        {
            return (State == NinjaState.Alive || State == NinjaState.Stunned) && Shield == ShieldType.None;
        }
    }

    public bool DestroyProjectileOnHit
    {
        get { return Shield != ShieldType.Phase; }
    }

    public void RemoveRopeController()
    {
        ropeController = null;
    }

    public void Killed()
    {
        if (IsKillable)
        {
            transform.position = new Vector3(100, 100);
            rigidbody.isKinematic = true;
            State = NinjaState.Dead;
            Detach();
        }
    }

    public void Spawn(Vector2 position)
    {
        Rigidbody.velocity = Vector2.zero;
        gameObject.SetActive(true);
        Rigidbody.isKinematic = false;
        State = NinjaState.Alive;
        transform.position = position;
    }

    public void SetToJoinable()
    {
        State = NinjaState.WaitingToJoin;
    }

    protected void Update()
    {
        switch (State)
        {
            case NinjaState.Alive:
            case NinjaState.Ready:
            case NinjaState.Pregame:
                Roping();
                Attacking();
                Aiming();
                Shielding();
                Customisation();
                break;
            case NinjaState.Dead:
                break;
            case NinjaState.WaitingToJoin:
                if (input.Jumped)
                {
                    GameManager.Instance.AddPlayer(this);
                    SetUpColour();
                }
                break;
            case NinjaState.WaitingToPlay:
                break;
            case NinjaState.Stunned:
                if (DateTime.Now >= unstunTime)
                {
                    State = NinjaState.Alive;
                    stunPfx.Stop();
                }
                break;
        }
    }

    private void Customisation()
    {
        if (State == NinjaState.Pregame)
        {
            if (input.PressedRight)
            {
                GameManager.Instance.GetNextDescription(this);
                SetUpColour();
            }
            else if (input.PressedLeft)
            {
                GameManager.Instance.GetPreviousDescription(this);
                SetUpColour();
            }
        }
    }

    private void Shielding()
    {
        if (Shield == ShieldType.None)
        {
            return;
        }
        shieldUpTime -= Time.deltaTime;
        if (shieldUpTime <= 0)
        {
            Shield = ShieldType.None;
            phaseShieldPfx.Stop();
            phaseShieldPfx.Clear();
        }
    }

    protected void LateUpdate()
    {
        lastVelocity = rigidbody.velocity.magnitude;
    }

    private void Aiming()
    {
        aimingTransform.gameObject.SetActive(input.RightStick.magnitude > 0.1f);
        var angle = Mathf.Atan2(input.RightStick.x, input.RightStick.y) * 180.0f / Mathf.PI;
        aimingTransform.eulerAngles = new Vector3(0, 0, -angle);

        if (input.RightStick.magnitude > 0.2f)
        {
            aimingTarget.gameObject.SetActive(true);
            var direction = input.RightStick;
            var rayHits = Physics2D.Raycast(transform.position, direction, 10.0f,
                1 << LayerMask.NameToLayer("Ropables"));
            if (rayHits.transform != null)
            {
                aimingTarget.enabled = true;
                aimingTarget.transform.position = rayHits.point;
                aimingTarget.color = NinjaColor;
            }
            else
            {
                aimingTarget.enabled = false;
                aimingTarget.transform.position = transform.position + (Vector3)(direction.normalized * 10);
                invalidAimingTarget.color = NinjaColor;
            }
            invalidAimingTarget.gameObject.SetActive(rayHits.transform == null);
        }
        else
        {
            aimingTarget.gameObject.SetActive(false);
        }
    }

    private void Attacking()
    {
        throwStamina = Mathf.Clamp(throwStamina + Time.deltaTime * staminaRecoverySpeed, -1, totalThrowStamina);
        if (throwStamina >= totalThrowStamina)
        {
            staminaDepleted = false;
            StopParticleSystem(fatiguedPfx, true);
        }

        if (!input.Attacked || staminaDepleted)
        {
            return;
        }

        Vector2 direction = input.RightStick.normalized;

        if (direction.magnitude < 0.25f)
        {
            return;
        }

        GameObject currentProjectile;
        switch (ammoType)
        {
            case AmmoType.Normal:
                currentProjectile = projectilePrefab;
                break;
            case AmmoType.Phase:
                currentProjectile = phaseProjectilePrefab;
                break;
            default:
                currentProjectile = projectilePrefab;
                break;
        }
        if (ammoType != AmmoType.Normal)
        {
            specialAmmoCount--;
            if (specialAmmoCount == 0)
            {
                ammoType = AmmoType.Normal;
                phasePfx.Stop();
                //TODO reset visual effects
            }
        }


        var projectile = Instantiate(currentProjectile, (Vector2)transform.position + direction, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().AddForce(direction * projectileSpeed, ForceMode2D.Impulse);
        projectile.GetComponent<NinjaStarController>().Thrower = this;
        audioSource.PlayOneShot(throwing);

        throwStamina -= throwStaminaDrain;
        if (throwStamina <= 0)
        {
            staminaDepleted = true;
            fatiguedPfx.Play();
            audioSource.PlayOneShot(fatigued);
        }
    }

    private void Roping()
    {
        if (input.Roped && input.RightStick.magnitude > 0.2f)
        {
            var direction = input.RightStick;
            var rayHits = Physics2D.Raycast(transform.position, direction, 10.0f, 1 << LayerMask.NameToLayer("Ropables"));
            Detach();
            ropeController = Instantiate(ropeControllerPrefab, transform.position, Quaternion.identity);
            audioSource.PlayOneShot(rope);
            if (rayHits.transform != null)
            {
                var targetRigidbody = rayHits.transform.GetComponent<Rigidbody2D>();
                ropeController.AttachRope(rayHits.point, this, targetRigidbody);
                anchoredJoint2D.enabled = true;
                rigidbody.AddForce(direction.normalized);
            }
            else
            {
                var point = (Vector2)transform.position + (direction.normalized * 10);
                ropeController.AttachRope(point, this);
                Destroy(ropeController.LastRopeNode.GetComponent<RopeNode>().gameObject);
                ropeController = null;
                AnchoredJoint2D.enabled = false;
            }
        }
        else if (input.RopeUp )
        {
            Detach();
        }

        if (ropeController != null && ropeController.LastRopeNode != null)
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (lastVelocity >= minVelocityForStun)
        {
            State = NinjaState.Stunned;
            unstunTime = DateTime.Now.AddSeconds(stunDuration);
            stunPfx.Play();
            audioSource.PlayOneShot(grunt);
            Detach();
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

    private void OnRoundStart()
    {
        Detach();
        throwStamina = totalThrowStamina;
        staminaDepleted = false;

        if (State != NinjaState.Alive)
        {
            State = NinjaState.NotPlaying;
            transform.position = new Vector3(100, 100);
            rigidbody.isKinematic = true;
        }
        else
        {
            rigidbody.isKinematic = false;
            rigidbody.velocity = Vector2.zero;
        }
    }

    private void OnRoundEnd()
    {
    }

    private void OnMatchFinished()
    {
        SetToJoinable();
    }
    
    private void OnScoresDisplayed()
    {
        ammoType = AmmoType.Normal;
        Shield = ShieldType.None;
        phasePfx.Stop();
        phaseShieldPfx.Stop();
        fatiguedPfx.Stop();
        transform.position = new Vector3(100, 100);
        rigidbody.isKinematic = true;
    }

    protected void Start()
    {
        State = NinjaState.WaitingToJoin;

        GameManager.Instance.RoundStart += OnRoundStart;
        GameManager.Instance.RoundEnd += OnRoundEnd;
        GameManager.Instance.MatchFinished += OnMatchFinished;
        GameUiManager.Instance.ScoresDisplayed += OnScoresDisplayed;
    }

    protected void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PickUp(PickUp.Type type)
    {
        switch (type)
        {
            case global::PickUp.Type.PhaseStar:
                ammoType = AmmoType.Phase;
                specialAmmoCount = 5;
                phasePfx.Play();
                break;
            case global::PickUp.Type.PhaseShield:
                StartShield(5.0f);
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
    }

    public void StartShield(float upTime)
    {
        Shield = ShieldType.Phase;
        shieldUpTime = upTime;
        phaseShieldPfx.Play();
    }

    private void StopParticleSystem(ParticleSystem particleSys, bool destroyRemainingParticles)
    {
        particleSys.Stop();
        if (destroyRemainingParticles)
        {
            particleSys.Clear();
        }
    }

    public void SetUpBody(AccessoryDescription body)
    {
        bodySprite.sprite = body.Sprite;
        bodySprite.color = NinjaColor;
    }

    public void SetUpColour()
    {
        headSprite.color = aimingTransform.GetComponentInChildren<SpriteRenderer>().color = bodySprite.color = NinjaColor;
    }

    public void InputAssigned()
    {
        if (State != NinjaState.WaitingToJoin)
        {
            return;
        }
        GameManager.Instance.AddPlayer(this);
        headSprite.color = aimingTransform.GetComponentInChildren<SpriteRenderer>().color = NinjaColor;
    }
}
