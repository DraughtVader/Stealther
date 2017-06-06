using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceApplyingCollider : MonoBehaviour
{
    public enum Type
    {
        NoExtraForce,
        CenterX,
        CenterY,
        Center
    }

    [SerializeField]
    protected Vector2 force;

    [SerializeField]
    protected float centeringForce;

    [SerializeField]
    protected Type type;
    
    private void OnTriggerStay2D(Collider2D other)
    {
        var ninja = other.GetComponent<NinjaController>();
        if (ninja == null)
        {
            return;
        }
        Vector2 centerVector = (Vector2)(transform.position - other.transform.position).normalized,
                forceToApply = force;
            
        switch (type)
        {
            case Type.NoExtraForce:
                break;
            case Type.CenterX:
                forceToApply.x += centerVector.x * centeringForce;
                break;
            case Type.CenterY:
                forceToApply.y += centerVector.y * centeringForce;
                break;
            case Type.Center:
                forceToApply += centerVector * centeringForce;
                break;
        }
            
        ninja.Rigidbody.AddForce(forceToApply);
    }
}
