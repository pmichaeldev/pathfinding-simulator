using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeRayTest : MonoBehaviour {

    public List<GameObject> neighborNodes;

	// Initializes neighbor nodes on Start
    void Start()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 2.0f);
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
                    Debug.Log("In");
                    if (Physics.Raycast(origin.position, direction, out hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                    {
                        if (target.gameObject.tag != "Node")
                        {
                            continue;
                        }
                        neighborNodes.Add(target.gameObject);
                        Debug.Log("Added");
                    }
                }
            }
        }
    }

    // -- Returns TRUE if a ray, cast from origin -> to, is intercepted
    // -- FALSE otherwise
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

    // -- Draws the connected nodes with lines
    public void OnDrawGizmos()
    {
        foreach (GameObject _Node in neighborNodes)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(_Node.transform.position, this.transform.position);
        }
    }
}
