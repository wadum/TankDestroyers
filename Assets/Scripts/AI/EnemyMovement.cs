using System.ComponentModel;
using System.Linq;
using Assets.Scripts.Variables;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.AI
{
    public class EnemyMovement : NetworkBehaviour
    {

        public int MsBetweenShots;
        public int NumberOfWaypoints;

        [Space(10)]
        [Header("Tank Shot")]
        public AudioClip TankShotSound;
        public float TankShotVolume = 0.3f;

        private NavMeshAgent _nav;
        private ChaseState _chaseState;
        private PatrolState _patrolState;
        private IState _currentState;
        private AudioSource _shotSource;
        private int _health = 100;
        private RoundKeeper _roundKeeper;


        void Start ()
        {
            if (!isServer)
                return;

            GetComponent<NavMeshAgent>().enabled = true;
            InitiateSoundSettings();

            var allWaypoints = GameObject.FindGameObjectsWithTag(Constants.Tags.Waypoint).ToList();
            var selectedWapoints = allWaypoints.Select(go => go.transform)
                .OrderBy(t => Random.Range(0, allWaypoints.Count))
                .Take(NumberOfWaypoints)
                .ToList();

            _nav = GetComponent<NavMeshAgent>();
            _chaseState = new ChaseState(_nav, MsBetweenShots, _shotSource);
            _patrolState = new PatrolState(_nav, selectedWapoints);
            _currentState = _patrolState;

            GameObject.FindGameObjectsWithTag("Waypoint");
            _roundKeeper = GameObject.FindObjectOfType<RoundKeeper>();
        }


        void Update()
        {
            if(!isServer)
                return;

            _currentState.ExecuteState();
            _currentState = _patrolState;
        }

        void OnTriggerStay(Collider other)
        {
            if (!isServer)
                return;

            if(other.tag != Constants.Tags.Player)
                return;
            
            _chaseState.Target = other.gameObject.transform;
            _currentState = _chaseState;
        }

        public void HitByBullet(short owner)
        {
            _health -= 25;
            if (_health < 0)
            {
                Destroy(gameObject);
                if(isServer)
                    _roundKeeper.AddScoreTo();
            }
            
        }

        void InitiateSoundSettings()
        {
            _shotSource = gameObject.AddComponent<AudioSource>();
            _shotSource.clip = TankShotSound;
            _shotSource.loop = false;
            _shotSource.playOnAwake = false;
            _shotSource.volume = TankShotVolume;
            _shotSource.minDistance = 5;
            _shotSource.maxDistance = 20;
            _shotSource.rolloffMode = AudioRolloffMode.Linear;
            _shotSource.spatialBlend = 1;
        }
    }
}
