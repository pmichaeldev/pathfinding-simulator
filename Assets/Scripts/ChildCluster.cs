using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCluster : MonoBehaviour
{
    #region ABOUT
    /**
     * Identifier for sub-components of clusters (i.e. Part 1, 2, etc.)
     * It attaches itself to the parent.
     **/
    #endregion

    #region VARIABLES
    [Tooltip("The parent cluster for this sub-component.")]
    public Cluster parent;

    private MeshRenderer mRenderer;
    #endregion

    /// <summary>
    /// Acquires this child cluster's MeshRenderer component.
    /// </summary>
    void Start()
    {
        mRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Detects key press of 'T' key to display the clusters or not.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            mRenderer.enabled = !(mRenderer.enabled);
        }
    }

    /// <summary>
    /// Binds the cluster node that was passed.
    /// Calls its parent BindClusterNode function.
    /// </summary>
    /// <param name="node">The node to bind.</param>
    public void BindClusterNode(GameObject node)
    {
        parent.BindClusterNode(node);
    }
}
