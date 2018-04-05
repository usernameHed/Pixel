using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

/// <summary>
/// ListenerPosition Description
/// </summary>
public class ListenerPosition : MonoBehaviour
{
    #region Attributes

    #endregion

    #region Initialization

    private void Start()
    {
		// Start function
    }
    #endregion

    #region Core

    #endregion

    #region Unity ending functions

    private void LateUpdate()
    {
        transform.SetZ(0);
    }


    #endregion
}
