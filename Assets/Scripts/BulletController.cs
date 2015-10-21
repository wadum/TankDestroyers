using System.Collections;
using Assets.Scripts.AI;
using UnityEngine;

namespace Assets.Scripts
{
    public class BulletController : MonoBehaviour
    {

        public ParticleSystem Explosion;
        public short Owner;
        public float BotDamage = 5;
        public float PlayerDamage = 5;
        
        private bool _moving;

        void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "Player":
                    if (Owner == other.GetComponent<PlayerController>().networkId) return;
                    other.GetComponent<PlayerController>().HitByBullet(PlayerDamage, Owner);
                    StartCoroutine(BulletHit());
                    break;
                case "Enemy":
                    if (Owner == other.GetComponent<EnemyMovement>().NetworkId) return;
                    other.GetComponent<EnemyMovement>().HitByBullet(BotDamage, Owner);
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

        public void FireBullet(Vector3 direction, float bulletSpeed, short owner)
        {
            Owner = owner;
            StartCoroutine(MoveBullet(direction, bulletSpeed));
        }

        public IEnumerator MoveBullet(Vector3 direction, float bulletSpeed)
        {
            _moving = true;
            while (_moving)
            {
                transform.Translate(direction * bulletSpeed * Time.deltaTime);
                yield return null;
            }
        }

    }
}
