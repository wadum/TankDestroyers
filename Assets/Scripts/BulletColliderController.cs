using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class BulletColliderController : MonoBehaviour
    {

        public ParticleSystem Explosion;

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("Entered");
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
            Explosion.Play();
            GetComponentInChildren<MeshRenderer>().enabled = false;
            yield return new WaitForSeconds(1);
            GetComponentInChildren<MeshRenderer>().enabled = true;
            gameObject.SetActive(false);  
        }
    }
}
