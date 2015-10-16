using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Variables;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class ChaseState : IState
    {
        public Transform Target { get; set; }

        private readonly NavMeshAgent _nav;
        private readonly IWeaponController _bc;
        private readonly int _msBetweenShots;
        private float _nextTimeShotAllowed = 0;
        private readonly AudioSource _shotSource;

        public ChaseState(NavMeshAgent nav, int msBetweenShots, AudioSource shotSource)
        {
            _shotSource = shotSource;
            _nav = nav;
            _bc = GameObject.FindGameObjectWithTag("GameScripts").GetComponent<BulletManager>();
            _msBetweenShots = msBetweenShots;
        }

        public void ExecuteState()
        {
            if (Target != null)
                _nav.destination = Target.position;
            FireIfEnemy();
        }

        private void FireIfEnemy()
        {
            var fwd = _nav.transform.TransformDirection(Vector3.forward);
            var hits = Physics.RaycastAll(_nav.transform.position, fwd, 10f);

            if (hits.Where(other => other.transform.tag == Constants.Tags.Player).Any())
                FireIfAllowed(fwd); 
        }

        private void FireIfAllowed(Vector3 direction)
        {
            if(Time.time <= _nextTimeShotAllowed)
                return;

            _bc.FireWeapon(_nav.transform.position, direction);
            _nextTimeShotAllowed = Time.time + (_msBetweenShots/1000f);
            _shotSource.Play();
        }
    }
}
