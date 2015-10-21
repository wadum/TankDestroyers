using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class MineController : NetworkBehaviour
    {

        public ParticleSystem MineExplosion;

        public AudioSource placing;
        public AudioSource ExplosionSound;
        [SyncVar]
        public short Owner;

        void Start()
        {
            placing.Play();
        }

        void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "Player":
                    other.gameObject.GetComponent<PlayerController>().HitByMine(gameObject, Owner);
                    break;
                case "Enemy":
                    StartCoroutine(Hit());
                    break;
            }
        }

        [ClientRpc]
        public void RpcExplode()
        {
            StartCoroutine(Hit());
        }

        IEnumerator Hit()
        {
            MineExplosion.Play();
            ExplosionSound.Play();
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }
    }
}
