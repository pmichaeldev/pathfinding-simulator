using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggedCharacter : MonoBehaviour {

    #region ABOUT
    /**
     * The tagged character.
     * Will flee from the chasing enemy.
     */
    #endregion

    #region VARIABLES
    [Tooltip("The target this character will seek and arrive to.")]
    public GameObject target;
    [Tooltip("The character chasing this tagged character.")]
    public GameObject chasingEnemy;

    private List<GameObject> visitedNodes = new List<GameObject>();
    // After visiting nodes 5 times (when it is <= 0), clear the visitedNodes list
    private int visitedNodeClearCounter = 5;

    private bool calledMoveToTarget = false;
    private GameObject[] nodes;

    // -- Steering Arrive Variables
    private float velocityThreshold = 0.0f;
    private float angleThreshold = 1.0f;
    private float maxAcceleration = 1.5f;
    private float slowDownRadius = 1.0f;
    private Vector3 mVelocity;
    private float time_to_target = 0.5f;
    private const float ANGLE_ARC = 90.0f;
    private const float MAX_VELOCITY = 2.0f;
    private float maxDistance = 0.50f;

    // -- Align Behavior Variables
    private Quaternion lookWhereYoureGoing;
    private Vector3 goalFacing;
    private float rotationSpeedRads = 1.5f;
    #endregion

    /// <summary>
    /// Finds the chasing enemy, if not set, and gets all nodes.
    /// </summary>
    void Start()
    {
        if (!chasingEnemy)
        {
            chasingEnemy = GameObject.FindGameObjectWithTag("Chasing");
        }

        nodes = GameObject.FindGameObjectsWithTag("Node");
    }

    /// <summary>
    /// Calls SteeringArriveBehavior at each frame to the closest node.
    /// No path finding is necessary.
    /// </summary>
    void Update()
    {
        if (chasingEnemy.GetComponent<ChasingCharacter>().currState == ChasingCharacter.ChasingState.CaughtTarget)
        {
            mVelocity = Vector3.zero;
            return;
        }

        // If no target is set, find the closest neighbor node
        if (!target)
        {
            // Target is a node with neighbors
            target = GetClosestNode();
            FindFarthestNeighborFromChasing();
            SteeringArriveBehavior();
        }
        else
        {
            // If we're very close to the target, we reset it and get a new one
            if (Vector3.Distance(this.transform.position, target.transform.position) < 0.5f)
            {
                MoveToTarget();
                FindFarthestNeighborFromChasing();
            }
            else
            {
                SteeringArriveBehavior();
            }
        }
    }

    /// <summary>
    /// Gets the node closest to the player.
    /// </summary>
    /// <returns>The closest node to the player.</returns>
    private GameObject GetClosestNode()
    {
        if (nodes == null || nodes.Length <= 0) return null;
        else
        {
            float shortestDistance = float.MaxValue;
            float dist = 0.0f;
            GameObject closestNode = null;
            foreach (GameObject _Node in nodes)
            {
                dist = Vector3.Distance(this.transform.position, _Node.transform.position);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    closestNode = _Node;
                }
            }
            return closestNode;
        }
    }

    /// <summary>
    /// Finds the farthest node from the chasing enemy.
    /// </summary>
    private void FindFarthestNeighborFromChasing()
    {
        if (visitedNodeClearCounter <= 0)
        {
            visitedNodes.Clear();
            visitedNodeClearCounter = 5;
        }

        // Add the node to our visited nodes list
        visitedNodes.Add(target);

        float maxDist = float.MinValue;
        GameObject farthestTarget = null;

        // Find farthest node
        foreach (GameObject _node in target.GetComponent<NodeNeighbors>().GetNeighbors())
        {
            float newMaxDist = Vector3.Distance(_node.transform.position, chasingEnemy.transform.position);
            if (newMaxDist > maxDist && !visitedNodes.Contains(_node))
            {
                maxDist = newMaxDist;
                farthestTarget = _node;
            }
        }

        // Set target to that farthest node & decrement the visitedNodeClearCounter
        target = farthestTarget;
        // Set a new target for the chasing character
        chasingEnemy.GetComponent<ChasingCharacter>().SetTarget(target);
        // Decrement node clear counter
        visitedNodeClearCounter--;
    }

    /// <summary>
    /// Moves the character to its target goal node after being Invoke'd.
    /// </summary>
    private void MoveToTarget()
    {
        this.transform.position = new Vector3(target.transform.position.x, 0.0f, target.transform.position.z);
        target = null;
        calledMoveToTarget = false;
    }

    /// <summary>
    /// Sets the target for the character to move to.
    /// </summary>
    /// <param name="_target">The target to move to.</param>
    public void SetTarget(GameObject _target)
    {
        target = _target;
    }

    /// <summary>
    /// Aligns the character to where it's going.
    /// </summary>
    private void AlignBehavior()
    {
        goalFacing = (target.transform.position - transform.position).normalized;
        lookWhereYoureGoing = Quaternion.LookRotation(goalFacing, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookWhereYoureGoing, rotationSpeedRads);
    }

    /// <summary>
    /// Executes Steering Arrive behavior, based on the target.
    /// </summary>
    private void SteeringArriveBehavior()
    {
        Vector3 targetTransform = target.transform.position;

        Vector3 direction = targetTransform - transform.position;


        if (direction.magnitude <= maxDistance)
        {
            // Step directly to target's position
            transform.position = new Vector3(targetTransform.x, 0.0f, targetTransform.z);
        }
        if (mVelocity.magnitude < velocityThreshold)
        {
            if (direction.magnitude <= maxDistance)
            {
                // Step directly to target's position
                transform.position = new Vector3(targetTransform.x, 0.0f, targetTransform.z);
            }
            else
            {
                // Begin by aligning to the target
                AlignBehavior();


                if (Vector3.Angle(transform.forward, direction) <= angleThreshold)
                {
                    Vector3 accel = maxAcceleration * direction.normalized;
                    // Time.deltaTime since not in FixedUpdate
                    mVelocity += accel * Time.deltaTime;

                    if (mVelocity.magnitude > maxAcceleration)
                    {
                        // Clamp the velocity so its magnitude is within maximum
                        mVelocity = mVelocity.normalized * maxAcceleration;
                    }

                    transform.Translate(transform.forward * mVelocity.magnitude * Time.deltaTime, Space.World);
                }
            }
        }
        else
        {
            if (Vector3.Angle(transform.forward, direction) <= ANGLE_ARC)
            {
                Vector3 accelerate;
                if (direction.magnitude < slowDownRadius)
                {
                    float goalFacing = (MAX_VELOCITY * direction.magnitude) / slowDownRadius;
                    float accel = (goalFacing - mVelocity.magnitude) / time_to_target;
                    accelerate = accel * direction.normalized;
                }
                else
                {
                    accelerate = maxAcceleration * direction.normalized;
                }

                // Time.deltaTime since not in FixedUpdate
                mVelocity += accelerate * Time.deltaTime;

                if (mVelocity.magnitude > maxAcceleration)
                {
                    // Clamp velocity's magnitude so it doesn't exceed maximum
                    mVelocity = mVelocity.normalized * maxAcceleration;
                }

                transform.Translate(transform.forward * mVelocity.magnitude * Time.deltaTime, Space.World);
            }

            // Align one last time.
            AlignBehavior();
        }
    }
	
}
