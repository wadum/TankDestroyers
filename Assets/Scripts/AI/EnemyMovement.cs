using System.Collections.Generic;
using Assets.Scripts.AI;
using Assets.Scripts.Variables;
using UnityEngine;

namespace Assets.Scripts
{
    public class EnemyMovement : MonoBehaviour
    {
        public List<Transform> Waypoints; 
        private NavMeshAgent _nav;
        private ChaseState _chaseState;
        private PatrolState _patrolState;
        private IState _currentState;

        // Use this for initialization
        void Start ()
        {
            _nav = GetComponent<NavMeshAgent>();
            _chaseState = new ChaseState(_nav);
            _patrolState = new PatrolState(_nav, Waypoints);
        }

        void Update()
        {
            _currentState.ExecuteState();
            _currentState = _patrolState;
        }

        void OnTriggerStay(Collider other)
        {
            _chaseState.Target = other.gameObject.transform;
            _currentState = _chaseState;
        }
    }
}
