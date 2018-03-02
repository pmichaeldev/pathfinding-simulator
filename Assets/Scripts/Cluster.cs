using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster : MonoBehaviour
{
    #region ABOUT
    /**
     * Cluster identifying script.
     * Each node has an associated Cluster script attached to it.
     * This will identify which cluster it belongs to and, accordingly, hold the Dictionaries.
     **/
    #endregion

    #region VARIABLES
    public List<GameObject> nodes;
    public List<Cluster> neighborClusters;
    public Dictionary<Cluster, GameObject> exitNodes = new Dictionary<Cluster, GameObject>();
    public Dictionary<Cluster, List<GameObject>> pathToCluster = new Dictionary<Cluster, List<GameObject>>();

    // This cluster's mesh renderer
    private MeshRenderer mRenderer;
    #endregion

    /// <summary>
    /// Initializes the neighboring cluster nodes to 'this' node. Also acquires the MeshRenderer for this cluster.
    /// </summary>
	void Start () {
        mRenderer = GetComponent<MeshRenderer>();

		foreach(GameObject node in nodes)
        {
            foreach (GameObject candidateNode in nodes)
            {
                if (node != candidateNode && Physics.Linecast(node.transform.position, candidateNode.transform.position, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    node.GetComponent<NodeNeighbors>().AddNeighborClusterNode(candidateNode);
                }
            }
        }
	}

    /// <summary>
    /// Detects key press of 'T' key to display the clusters or not.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (mRenderer)
            {
                mRenderer.enabled = !(mRenderer.enabled);
            }
        }
    }

    /// <summary>
    /// Adds a node to the cluster's node list, and then sets that passed node's cluster to 'this'.
    /// </summary>
    /// <param name="node">The node to add to our list, and which we set its cluster to 'this'.</param>
    public void BindClusterNode(GameObject node)
    {
        nodes.Add(node);
        node.GetComponent<NodeNeighbors>().SetCluster(this);
    }
}
