using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField]
    protected AudioClip sfxClip;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        OnCollision(other.gameObject);
    }
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        OnCollision(other.gameObject);
    }

    protected virtual void OnCollision(GameObject other)
    {
        var ropeNode = other.GetComponent<RopeNode>();
        if (ropeNode)
        {
            ropeNode.CutRope();
        }
        var ninja = other.GetComponent<NinjaController>();
        if (ninja != null && ninja.IsKillable)
        {
            Vector2 position = ninja.transform.position;
            ninja.Killed();
            OnKilledNinja(ninja);
            GameManager.Instance.NinjaKilled(ninja, position);
        }
    }

    protected virtual void OnKilledNinja(NinjaController ninja)
    {
        if (audioSource != null && sfxClip != null)
        {
            audioSource.PlayOneShot(sfxClip);
        }
    }
}
