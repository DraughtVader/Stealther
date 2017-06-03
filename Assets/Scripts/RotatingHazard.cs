using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingHazard : Hazard
{
    [SerializeField]
    protected float rotationSpeed = 1;

    public float RotationSpeed
    {
        get { return rotationSpeed; }
    }

    public float Radius
    {
        get { return GetComponent<CircleCollider2D>().radius * transform.lossyScale.x; }
    }

    protected void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, rotationSpeed);
    }
}
