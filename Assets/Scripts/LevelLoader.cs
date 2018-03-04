using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    #region ABOUT
    /**
     * This script handles loading scenes to either restart or move to another.
     **/
    #endregion

    /// <summary>
    /// Loads different scenes based on key presses.
    /// </summary>
	void Update () {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Loads the first scene, MainScene
            SceneManager.LoadScene("MainScene");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            // Loads the second scene, ChaseScene
            SceneManager.LoadScene("ChaseScene");
        }
	}
}
