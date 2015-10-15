using Assets.Scripts.Variables;
using UnityEngine;

namespace Assets.Scripts
{
    public class EnemyMovement : MonoBehaviour
    {
        private NavMeshAgent _nav;

        // Use this for initialization
        void Start ()
        {
            _nav = GetComponent<NavMeshAgent>();
        }

        void OnTriggerStay(Collider other)
        {
            if (other.tag != Constants.Tags.Player) return;
            _nav.destination = other.gameObject.transform.position;
        }
    }
}
