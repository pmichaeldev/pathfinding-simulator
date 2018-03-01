using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region ABOUT
    /**
     * Character default script.
     * Handles movements.
     */
    #endregion

    #region VARIABLES
    [Tooltip("The target this character will seek and arrive to.")]
    public GameObject target;

    private Pathfinding pFinding;
    private List<GameObject> pathList;
    private bool calledMoveToTarget = false;

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
    Quaternion lookWhereYoureGoing;
    Vector3 goalFacing;
    float rotationSpeedRads = 1.5f;
    #endregion

    /// <summary>
    /// Acquires the Pathfinding object, then gets its path list, and finally finds a target node.
    /// </summary>
    void Start()
    {
        pFinding = GetComponent<Pathfinding>();
        AcquirePathList();
        GetNewTarget();
    }

    /// <summary>
    /// Calls SteeringArriveBehavior at each frame if we have a target. 
    /// Additionally also handles unsetting targets if we've reached it.
    /// </summary>
    void Update()
    {
        // If we still have a target but nothing's left in the path node
        // Then this means we're near the end, and just need to get there
        // So invoke a move to target just in case we didn't reach it yet.
        if (target && pathList.Count == 0 && !calledMoveToTarget)
        {
            // Set the flag so we don't keep invoking
            calledMoveToTarget = true;
            Invoke("MoveToTarget", 2.0f);
        }

        if (target == null && pathList.Count > 0)
        {
            GetNewTarget();
        }
        else
        {
            if (target == null) return;

            if (Vector3.Distance(transform.position, target.transform.position) > 1.0f)
            {
                SteeringArriveBehavior();
            }
            else if (pathList.Count > 0)
            {
                GetNewTarget();
            }
        }
    }

    /// <summary>
    /// Moves the character to its target goal node after being Invoke'd.
    /// </summary>
    private void MoveToTarget()
    {
        this.transform.position = target.transform.position;
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
    /// Gets a new target to go towards.
    /// </summary>
    private void GetNewTarget()
    {
        // Set a target
        if (target == null && pathList[1])
        {
            target = pathList[1];
            pathList.Remove(pathList[0]);
            pathList.Remove(pathList[0]);
        }
        else if (pathList[0] != null)
        {
            target = pathList[0];
            pathList.Remove(target);
        }
        else
        {
            target = null;
        }
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
            transform.position = new Vector3(targetTransform.x, transform.position.y, targetTransform.z);
        }
        if (mVelocity.magnitude < velocityThreshold)
        {
            if (direction.magnitude <= maxDistance)
            {
                // Step directly to target's position
                transform.position = new Vector3(targetTransform.x, transform.position.y, targetTransform.z);
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
