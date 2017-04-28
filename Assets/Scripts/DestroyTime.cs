using System;
using UnityEngine;

public class DestroyTime : MonoBehaviour
{
    [SerializeField]
    protected float lifeTime = 2.0f;

    private DateTime startTime;

    private void Start()
    {
        startTime = DateTime.Now;
    }

	private void Update ()
	{
	    if (DateTime.Now > startTime.AddSeconds(lifeTime))
	    {
	        Destroy(gameObject);
	    }
	}
}