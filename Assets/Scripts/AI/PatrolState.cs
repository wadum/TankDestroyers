using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class PatrolState : IState
    {
        private readonly List<Transform> _waypoints;
        private readonly NavMeshAgent _nav;
        private int _currentWaypoint = 0;

        public PatrolState(NavMeshAgent nav, List<Transform> waypoints)
        {
            _waypoints = waypoints;
            _nav = nav;
        }

        public void ExecuteState()
        {
            if (IsWaypointReashed())
                _currentWaypoint = ++_currentWaypoint % _waypoints.Count;
            _nav.destination = _waypoints[_currentWaypoint].position;
        }

        private bool IsWaypointReashed()
        {
            // Check if we've reached the destination
            if (!_nav.pathPending && _nav.remainingDistance <= _nav.stoppingDistance)
                return !_nav.hasPath || _nav.velocity.sqrMagnitude < 0.2f;
            
            return false;
        }
    }
}
