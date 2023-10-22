using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }    // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; }    // the character we are controllingf
        // public Transform target;  // target to aim for
        public bool useWaypoints = true;

        public Transform[] points;
        private int destPoint = 0;


        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

	        agent.updateRotation = true;
	        agent.updatePosition = true;

            // Disabling auto-braking allows for continuous movement
            // between points (ie, the agent doesn't slow down as it
            // approaches a destination point).
            agent.autoBraking = true;

            if (useWaypoints && points.Length > 1)
                GotoNextPoint();
            else
                Debug.LogWarning(gameObject.name + " does not have it's waypoints setup ^_^");
        }


        private void Update()
        {

            if (useWaypoints)
            {
                if (agent.remainingDistance > agent.stoppingDistance)
                    character.Move(agent.desiredVelocity, false, false);
                // Choose the next destination point when the agent gets
                // close to the current one.
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    GotoNextPoint();
                }
            }

            // else if (!useWaypoints)
            // {
            //     if(target != null)
            //         agent.SetDestination(target.position);
            // 
            //     if (agent.remainingDistance > agent.stoppingDistance)
            //         character.Move(agent.desiredVelocity, false, false);
            // 
            //     else
            //     {
            //         // wait, shop, gesture, next waypoint
            //         character.Move(Vector3.zero, false, false);
            //     }
            //         
            // }
        }

        void GotoNextPoint()
        {
            // Returns if no points have been set up
            if (points.Length == 0)
            {
                Debug.Log("There are no waypoint setup for this AI Character Control");
                return;
            }

            int i = 0;

            if(points[destPoint].transform.childCount >= 2)
            {
                // destPoint = RANDOM child transform
            }

            // Set the agent to go to the currently selected destination.
            agent.SetDestination(points[destPoint].position);

            // Choose the next point in the array as the destination,
            // cycling to the start if necessary.
            destPoint = (destPoint + 1) % points.Length;
        }
    }
}
