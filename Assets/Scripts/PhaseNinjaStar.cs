using UnityEngine;

public class PhaseNinjaStar :NinjaStarController
{
    protected override void PostCollision(GameObject other)
    {
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
       OnCollision(other.gameObject);
       StopSlowMo();
    }
}
