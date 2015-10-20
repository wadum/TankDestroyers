using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class MineController : NetworkBehaviour
    {

        public ParticleSystem MineExplosion;

        public AudioSource placing;

        void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "Player":
                    if(other.gameObject.GetComponent<PlayerController>().HitByMine())
                        StartCoroutine(Hit());
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
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }
    }
}
