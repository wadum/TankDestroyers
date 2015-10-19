using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class BulletController : MonoBehaviour
    {

        public ParticleSystem Explosion;

        private bool _moving;

        void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "Player":
                    other.GetComponent<PlayerController>().HitByBullet();
                    StartCoroutine(BulletHit());
                    break;
                case "Enemy":
                    StartCoroutine(BulletHit());
                    break;
                case "Wall":
                    StartCoroutine(BulletHit());
                    break;
            }
        }

        IEnumerator BulletHit()
        {
            _moving = false;
            gameObject.GetComponent<SphereCollider>().enabled = false;
            Explosion.Play();
            GetComponentInChildren<MeshRenderer>().enabled = false;
            yield return new WaitForSeconds(1);
            GetComponentInChildren<MeshRenderer>().enabled = true;
            gameObject.GetComponent<SphereCollider>().enabled = true;
            gameObject.SetActive(false);  
        }

        public void FireBullet(Vector3 direction, float bulletSpeed)
        {
            StartCoroutine(MoveBullet(direction, bulletSpeed));
        }

        public IEnumerator MoveBullet(Vector3 direction, float bulletSpeed)
        {
            _moving = true;
            while (_moving)
            {
                transform.Translate(direction * bulletSpeed);
                yield return null;
            }
        }

    }
}
