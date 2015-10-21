using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Assets.Scripts.Variables;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class ChaseState : IState
    {
        public Transform Target { get; set; }

        private readonly NavMeshAgent _nav;
        private readonly BulletManager _bc;
        private readonly int _msBetweenShots;
        private float _nextTimeShotAllowed = 0;
        private readonly AudioSource _shotSource;
        private readonly short _networkId;

        public ChaseState(NavMeshAgent nav, int msBetweenShots, AudioSource shotSource, short networkId)
        {
            _networkId = networkId;
            _shotSource = shotSource;
            _nav = nav;
            _bc = GameObject.FindGameObjectWithTag("GameScripts").GetComponent<BulletManager>();
            _msBetweenShots = msBetweenShots;
        }

        public void ExecuteState()
        {
            _nav.enabled = true;
            if (Target != null)
                _nav.destination = Target.position;
            if (FireIfEnemy())
                _nav.enabled = false;
        }

        private bool FireIfEnemy()
        {
            var fwd = _nav.transform.TransformDirection(Vector3.forward);
            var hits = Physics.RaycastAll(_nav.transform.position, fwd, 10f);

            if (!hits.Where(other => other.transform.tag == Constants.Tags.Player).Any()) return false;
            
            FireIfAllowed(fwd);
            return true;
        }

        private void FireIfAllowed(Vector3 direction)
        {
            if(Time.time <= _nextTimeShotAllowed)
                return;

            _bc.RpcFireWeapon(_nav.transform.position, direction, _networkId);
            _nextTimeShotAllowed = Time.time + (_msBetweenShots/1000f);
            _shotSource.Play();
        }
    }
}
