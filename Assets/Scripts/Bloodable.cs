using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloodable : MonoBehaviour
{
    [SerializeField]
    protected bool setParent;

    public bool SetParent
    {
        get { return setParent; }
    }
}
