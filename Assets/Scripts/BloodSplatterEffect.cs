using System.Collections.Generic;
using UnityEngine;

public class BloodSplatterEffect : MonoBehaviour
{
    [SerializeField]
    protected int particleNumber = 50;

    [SerializeField]
    protected BloodParticle bloodParticle;

    public void SetUp(Color color, Hazard hazard, Vector2 ninjaPosition)
    {
        for (var i = 0; i < particleNumber; i++)
        {
            var data = GetBloodParticleData(hazard, ninjaPosition);
            Vector2 particleVelocity = data.Velocity;
            var particle = Instantiate(bloodParticle, data.Position, Quaternion.identity);
            particle.SetUp(color, particleVelocity);
        }
        
        var saw = hazard as RotatingHazard;
        if (saw != null)
        {
            for (var i = 0; i < 10; i++)
            {
                var position = (Vector2)saw.transform.position + (Random.insideUnitCircle * saw.Radius);
                var particle = Instantiate(bloodParticle, position, Quaternion.identity);
                particle.SetUp(color, Vector2.zero);
            }
        }
    }

    private BloodParticleData GetBloodParticleData(Hazard hazard, Vector2 ninjaPosition)
    {
        var star = hazard as NinjaStarController;
        if (star != null)
        {
            var data = new BloodParticleData(Quaternion.Euler(0,0,Random.Range(-10.0f, 10.0f)) * (star.Rigidbody2D.velocity * Random.Range(0.5f, 0.75f)), ninjaPosition);
            return data;
        }
        var saw = hazard as RotatingHazard;
        if (saw != null)
        {
            float speed = saw.RotationSpeed;
            float angle = Random.Range(0.0f, 359.0f);
            Vector2 velocity = Quaternion.Euler(0, 0, angle) * new Vector2(speed * Random.Range(0.125f, 1.25f), 0);
            Vector2 position = Quaternion.Euler(0, 0, angle) * new Vector2(saw.Radius + 0.1f, 0) + saw.transform.position;
            return new BloodParticleData(velocity, position);
        }
        return new BloodParticleData(Vector2.zero, Vector2.zero);
    }

    public class BloodParticleData
    {
        public Vector2 Velocity;
        public Vector2 Position;

        public BloodParticleData(Vector2 velocity, Vector2 position)
        {
            Velocity = velocity;
            Position = position;
        }
    }
}
