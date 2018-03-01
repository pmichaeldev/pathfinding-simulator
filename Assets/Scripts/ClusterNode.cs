using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterNode : MonoBehaviour {
    #region ABOUT
    /**
     * Cluster identifier for nodes belonging to a cluster.
     * Clusters are as follows (from top left of Game View):
     * 1 = top left
     * 2 = top right
     * 3 = middle left
     * 4 = center portion/hallway
     * 5 = bottom/middle right
     * 6 = bottom left
     **/
    #endregion

    #region VARIABLES
    [Tooltip("The int ID of the cluster that this node belongs to.")]
    public int clusterBelongsTo; // Needs to be set in inspector by hand
    #endregion

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
