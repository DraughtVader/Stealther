using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField]
    protected BloodSplatterEffect splatterEffect;

    [SerializeField]
    protected PregameManager pregameManager;

    [SerializeField]
    protected NinjaController ninja;

    protected void OnTriggerEnter2D(Collider2D other)
    {
        var star = other.GetComponent<NinjaStarController>();
        if (star != null)
        {
            OnDestroyed(star);
        }
    }

    private void OnDestroyed(Hazard star)
    {
        var spaltter = Instantiate(splatterEffect, transform.position, Quaternion.identity);
        spaltter.SetUp(ninja.NinjaColor, star, transform.position);
        pregameManager.PlayerReady(ninja);
        gameObject.SetActive(false);
    }

    private void Start()
    {
        GetComponentInChildren<Animation>().Play();
    }
}
