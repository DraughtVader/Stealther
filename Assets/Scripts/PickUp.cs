using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum Type
    {
        PhaseStar, MultiStar, PhaseShield
    }

    [SerializeField]
    protected Type type;

    private PickUpSpawner spawner;

    public void AssignSpawner(PickUpSpawner sickUpSpawner)
    {
        spawner = sickUpSpawner;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var ninja = other.GetComponent<NinjaController>();
        if (ninja != null)
        {
            ninja.PickUp(type);
            spawner.PickUpUsed(this);
            Destroy();
        }
    }

    private void OnRoundStart()
    {
        Destroy();
    }

    private void Start()
    {
        GameManager.Instance.RoundStart += OnRoundStart;
        GameManager.Instance.MatchFinished += Destroy;

    }

    private void Destroy()
    {
        Destroy(gameObject);
        GameManager.Instance.RoundStart -= OnRoundStart;
        GameManager.Instance.MatchFinished -= Destroy;
    }

}
