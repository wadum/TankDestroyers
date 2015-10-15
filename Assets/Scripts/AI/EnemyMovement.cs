using System.Collections.Generic;
using Assets.Scripts.Variables;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class EnemyMovement : MonoBehaviour
    {

        public int MsBetweenShots;
        public List<Transform> Waypoints;

        private NavMeshAgent _nav;
        private ChaseState _chaseState;
        private PatrolState _patrolState;
        private IState _currentState;

        void Start ()
        {
            _nav = GetComponent<NavMeshAgent>();
            _chaseState = new ChaseState(_nav, MsBetweenShots);
            _patrolState = new PatrolState(_nav, Waypoints);
        }

        void Update()
        {
            _currentState.ExecuteState();
            _currentState = _patrolState;
        }

        void OnTriggerStay(Collider other)
        {
            if(other.tag != Constants.Tags.Player)
                return;
            
            _chaseState.Target = other.gameObject.transform;
            _currentState = _chaseState;
        }
    }
}
