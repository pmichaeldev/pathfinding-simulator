﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeNeighbors : MonoBehaviour
{
    #region ABOUT
    /**
     * This script populates a publicly accessible List of neighbor nodes to each individual node.
     * It executes 2 instances of ray casts to check for this, and adds accordingly, all while iterating
     * through a collider of nearby nodes within a radius.
     */ 
    #endregion

    #region VARIABLES
    public List<GameObject> neighborNodes;

    private const float RADIUS = 2.0f;
    #endregion

    /// <summary>
    /// Initializes the neighboring nodes on Start.
    /// </summary>
    void Awake()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, RADIUS);
        foreach (var target in hitColliders)
        {
            if (target.gameObject.tag == "Node")
            {
                if (target.gameObject.Equals(this.gameObject))
                {
                    continue;
                }
                else if (!IsIntercepted(target.gameObject, this.gameObject))
                {
                    RaycastHit hit;
                    var origin = this.transform;
                    var to = target.gameObject.transform;
                    var direction = to.position - origin.position;
                    var distance = Vector3.Distance(origin.position, to.position);
                    if (Physics.Raycast(origin.position, direction, out hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                    {
                        if (target.gameObject.tag != "Node")
                        {
                            continue;
                        }
                        neighborNodes.Add(target.gameObject);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks if a ray cast from origin -> to is intercepted or not.
    /// </summary>
    /// <param name="origin">The origin we are checking for.</param>
    /// <param name="to">The target to cast the ray to.</param>
    /// <returns>True if intercepted, false otherwise.</returns>
    private bool IsIntercepted(GameObject origin, GameObject to)
    {
        RaycastHit hit;
        var direction = to.transform.position - origin.transform.position;
        var distance = Vector3.Distance(origin.transform.position, to.transform.position);
        Physics.Raycast(origin.transform.position, direction, out hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        if(hit.collider.gameObject.tag != "Node"){
            return true;
        }
        return false;
    }

    /// <summary>
    /// Draws the nodes as spheres, and lines connecting their edges (on play).
    /// </summary>
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(this.transform.position, 0.2f);
        foreach (GameObject _Node in neighborNodes)
        {
            // Draw the node
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_Node.transform.position, 0.2f);
            // Then draw its line
            Gizmos.color = Color.white;
            Gizmos.DrawLine(_Node.transform.position, this.transform.position);
        }
    }
}
