using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeNeighbors : MonoBehaviour, System.IComparable<NodeNeighbors>
{
    #region ABOUT
    /**
     * This script populates a publicly accessible List of neighbor nodes to each individual node.
     * It executes 2 instances of ray casts to check for this, and adds accordingly, all while iterating
     * through a collider of nearby nodes within a radius.
     */ 
    #endregion

    #region VARIABLES
    [Tooltip("The neighbors to this node.")]
    public List<GameObject> neighborNodes;
    [Tooltip("The cost to reach this node, so far. This is h(n).")]
    public float costSoFar = 0.0f; // By default 0.0f
    [Tooltip("The total estimate value through this node. This would be g(n).")]
    public float totalEstimateVal = 0.0f; // By default 0.0f
    [Tooltip("Heuristic value (weight) of this node edge.")]
    public float heuristicVal = 0.0f; // By default 0.0f
    [Tooltip("The previous visited node to this one.")]
    public GameObject previousNode;
    // -- The neighboring cluster nodes to 'this' node
    public List<GameObject> neighborClusterNodes;
    [Tooltip("The cluster that this node belongs to.")]
    public Cluster cluster;

    private const float RADIUS = 2.0f;
    #endregion

    /// <summary>
    /// Initializes the neighboring nodes and its cluster.
    /// </summary>
    void Awake()
    {
        // Find the node's neighbors first
        FindNeighbors();

        // Then find its belonging cluster
        FindCluster();
    }

    /// <summary>
    /// Finds and sets this node's neighbor nodes.
    /// </summary>
    private void FindNeighbors()
    {
        // -- FIND THE NODE'S NEIGHBORS -- //
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
    /// Finds this node's associated cluster by firing a downwards ray.
    /// </summary>
    private void FindCluster()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            Cluster cluster = hit.collider.transform.GetComponent<Cluster>();
            ChildCluster clusterChild = hit.collider.transform.GetComponent<ChildCluster>();
            if (cluster != null)
            {
                cluster.BindClusterNode(this.gameObject);
            }
            else if (clusterChild != null)
            {
                clusterChild.BindClusterNode(this.gameObject);
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
    /// Returns the neighbor nodes to this node.
    /// </summary>
    /// <returns>The List of neighbor nodes.</returns>
    public List<GameObject> GetNeighbors() { return neighborNodes; }

    /// <summary>
    /// Compares 'this' node to a passed other node by checking their total estimate costs and heuristic values.
    /// </summary>
    /// <param name="node">A node to compare with.</param>
    /// <returns>An integer result of the comparison.</returns>
    public int CompareTo(NodeNeighbors node)
    {
        int comparison = totalEstimateVal.CompareTo(node.totalEstimateVal);

        if (totalEstimateVal != 0)
        {
            return comparison;
        }
        else
        {
            return heuristicVal.CompareTo(node.heuristicVal);
        }
    }

    /// <summary>
    /// Adds a neighboring cluster node to this node's neighboring cluster nodes list.
    /// </summary>
    /// <param name="node">The node to be added.</param>
    public void AddNeighborClusterNode(GameObject node)
    {
        neighborClusterNodes.Add(node);
    }

    /// <summary>
    /// Returns the neighbor cluster nodes list to this node.
    /// </summary>
    /// <returns>The neighbor cluster nodes list.</returns>
    public List<GameObject> GetNeighborClusterNodesList() { return neighborClusterNodes; }

    /// <summary>
    /// Sets this node's cluster.
    /// </summary>
    /// <param name="cluster">The cluster that this node will belong to.</param>
    public void SetCluster(Cluster cluster)
    {
        this.cluster = cluster;
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
