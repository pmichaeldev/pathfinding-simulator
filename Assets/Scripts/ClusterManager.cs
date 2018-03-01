using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterManager : MonoBehaviour {

    #region ABOUT
    /**
     * The manager for all cluster nodes.
     * Builds the lookup table.
     **/
    #endregion

    #region VARIABLES
    // The lookup table for the cluster algorithm
    public Dictionary<int, Dictionary<int, float>> lookupTable;
    // lookupTable[1][2] returns a float for the distance between cluster 1 and 2
    // lookupTable[1][5] would return a float from cluster 1 to 5, despite not having immediate connection
    // another option List<List<float>>
    #endregion

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
