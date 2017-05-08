using System.Collections.Generic;
using UnityEngine;

public class BloodSplatterFX : MonoBehaviour
{
    [SerializeField]
    protected ParticleSystem ps;

    [SerializeField]
    protected GameObject bloodSplatter;

    private List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

    public static Transform bloodSpatterParent;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
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
            var blood = Instantiate(bloodSplatter, position,  Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
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
    }
}
