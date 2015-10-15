using System.Collections.Generic;
using Assets.Scripts.Variables;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class EnemyMovement : MonoBehaviour
    {

        public int MsBetweenShots;
        public List<Transform> Waypoints;

        [Space(10)]
        [Header("Tank Shot")]
        public AudioClip TankShotSound;
        public float TankShotVolume = 0.3f;

        private NavMeshAgent _nav;
        private ChaseState _chaseState;
        private PatrolState _patrolState;
        private IState _currentState;
        private AudioSource _shotSource;

        void Start ()
        {
            _nav = GetComponent<NavMeshAgent>();
            _shotSource = gameObject.AddComponent<AudioSource>();
            _shotSource.clip = TankShotSound;
            _shotSource.loop = false;
            _shotSource.playOnAwake = false;
            _shotSource.volume = TankShotVolume;
            _shotSource.minDistance = 5;
            _shotSource.maxDistance = 20;
            _shotSource.rolloffMode = AudioRolloffMode.Linear;
            _shotSource.spatialBlend = 1;
            _chaseState = new ChaseState(_nav, MsBetweenShots, _shotSource);
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
