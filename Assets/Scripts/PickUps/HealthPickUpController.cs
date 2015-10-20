using System.Collections;
using Assets.Scripts.Variables;
using UnityEngine;

namespace Assets.Scripts.PickUps
{
    public class HealthPickUpController : MonoBehaviour, IPickUpController
    {

        public int Amount = 1;
        public AudioSource PickUpSound;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != Constants.Tags.Player) return;
            if (!other.gameObject.GetComponent<PlayerController>().ResetPickUp(gameObject)) return;
            other.gameObject.GetComponent<PlayerController>().AddHealth(Amount);
        }

        public IEnumerator PickUp()
        {
            PickUpSound.Play();
            gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            yield return new WaitForSeconds(PickUpSound.clip.length);
            yield return new WaitForSeconds(20);
            gameObject.GetComponent<BoxCollider>().enabled = true;
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
