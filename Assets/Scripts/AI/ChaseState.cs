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
        public float StopAtDistance = 10f;

        private readonly NavMeshAgent _nav;

        public ChaseState(NavMeshAgent nav)
        {
            _nav = nav;
        }

        public void ExecuteState()
        {
            _nav.enabled = true;
            if (Target != null)
            {
                if (Vector3.Distance(_nav.transform.position, Target.position) < StopAtDistance)
                {
                    _nav.destination = _nav.transform.position;

                    //find the vector pointing from our position to the target
                    var direction = (Target.position - _nav.transform.position).normalized;

                    //create the rotation we need to be in to look at the target
                    Quaternion lookRotation = Quaternion.LookRotation(direction);

                    //rotate us over time according to speed until we are in the required rotation
                    _nav.transform.rotation = Quaternion.Slerp(_nav.transform.rotation, lookRotation, Time.deltaTime * 2);
                }
                else
                    _nav.destination = Vector3.Lerp(_nav.transform.position, Target.position,
                        (Vector3.Distance(_nav.transform.position, Target.position) - StopAtDistance)/
                        Vector3.Distance(_nav.transform.position, Target.position));
            }
        }
    }
}
