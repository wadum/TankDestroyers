using UnityEngine;

namespace Assets.Scripts
{
    public class HealthIndicator : MonoBehaviour
    {
        public ParticleSystem Smoke;
        public ParticleSystem Steam;

        public void SetHealth(float health)
        {
            Smoke.enableEmission = health <= 33;
            Steam.enableEmission = health > 33 && health <= 66;
        }
    }
}
