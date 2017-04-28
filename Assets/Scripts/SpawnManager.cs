using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    protected Transform[] points;

    public Transform[] SpawnPoints
    {
        get
        {
            points.ShuffleInPlace();
            return points;
        }
    }
}
