using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        OnCollision(other.gameObject);
    }
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        OnCollision(other.gameObject);
    }

    private void OnCollision(GameObject other)
    {
        var ropeNode = other.GetComponent<RopeNode>();
        if (ropeNode)
        {
            ropeNode.CutRope();
        }
        var ninja = other.GetComponent<NinjaController>();
        if (ninja != null && ninja.State == NinjaController.NinjaState.Alive)
        {
            ninja.Killed();
            GameManager.Instance.NinjaKilled(ninja, transform.position);
        }
    }
}
