using UnityEngine;

namespace Assets.Scripts
{
    public class BulletColliderController : MonoBehaviour {

        void OnTriggerStay(Collider other)
        {
            Debug.Log("Entered");
            switch (other.tag)
            {
                case "Player":
                    other.GetComponent<PlayerController>().HitByBullet();
                    gameObject.SetActive(false);
                    break;
                case "Enemy":
                    gameObject.SetActive(false);
                    break;
                case "Wall":
                    gameObject.SetActive(false);
                    break;
            }
        }
    }
}
