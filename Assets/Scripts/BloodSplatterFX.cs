using System.Collections;
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
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        if (numEnter > 0)
        {
            foreach (var item in enter)
            {
                var blood = Instantiate(bloodSplatter, transform.position + item.position,  Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
                if (bloodSpatterParent == null)
                {
                    bloodSpatterParent = new GameObject("BloodSpatterParent").transform;
                }
                blood.transform.parent = bloodSpatterParent;
            }
        }
    }
}
