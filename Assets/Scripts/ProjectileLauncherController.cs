using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncherController : MonoBehaviour
{
    [SerializeField]
    protected GameObject projectilePrefab;

    [SerializeField]
    protected Vector2 direction;

    [SerializeField]
    protected float projectileSpeed;

    private new Animation animation;

    public void FireProjectile()
    {
        animation.Play("StarLaunch");
    }

    private void Fire()
    {
        var projectile = Instantiate(projectilePrefab, transform.position + (Vector3)direction * 0.5f, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().AddForce(direction * projectileSpeed, ForceMode2D.Impulse);
    }

    private void Start()
    {
        animation = GetComponent<Animation>();
    }
}
