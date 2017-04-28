using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SlowMoController : MonoBehaviour
{
    public static SlowMoController Instance;

    public GameObject Current { get; set; }

    public bool CanDoSlowMo(GameObject gO)
    {
        return Current == null || Current == gO;
    }

    protected virtual void Awake()
    {
        Instance = this;
    }

    protected virtual void OnDestroy()
    {
        Instance = null;
    }
}
