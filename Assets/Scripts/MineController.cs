using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class MineController : MonoBehaviour
    {

        public ParticleSystem MineExplosion;

        void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "Player":
                    StartCoroutine(Hit());
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().HitByMine();
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
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }
    }
}
