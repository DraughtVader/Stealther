using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BloodSplatterFX : MonoBehaviour
{
    [SerializeField]
    protected ParticleSystem ps;

    [SerializeField]
    protected GameObject bloodSplatter;

    [SerializeField]
    protected Vector2 boxSize,
        boxOffset;

    private List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

    public static Transform bloodSpatterParent;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        var colliders = Physics2D.OverlapBoxAll((Vector2)transform.position + boxOffset, boxSize, 0.0f);
        int length = colliders.Length,
            count = 0;

        var closest = colliders.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).ToList();
        for (var i = 0; i < length; i++)
        {
            if (closest[i].GetComponent<Bloodable>())
            {
                ps.trigger.SetCollider(count++, closest[i]);
            }
        }
    }

    private void OnParticleTrigger()
    {
        // get the particles which matched the trigger conditions this frame
        var numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        if (numEnter <= 0)
        {
            return;
        }
        foreach (var item in enter)
        {
            Vector2 position = transform.position + item.position;
            var hit = Physics2D.OverlapPoint(position);
            if (hit == null)
            {
                continue;
            }
            var bloodable = hit.GetComponent<Bloodable>();
            if (bloodable == null)
            {
                return;
            }
            var blood = Instantiate(bloodSplatter, position,  Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
            blood.GetComponent<SpriteRenderer>().sortingOrder =
                bloodable.GetComponent<SpriteRenderer>().sortingOrder + 1;
            if (bloodable.SetParent)
            {
                blood.transform.parent = (hit.transform);
            }
            else
            {
                if (bloodSpatterParent == null)
                {
                    bloodSpatterParent = new GameObject("BloodSpatterParent").transform;
                }
                blood.transform.parent = bloodSpatterParent;
            }
        }
        var particles = new ParticleSystem.Particle[ps.particleCount];
        ps.GetParticles(particles);
        var list = particles.ToList();
        foreach (var particle in enter)
        {
            list.Remove(particle);
        }
        ps.SetParticles(list.ToArray(), list.Count);
    }

    public static void DestroyAll()
    {
        if (bloodSpatterParent == null)
        {
            return;
        }
        Destroy(bloodSpatterParent.gameObject);
        bloodSpatterParent = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + (Vector3)boxOffset, boxSize);
    }
}
