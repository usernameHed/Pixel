﻿using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TriggerWin Description
/// </summary>
public class TriggerDeath : MonoBehaviour
{
    #region Attributes

    [FoldoutGroup("GamePlay"), Tooltip("list des prefabs à push"), SerializeField]
    private List<GameData.Layers> listLayerToCollide;

    private bool enabledObject = true;
    #endregion

    #region Initialization

    private void Start()
    {
		// Start function
    }
    #endregion

    #region Core
    /// <summary>
    /// trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!enabledObject)
            return;

        if (GameData.IsInList(listLayerToCollide, other.gameObject.layer))
        {
            IKillable kill = other.gameObject.GetComponent<IKillable>();
            if (kill != null)
            {
                kill.Kill();
            }

            //GameManager.Instance.SceneManagerLocal.PlayIndex(2, true);
            enabledObject = false;
        }
    }
    #endregion

    #region Unity ending functions

	#endregion
}
