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
    private bool active;

    private void Start()
    {
        GameManager.Instance.RoundStart += SetUpTimes;
        GameManager.Instance.RoundEnd += StopProjectiles;
    }

    private void Update()
    {
        if (active && DateTime.Now >= fireTime)
        {
            if (GameManager.Instance.GameState == GameManager.State.Playing)
            {
                var launcher = launchers[Random.Range(0, launchers.Length)];
                launcher.FireProjectile();
            }
            SetUpTimes();
        }
    }

    private void SetUpTimes()
    {
        fireTime = DateTime.Now.AddSeconds(Random.Range(frequencyRange.x, frequencyRange.y));
        active = true;
    }

    private void StopProjectiles()
    {
        active = false;
    }

}
