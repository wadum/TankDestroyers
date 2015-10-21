using System.Collections;
using System.ComponentModel;
using System.Linq;
using Assets.Scripts.Variables;
using Assets.Scripts.Weapons;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.AI
{
    public class EnemyMovement : NetworkBehaviour
    {

        public int MsBetweenShots;
        public int NumberOfWaypoints;
        public float RespawnDelay = 10;

        [Space(10)]
        [Header("Tank Shot")]
        public AudioClip TankShotSound;
        public float TankShotVolume = 0.3f;

        public HealthIndicator HealthIndicator;

        public short NetworkId { get; private set; }

        private NavMeshAgent _nav;
        private ChaseState _chaseState;
        private PatrolState _patrolState;
        private IState _currentState;
        private AudioSource _shotSource;
        
        [SyncVar]
        private float _health = 100;
        private RoundKeeper _roundKeeper;
        private Vector3 _spawnPos;
        private Quaternion _spawnRot;
        private BulletManager _bc;
        private float _nextTimeShotAllowed = 0;
        private float ChaseShooterUntil;

        

        void Start ()
        {
            NetworkId = (short)GetComponent<NetworkIdentity>().netId.Value;
            SetHealthOnIndicator(_health);
            
            if (!isServer)
                return;

            GetComponent<NavMeshAgent>().enabled = true;
            InitiateSoundSettings();
            _bc = GameObject.FindGameObjectWithTag("GameScripts").GetComponent<BulletManager>();

            var allWaypoints = GameObject.FindGameObjectsWithTag(Constants.Tags.Waypoint).ToList();
            var selectedWapoints = allWaypoints.Select(go => go.transform)
                .OrderBy(t => Random.Range(0, allWaypoints.Count))
                .Take(NumberOfWaypoints)
                .ToList();

            _nav = GetComponent<NavMeshAgent>();
            _chaseState = new ChaseState(_nav);
            _patrolState = new PatrolState(_nav, selectedWapoints);
            _currentState = _patrolState;

            _spawnPos = transform.position;
            _spawnRot = transform.rotation;

            GameObject.FindGameObjectsWithTag("Waypoint");
            _roundKeeper = FindObjectOfType<RoundKeeper>();
            
        }


        void Update()
        {
            if(!isServer)
                return;

            if (ChaseShooterUntil > Time.time)
                _currentState = _chaseState;

            _currentState.ExecuteState();
            _currentState = _patrolState;
            FireIfEnemy();
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

        public void HitByBullet(float damage, short owner)
        {
            _health -= damage;
            SetHealthOnIndicator(_health);
            ChaseShooter(owner);
            if (!(_health < 1)) return;
            if (!isServer) return;
            _roundKeeper.AddScoreTo(owner);
            Reset();
        }



        private void ChaseShooter(short owner)
        {
            var players = GameObject.FindGameObjectsWithTag(Constants.Tags.Player);
            var player = players.FirstOrDefault(p => p.GetComponent<NetworkIdentity>().netId.Value == owner);

            if (player == null) return;
            
            _chaseState.Target = player.transform;
            ChaseShooterUntil = Time.time + 2;
        }

        public void Reset()
        {
            _health = 100;
            transform.rotation = _spawnRot;
            transform.position = _spawnPos;
            _currentState = _patrolState;
            _patrolState.Reset();
            RpcReset();
        }

        //[ClientRpc]
        private void SetHealthOnIndicator(float health)
        {
            HealthIndicator.SetHealth(health);
        }

        [ClientRpc]
        private void RpcReset()
        {
            SetHealthOnIndicator(_health);
            StartCoroutine(ResetClient());
        }

        private IEnumerator ResetClient()
        {
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
            yield return new WaitForSeconds(RespawnDelay);
            gameObject.GetComponent<NavMeshAgent>().enabled = true;
            gameObject.GetComponent<BoxCollider>().enabled = true;
            transform.GetChild(0).gameObject.SetActive(true);

        }

        void InitiateSoundSettings()
        {
            _shotSource = gameObject.AddComponent<AudioSource>();
            _shotSource.clip = TankShotSound;
            _shotSource.loop = false;
            _shotSource.playOnAwake = false;
            _shotSource.volume = TankShotVolume;
            _shotSource.minDistance = 10;
            _shotSource.maxDistance = 100;
            _shotSource.rolloffMode = AudioRolloffMode.Linear;
            _shotSource.spatialBlend = 1;
        }

        private bool FireIfEnemy()
        {
            var fwd = _nav.transform.TransformDirection(Vector3.forward);
            var hits = Physics.RaycastAll(_nav.transform.position, fwd, 500f);

            foreach (var rh in hits)
            {
                if (rh.transform.gameObject.tag == Constants.Tags.Wall)
                    return false;
                if (rh.transform.gameObject.tag == Constants.Tags.Player)
                    FireIfAllowed(fwd);
            }

            return true;
        }

        private void FireIfAllowed(Vector3 direction)
        {
            if (Time.time <= _nextTimeShotAllowed)
                return;

            _bc.RpcFireWeapon(_nav.transform.position, direction, NetworkId);
            _nextTimeShotAllowed = Time.time + (MsBetweenShots / 1000f);
            _shotSource.Play();
        }
    }
}
