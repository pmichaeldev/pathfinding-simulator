using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    #region ABOUT
    /**
     * Pathfinding implementation of the 3 algorithms:
     * Dijkstra (with 0 heuristic)
     * A* (with Euclidean Distance heuristic)
     * Cluster
     * 
     * => THIS SCRIPT IS ATTACHED TO THE CHARACTER! 
     **/
    #endregion

    #region VARIABLES
    // -- All nodes in our scene
    private GameObject[] nodes;

    [Tooltip("The path list of nodes to visit.")]
    public List<GameObject> pathList;
    [Tooltip("The open list for nodes.")]
    public List<GameObject> openList;
    [Tooltip("The closed list for nodes.")]
    public List<GameObject> closedList;
    [Tooltip("The goal node to reach.")]
    public GameObject goalNode;

    // Closest node to character
    private GameObject startNode;
    #endregion

    /// <summary>
    /// Initialization for variables.
    /// </summary>
    void Start()
    {
        nodes = GameObject.FindGameObjectsWithTag("Node");
        ComputePath();
        DisplayPath();
    }

    /// <summary>
    /// Clears the lists and calls appropriate algorithm function.
    /// </summary>
    private void ComputePath()
    {
        // Clear the lists first
        pathList.Clear();
        openList.Clear();
        closedList.Clear();

        // Select which algorithm we use
        switch (GameController.currentAlgorithm)
        {
            case GameController.AlgorithmChoice.Dijkstra:
                // Call Dijkstra's
                DijkstrasAlgorithm();
                break;
            case GameController.AlgorithmChoice.AStar:
                // Call A*
                AStarAlgorithm();
                break;
            case GameController.AlgorithmChoice.Cluster:
                // Call Cluster
                break;
            default:
                Debug.LogError("ERROR::UNKNWON_CURRENT_ALGORITHM");
                break;
        }
    }

    private void DijkstrasAlgorithm()
    {
        // Find closest node to the character
        // By default the character will never be too far from a node.
        GameObject closestNode = GetClosestNode();
        startNode = closestNode;

        if (closestNode == null)
        {
            Debug.LogError("ERROR::UNKNOWN_CLOSEST_NODE_TO_PLAYER");
            return;
        }

        // TODO: Remove these Debug statements later.
        Debug.Log("closestNode's x: " + closestNode.transform.position.x + ", y: " + closestNode.transform.position.y + ", z: " + closestNode.transform.position.z);
        Debug.Log("closestNode's name: " + closestNode.name);
        ////////////////////////////////////////////////////////

        // Add this closest node to the Open List
        openList.Add(closestNode);

        // While we still have nodes in the openlist, and the current node is not the goal...
        while (openList.Count > 0 && openList[0] != goalNode)
        {
            DijkstraVisitNode(openList[0]); // Visit the next node in the open list sequentially
        }

        // Compute the path list
        ComputePathList();
    }

    /// <summary>
    /// Dijkstra node visit algorithm.
    /// </summary>
    /// <param name="node">The node we're currently visiting.</param>
    private void DijkstraVisitNode(GameObject node)
    {
        // Now that we're visiting this node, add it to the closed list
        closedList.Add(node);
        // As such, remove it from the open list
        openList.Remove(node);

        List<GameObject> neighbors = node.GetComponent<NodeNeighbors>().GetNeighbors();

        foreach (GameObject currNeighbor in neighbors)
        {
            if(Physics.Linecast(node.transform.position, currNeighbor.transform.position, Physics.DefaultRaycastLayers)){

                NodeNeighbors currentNode = currNeighbor.GetComponent<NodeNeighbors>();
                float distance = (currNeighbor.transform.position - node.transform.position).magnitude;
                float costSoFar = node.GetComponent<NodeNeighbors>().costSoFar + distance;
                float heuristic = 0.0f; // As per instructed, 0 heuristic for Dijkstra's algorithm
                float totalEstimateVal = costSoFar + heuristic;

                bool isInClosedList = closedList.Contains(currNeighbor);
                bool isInOpenList = openList.Contains(currNeighbor);
                bool foundBetter = totalEstimateVal < currentNode.totalEstimateVal;

                if (isInClosedList && foundBetter)
                {
                    // Update the current node's attributes
                    UpdateNodeValues(currentNode, costSoFar, totalEstimateVal, heuristic, node);

                    // Remove it from the closed list
                    closedList.Remove(currNeighbor);
                    // Add it to the open list
                    openList.Add(currNeighbor);
                }
                else if (isInOpenList && foundBetter)
                {
                    // Update the current node's attributes
                    UpdateNodeValues(currentNode, costSoFar, totalEstimateVal, heuristic, node);
                }
                else if (!isInClosedList && !isInOpenList)
                {
                    // Update the current node's attributes
                    UpdateNodeValues(currentNode, costSoFar, totalEstimateVal, heuristic, node);
                    
                    // Add it to the open list
                    openList.Add(currNeighbor);
                }
            }
        }

        // Sort the open list
        openList.Sort((GameObject n, GameObject m) => { return n.GetComponent<NodeNeighbors>().costSoFar.CompareTo(m.GetComponent<NodeNeighbors>().costSoFar); });
    }

    /// <summary>
    /// A* path finding algorithm. Will execute as intended by visiting nodes.
    /// </summary>
    private void AStarAlgorithm()
    {
        // Find closest node to the character
        // By default the character will never be too far from a node.
        GameObject closestNode = GetClosestNode();
        startNode = closestNode;

        if (closestNode == null)
        {
            Debug.LogError("ERROR::UNKNOWN_CLOSEST_NODE_TO_PLAYER");
            return;
        }

        // TODO: Remove these Debug statements later.
        Debug.Log("closestNode's x: " + closestNode.transform.position.x + ", y: " + closestNode.transform.position.y + ", z: " + closestNode.transform.position.z);
        Debug.Log("closestNode's name: " + closestNode.name);
        ////////////////////////////////////////////////////////

        // Add this closest node to the Open List
        openList.Add(closestNode);

        while (openList.Count > 0 && openList[0] != goalNode)
        {
            // Visit the nodes
            AStarVisitNode(openList[0]);
        }

        // Finally compute the final path list
        ComputePathList();
    }

    /// <summary>
    /// Similar to the Dijkstra algorithm, we visit the node passed.
    /// This time, the heuristic is not 0, but rather the Euclidean Distance.
    /// </summary>
    /// <param name="node"></param>
    private void AStarVisitNode(GameObject node)
    {
        // Now that we're visiting this node, add it to the closed list
        closedList.Add(node);
        // As such, remove it from the open list
        openList.Remove(node);

        List<GameObject> neighbors = node.GetComponent<NodeNeighbors>().GetNeighbors();

        foreach (GameObject currNeighbor in neighbors)
        {
            if (Physics.Linecast(node.transform.position, currNeighbor.transform.position, Physics.DefaultRaycastLayers))
            {

                NodeNeighbors currentNode = currNeighbor.GetComponent<NodeNeighbors>();
                float distance = (currNeighbor.transform.position - node.transform.position).magnitude;
                float costSoFar = node.GetComponent<NodeNeighbors>().costSoFar + distance;
                // Distance is the heuristic
                float heuristic = Vector3.Distance(goalNode.transform.position, node.transform.position);
                float totalEstimateVal = costSoFar + heuristic;

                bool isInClosedList = closedList.Contains(currNeighbor);
                bool isInOpenList = openList.Contains(currNeighbor);
                bool foundBetter = totalEstimateVal < currentNode.totalEstimateVal;

                if (isInClosedList && foundBetter)
                {
                    // Update the current node's attributes
                    UpdateNodeValues(currentNode, costSoFar, totalEstimateVal, heuristic, node);

                    // Remove it from the closed list
                    closedList.Remove(currNeighbor);
                    // Add it to the open list
                    openList.Add(currNeighbor);
                }
                else if (isInOpenList && foundBetter)
                {
                    // Update the current node's attributes
                    UpdateNodeValues(currentNode, costSoFar, totalEstimateVal, heuristic, node);
                }
                else if (!isInClosedList && !isInOpenList)
                {
                    // Update the current node's attributes
                    UpdateNodeValues(currentNode, costSoFar, totalEstimateVal, heuristic, node);

                    // Add it to the open list
                    openList.Add(currNeighbor);
                }
            }
        }

        // Sort the open list
        openList.Sort((GameObject n, GameObject m) => { return n.GetComponent<NodeNeighbors>().costSoFar.CompareTo(m.GetComponent<NodeNeighbors>().costSoFar); });
    }

    /// <summary>
    /// Updates the passed node's key essential attributes.
    /// </summary>
    /// <param name="node">The node we're updating.</param>
    /// <param name="costSoFar">New cost so far.</param>
    /// <param name="totalEstimateVal">New estimated total cost.</param>
    /// <param name="heuristic">New h(n) value.</param>
    /// <param name="previousNode">The previous node to the updated one.</param>
    private void UpdateNodeValues(NodeNeighbors node, float costSoFar, float totalEstimateVal, float heuristic, GameObject previousNode)
    {
        node.costSoFar = costSoFar;
        node.totalEstimateVal = totalEstimateVal;
        node.heuristicVal = heuristic;
        node.previousNode = previousNode;
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
    /// Computes the path list for the pathing.
    /// </summary>
    private void ComputePathList()
    {
        // Add the goal node at the start
        pathList.Add(goalNode);
        while (pathList.Count > 0 && pathList[pathList.Count - 1] != startNode)
        {
            GameObject nextNode = pathList[pathList.Count - 1].GetComponent<NodeNeighbors>().previousNode;
            // If there is no previous node
            if (!nextNode)
            {
                // Clear the list and exit
                pathList.Clear();
                Debug.LogError("ERROR::IMPOSSIBLE_PATHING_ATTEMPTED");
                return;
            }
            pathList.Add(nextNode);
        }

        // Finally, reverse the list so we get the proper order
        pathList.Reverse();
    }

    /// <summary>
    /// Displays (re-enables) visited path nodes.
    /// </summary>
    private void DisplayPath()
    {
        foreach (GameObject node in pathList)
        {
            node.GetComponent<Renderer>().enabled = true;
        }
    }
}
