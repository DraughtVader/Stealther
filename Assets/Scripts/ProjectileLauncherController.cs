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

    protected void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.Q))
        {
            var projectile = Instantiate(projectilePrefab, transform.position + (Vector3)direction * 0.5f, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().AddForce(direction * projectileSpeed, ForceMode2D.Impulse);
        }	
	}
}
