using System.Collections.Generic;
using System.Linq;
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
            // Makes the enenmy stand still, if no waypoints assinged
            if(!_waypoints.Any())
                return;

            // Change target, if waypoint is reached
            if (IsWaypointReashed())
                _currentWaypoint = ++_currentWaypoint % _waypoints.Count;
            
            _nav.destination = _waypoints[_currentWaypoint].position;
        }

        private bool IsWaypointReashed()
        {
            if (!_nav.pathPending && _nav.remainingDistance <= _nav.stoppingDistance)
                return !_nav.hasPath || _nav.velocity.sqrMagnitude < 0.2f;
            
            return false;
        }
    }
}
