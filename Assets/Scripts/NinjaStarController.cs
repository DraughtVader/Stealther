using UnityEngine;

public class NinjaStarController : Hazard
{
    [SerializeField]
    protected float rotationSpeed = 1.0f,
                    slowMoDistanceCheck = 0.5f;

    [SerializeField]
    protected Vector2 slowMoRange = new Vector2(0.1f, 0.5f);

    [SerializeField]
    protected GameObject collisionPfxPrefab;

    public NinjaController Thrower { get; set; }

    public Rigidbody2D Rigidbody2D { get; private set; }


    protected void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, rotationSpeed);
        SlowMo();
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
        PostCollision(other.gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        PostCollision(other.gameObject);
    }

    protected override void NinjaKilled(NinjaController killedNinja)
    {
        Vector2 position = killedNinja.transform.position;
        OnKilledNinja(killedNinja);
        killedNinja.Killed();
        GameManager.Instance.NinjaKilled(killedNinja, position, Thrower);
    }

    private void SlowMo()
    {
        var ninja = GameManager.Instance.GetClosestNinja(transform.position);
        if (ninja == null || ninja == Thrower || !SlowMoController.Instance.CanDoSlowMo(gameObject))
        {
            return;
        }
        var distance = Vector2.Distance(transform.position, ninja.transform.position);
        if (distance <= slowMoDistanceCheck)
        {
            Time.timeScale = slowMoRange.x + (slowMoRange.y - slowMoRange.x) * distance / slowMoDistanceCheck;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
            SlowMoController.Instance.Current = gameObject;
        }
        else
        {
            Time.timeScale = 1;
            SlowMoController.Instance.Current = null;
        }
    }

    protected virtual void PostCollision(GameObject other)
    {
        StopSlowMo();
        var star = other.GetComponent<NinjaStarController>();
        if (star != null && collisionPfxPrefab != null)
        {
            var position = new Vector3((other.transform.position.x + transform.position.x) * 0.5f, (other.transform.position.y + transform.position.y) * 0.5f);
            Instantiate(collisionPfxPrefab, position, Quaternion.identity);
        }

        var ropeNode = other.GetComponent<RopeNode>();
        if (ropeNode != null)
        {
            return;
        }
        
        var saw = other.GetComponent<RotatingHazard>();
        if (saw != null)
        {
            Vector2 velocity = Rigidbody2D.velocity,
                    tangent = Quaternion.Euler(0, 0, 90) * (transform.position - other.transform.position);
            var angle = Vector2.Angle(velocity, tangent);
            Vector2 newVelocity = Quaternion.Euler(0, 0, 360 - 2 * angle) * velocity;
            Rigidbody2D.velocity = newVelocity * 1.25f;
            var position = new Vector3((other.transform.position.x + transform.position.x) * 0.5f, (other.transform.position.y + transform.position.y) * 0.5f);
            Instantiate(collisionPfxPrefab, position, Quaternion.identity);
            return;
        }

        var ninja = other.GetComponent<NinjaController>();
        if (ninja == null || ninja.DestroyProjectileOnHit)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        StopSlowMo();
    }

    protected virtual void StopSlowMo()
    {
        if (SlowMoController.Instance == null || !SlowMoController.Instance.CanDoSlowMo(gameObject))
        {
            return;
        }
        Time.timeScale = 1;
        SlowMoController.Instance.Current = null;
    }

    protected override void Start()
    {
        base.Start();
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }
}
