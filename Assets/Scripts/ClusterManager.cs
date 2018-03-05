using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterManager : MonoBehaviour {

    #region ABOUT
    /**
     * The manager for all clusters.
     **/
    #endregion

    #region VARIABLES
    // -- All clusters
    public List<Cluster> clusters;

    // The pathfinding object
    private Pathfinding pFinding;
    #endregion

    /// <summary>
    /// Initializes the connections & lookup table thereafter
    /// </summary>
    void Start () {
        // Acquire the pathfinding object
        // pFinding = GameObject.Find("Character").GetComponent<Pathfinding>();
        pFinding = GameObject.FindObjectOfType<Pathfinding>();

        // First find the connections
        ComputeBridges();
        Debug.Log("Done bridge calculation.");
        // Then build the lookup table
        ComputeLookUpTable();
        Debug.Log("Done lookup table calculation.");
	}

    /// <summary>
    /// Computes the connectivities to each cluster node.
    /// </summary>
    private void ComputeBridges()
    {
        // -- NOTE: This algorithm's worst time complexity is of O(n^4).
        // TODO: Consider optimizing it by statically setting distances/minima. This can reduce it to O(n^2).

        float minDistance = float.MaxValue;

        // Go through each cluster's neighbor clusters
        foreach (Cluster currCluster in clusters)
        {
            foreach (Cluster currNeighbor in currCluster.neighborClusters)
            {
                List<GameObject> currentNodes = currCluster.nodes;
                List<GameObject> neighborNodes = currNeighbor.nodes;

                GameObject currMin = currentNodes[0];
                minDistance = Vector3.Distance(currentNodes[0].transform.position, neighborNodes[0].transform.position);

                // Find which nodes have the shortest distance between them
                foreach (GameObject node1 in currentNodes)
                {
                    foreach (GameObject node2 in neighborNodes)
                    {
                        if (Physics.Linecast(node1.transform.position, node2.transform.position, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                        {
                            float dist = Vector3.Distance(node1.transform.position, node2.transform.position);
                            if (dist < minDistance)
                            {
                                minDistance = dist;
                                currMin = node1;
                            }
                        }
                    }
                }

                // Save the current neighbor and current minimum as an exit node at the end
                currCluster.exitNodes.Add(currNeighbor, currMin);
            }
        }
    }

    /// <summary>
    /// Builds the cluster lookup table.
    /// </summary>
    private void ComputeLookUpTable()
    {
        // Go through all the clusters 
        foreach (Cluster currCluster in clusters)
        {
            // Need to go through again for a 2D matrix-like representation
            foreach (Cluster other in clusters)
            {
                if (currCluster != other)
                {
                    // Copy the exit nodes here
                    List<GameObject> currExitNodes = new List<GameObject>(currCluster.exitNodes.Values);
                    List<GameObject> otherExitNodes = new List<GameObject>(other.exitNodes.Values);

                    float minDistance = float.MinValue;

                    // Candidate shortest path
                    List<GameObject> shortestPath = new List<GameObject>();

                    // For each node in our exit nodes
                    foreach (GameObject currNode in currExitNodes)
                    {
                        // Go through it again for 2D representation
                        foreach (GameObject otherNode in otherExitNodes)
                        {
                            // Compute the A* search given current node and goal node (other node)
                            List<GameObject> possiblePath = pFinding.ComputeAStar(currNode, otherNode);

                            // Get the total cost of this search path
                            // Sort the open list
                            float totalCost = pFinding.GetTotalCostOfPath(possiblePath);

                            // Set the minimum distance to the total cost (for future iterations)
                            // And set the shortest path to the just computed possible path if so
                            if (minDistance < 0 || totalCost < minDistance)
                            {
                                minDistance = totalCost;
                                shortestPath = possiblePath;
                            }
                        }
                    }

                    // Store the shortest path to the lookup table
                    currCluster.pathToCluster.Add(other, shortestPath);
                }
            }
        }
    }
}
