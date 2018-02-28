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

    // -- Steering Arrive Variables
    private float velocityThreshold = 0.0f;
    private float angleThreshold = 1.0f;
    private float maxAcceleration = 1.5f;
    private float slowDownRadius = 0.5f;
    private Vector3 mVelocity;
    private float time_to_target = 0.5f;
    private const float ANGLE_ARC = 90.0f;
    private const float MAX_VELOCITY = 1.0f;
    private float maxDistance = 1.25f;

    /// ///////////////
    private float currentRotationVelocity = 0.0f;
    private float currentVelocity = 0.0f;

    // -- Steering Align variables
    // The maximum rotation acceleration radians
    private float maxRotationAccelerationRads = 5.0f;
    private float maxAngularVelocity = 180.0f;
    private float maxAngleAcceleration = 90.0f;
    private float slowDownOrientation = 90.0f;

    // By default, it's 0, will change after 1st alignment
    private float angularVel = 0.0f;
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
            else
            {
                target = null;
            }
        }
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
    /// Executes Steering Align behavior according to the target.
    /// </summary>
    private void SteeringAlignBehavior()
    {
        Vector3 targetTransform = target.transform.position;

        // Acquire the cross product for the sign
        float crossResult = Vector3.Cross(transform.forward, targetTransform).y;

        int sign_bit = 0;

        // Set the sign bit
        if (crossResult > 0)
        {
            sign_bit = 1;
        }
        else
        {
            sign_bit = -1;
        }

        // Find the angle difference to align to
        float differenceAngle = Vector3.Angle(transform.forward, targetTransform);

        if (differenceAngle > maxRotationAccelerationRads)
        {
            float goalVelocity = (maxAngularVelocity * differenceAngle) / slowDownOrientation;
            float goalAcceleration = (goalVelocity - angularVel) / time_to_target;

            if (goalAcceleration > maxAngleAcceleration)
            {
                // Clamp the goal acceleraiton if it surpasses the maximum
                goalAcceleration = maxAngleAcceleration;
            }

            // Multiply by time.deltatime since not in FixedUpdate for consistency
            angularVel += goalAcceleration * Time.deltaTime;
            if (angularVel > maxAngularVelocity)
            {
                // Clamp the angular velocity if it surpasses the maximum
                angularVel = maxAngularVelocity;
            }

            transform.Rotate(transform.up, angularVel * Time.deltaTime * sign_bit, Space.World);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(targetTransform);
        }
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
                SteeringAlignBehavior(); // Align before continuing

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
            SteeringAlignBehavior();
        }
    }
	
}
