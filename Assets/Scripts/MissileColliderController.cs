using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class MissileColliderController : MonoBehaviour
    {

        public ParticleSystem Explosion;

        private bool _moving = true;

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "Player":
                    other.GetComponent<PlayerController>().HitByBullet();
                    StartCoroutine(MissileHit());
                    break;
                case "Enemy":
                    StartCoroutine(MissileHit());
                    break;
                case "Wall":
                    StartCoroutine(MissileHit());
                    break;
            }
        }

        private IEnumerator MissileHit()
        {
            _moving = false;
            Explosion.Play();
            GameObject missileBody = transform.GetChild(0).gameObject;
            missileBody.SetActive(false);
            yield return new WaitForSeconds(1);
            missileBody.SetActive(true);
            gameObject.SetActive(false);
        }

        public IEnumerator MoveMissile(Vector3 direction, float missileSpeed)
        {
            _moving = true;
            while (_moving)
            {
                transform.Translate(direction * missileSpeed);
                yield return null;
            }
        }
    }
}