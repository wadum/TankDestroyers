using UnityEngine;
using System.Collections;
using System;

namespace Assets.Scripts
{
    public class MissileController : MonoBehaviour
    {

        public ParticleSystem Explosion;
        public AudioSource Shoot;

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
            Shoot.Stop();
            GameObject missileBody = transform.GetChild(0).gameObject;
            missileBody.SetActive(false);
            yield return new WaitForSeconds(1);
            missileBody.SetActive(true);
            gameObject.SetActive(false);
        }

        public IEnumerator MoveMissile(Vector3 direction, float missileSpeed)
        {
            _moving = true;
            Shoot.Play();
            while (_moving)
            {
                transform.Translate(direction * missileSpeed);
                yield return null;
            }
        }

        public void FireMissile(Vector3 direction, float missileSpeed)
        {
            Shoot.Play();
            StartCoroutine(MoveMissile(direction, missileSpeed));
        }
    }
}