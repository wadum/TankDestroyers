using System.Collections;
using Assets.Scripts.Variables;
using UnityEngine;

namespace Assets.Scripts.PickUps
{
    public class MinePickUpController : MonoBehaviour
    {

        public int Amount = 1;
        public AudioSource PickUpSound;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != Constants.Tags.Player) return;
            other.gameObject.GetComponent<PlayerController>().CmdAddMines(Amount);
            gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            StartCoroutine(PickUp());
        }

        IEnumerator PickUp()
        {
            PickUpSound.Play();
            yield return new WaitForSeconds(PickUpSound.clip.length);
            Destroy(gameObject);
        }
    }
}
