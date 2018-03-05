using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingCharacter : MonoBehaviour {
    #region ABOUT
    /**
     * The chasing character.
     * He will chase, while using A* path finding, the tagged character.
     */
    #endregion

    #region VARIABLES
    [Tooltip("The target node that this character will arrive to.")]
    public GameObject target;

    public enum ChasingState { Chasing, Idling, CaughtTarget, NewTarget };
    [Tooltip("The state that the chasing character is in.")
    public ChasingState currState = ChasingState.Idling;

    private Pathfinding pFinding;
    private List<GameObject> pathList;
    private bool calledMoveToTarget = false;
    private GameObject[] nodes;

    // -- Steering Arrive Variables
    private float velocityThreshold = 0.5f;
    private float angleThreshold = 1.0f;
    private float maxAcceleration = 1.0f;
    private float slowDownRadius = 1.0f;
    private Vector3 mVelocity;
    private float time_to_target = 2.0f;
    private const float ANGLE_ARC = 90.0f;
    private const float MAX_VELOCITY = 1.5f;
    private float maxDistance = 0.50f;

    // -- Align Behavior Variables
    private Quaternion lookWhereYoureGoing;
    private Vector3 goalFacing;
    private float rotationSpeedRads = 1.5f;
    #endregion

    /// <summary>
    /// Acquires the Pathfinding object, then gets its path list, and finally finds a target node.
    /// </summary>
    void Start()
    {
        pFinding = GetComponent<Pathfinding>();
        nodes = GameObject.FindGameObjectsWithTag("Node");
    }

    /// <summary>
    /// Calls SteeringArriveBehavior at each frame if we have a target. 
    /// Additionally also handles unsetting targets if we've reached it.
    /// </summary>
    void Update()
    {
        if (currState == ChasingState.CaughtTarget)
        {
            Debug.Log("Caught target!");
            currState = ChasingState.Idling;
        }

        // If we reach the target, or near him, we set the state to caught the target
        if (Vector3.Distance(this.transform.position, target.transform.position) <= 0.5f)
        {
            currState = ChasingState.CaughtTarget;
        }

        if (currState == ChasingState.NewTarget)
        {
            // Since we have a new target, need to re-path
            currState = ChasingState.Chasing;
        }
        else if (currState == ChasingState.Chasing)
        {
            pFinding.goalNode = target;
            pFinding.ComputePath(); // Compute the path list
            AcquirePathList(); // Then set our path list to it
            SteeringArriveBehavior();
        }
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
    /// Acquires the path list from the Pathfinding script (which has been computed).
    /// </summary>
    private void AcquirePathList()
    {
        pathList = pFinding.pathList;
    }

    /// <summary>
    /// Sets the target for the character to move to.
    /// </summary>
    /// <param name="_target">The target to move to.</param>
    public void SetTarget(GameObject _target)
    {
        target = _target;
        // Signal that we have a new target
        currState = ChasingState.NewTarget;
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
