using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlgorithmChoiceDisplay : MonoBehaviour
{
    #region ABOUT
    /**
     * Displays currently selected Algorithm.
     */ 
    #endregion

    #region VARIABLES
    private Text currentAlgo;
    #endregion

    /// <summary>
    /// Gets the Text component and sets the default from the Controller.
    /// </summary>
	void Start () {
        currentAlgo = GetComponent<Text>();
        ChangeAlgorithm();
	}

	/// <summary>
	/// Changes the displayed algorithm choice based on the Controller.
	/// </summary>
	public void ChangeAlgorithm(){
        if (GameController.currentAlgorithm == GameController.AlgorithmChoice.Dijkstra)
        {
            currentAlgo.text = "Dijkstra";
        }
        else if (GameController.currentAlgorithm == GameController.AlgorithmChoice.AStar)
        {
            currentAlgo.text = "A*";
        }
        else
        {
            currentAlgo.text = "Cluster";
        }
    }
}
