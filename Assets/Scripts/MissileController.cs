using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.AI;

namespace Assets.Scripts
{
    public class MissileController : MonoBehaviour
    {

        public ParticleSystem Explosion;
        public AudioSource Shoot;
        public AudioSource Hit;

        private bool _moving = true;

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "Player":
                    if(other.GetComponent<PlayerController>().HitByBullet())
                        StartCoroutine(ExplodeMissile());
                    break;
                case "Enemy":
                    other.transform.parent.gameObject.GetComponent<EnemyMovement>().HitByBullet();
                    StartCoroutine(ExplodeMissile());
                    break;
                case "Wall":
                    StartCoroutine(ExplodeMissile());
                    break;
            }
        }

        private IEnumerator ExplodeMissile()
        {
            _moving = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;
            Explosion.Play();
            Shoot.Stop();
            Hit.Play();
            GameObject missileBody = transform.GetChild(0).gameObject;
            missileBody.SetActive(false);
            yield return new WaitForSeconds(2);
            Hit.Stop();
            missileBody.SetActive(true);
            gameObject.GetComponent<BoxCollider>().enabled = true;
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