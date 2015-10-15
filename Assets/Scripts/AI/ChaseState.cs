using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class ChaseState : IState
    {
        public Transform Target { get; set; }
        private readonly NavMeshAgent _nav;

        public ChaseState(NavMeshAgent nav)
        {
            _nav = nav;
        }

        public void ExecuteState()
        {
            if (Target != null)
                _nav.destination = Target.position;
        }
    }
}
