using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class MineController : MonoBehaviour
    {

        public ParticleSystem MineExplosion;

        public AudioSource PlacingSound;
        public AudioSource ExplodingSound;

        void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "Player":
                    StartCoroutine(Hit());
                    other.gameObject.GetComponent<PlayerController>().HitByMine();
                    break;
                case "Enemy":
                    StartCoroutine(Hit());
                    break;
            }
        }

        IEnumerator Hit()
        {
            MineExplosion.Play();
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            ExplodingSound.Play();
            yield return new WaitForSeconds(ExplodingSound.clip.length);
            Destroy(gameObject);
        }

        public void Place(Vector3 origin, Vector3 direction)
        {
            transform.position = origin;
            transform.Translate(direction * -3.5f);
            transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
            PlacingSound.Play();
        }
    }
}
