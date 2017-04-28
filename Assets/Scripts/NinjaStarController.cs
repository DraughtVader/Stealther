using System.Collections.Generic;
using UnityEngine;

public class NinjaStarController : ProjectileController
{
    [SerializeField]
    protected float rotationSpeed = 1.0f,
                    slowMoDistanceCheck = 0.5f;

    [SerializeField]
    protected Vector2 slowMoRange = new Vector2(0.1f, 0.5f);

    public NinjaController Thrower { get; set; }


    protected override void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, rotationSpeed);

        SlowMo();
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
        var ropeNode = other.gameObject.GetComponent<RopeNode>();
        if (ropeNode)
        {
            ropeNode.CutRope();
        }
        var ninja = other.gameObject.GetComponent<NinjaController>();
        if (ninja != null && ninja.State == NinjaController.NinjaState.Alive)
        {
            ninja.Killed();
            GameManager.Instance.NinjaKilled(ninja, transform.position);
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

    private void OnDestroy()
    {
        if (SlowMoController.Instance.CanDoSlowMo(gameObject))
        {
            Time.timeScale = 1;
            SlowMoController.Instance.Current = null;
        }
    }

}
