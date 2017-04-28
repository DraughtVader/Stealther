using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingHazard : Hazard
{
    [SerializeField]
    protected float rotationSpeed = 1;

    protected void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, rotationSpeed);
    }
}
