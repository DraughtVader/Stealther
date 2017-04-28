using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SecretNinjaStartsController : MonoBehaviour
{
    [SerializeField]
    protected ProjectileLauncherController[] launchers;

    [SerializeField]
    protected Vector2 frequencyRange = new Vector2(5, 7);

    private DateTime fireTime;

    private void Start()
    {
        SetUpTimes();
    }

    private void Update()
    {
        if (DateTime.Now >= fireTime)
        {
            var launcher = launchers[Random.Range(0, launchers.Length)];
            launcher.FireProjectile();
            SetUpTimes();
        }
    }

    private void SetUpTimes()
    {
        fireTime = DateTime.Now.AddSeconds(Random.Range(frequencyRange.x, frequencyRange.y));
    }

}
