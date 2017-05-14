using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField]
    protected GameObject splatterPfx;

    [SerializeField]
    protected PregameManager pregameManager;

    [SerializeField]
    protected NinjaController ninja;

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NinjaStarController>() != null)
        {
            OnDestroyed();
        }
    }

    private void OnDestroyed()
    {
        var splatter = Instantiate(splatterPfx, transform.position, Quaternion.identity);
        pregameManager.PlayerReady(ninja);
        gameObject.SetActive(false);
    }

    private void Start()
    {
        GetComponentInChildren<Animation>().Play();
    }
}
