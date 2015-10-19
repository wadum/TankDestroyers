using Assets.Scripts.Variables;
using UnityEngine;

namespace Assets.Scripts.PickUps
{
    public class HealthPickUpController : MonoBehaviour {

        public int Amount = 1;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != Constants.Tags.Player) return;
            other.gameObject.GetComponent<PlayerController>().AddHealth(Amount);
            Destroy(gameObject);
        }
    }
}
