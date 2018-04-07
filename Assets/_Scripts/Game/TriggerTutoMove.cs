using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TriggerWin Description
/// </summary>
public class TriggerTutoMove : MonoBehaviour
{
    public enum EnableMove
    {
        Jump,
        Dash
    }

    #region Attributes
    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private EnableMove enableMove;

    private bool enabledObject = true;
    #endregion

    #region Initialization

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

        if (other.gameObject.CompareTag(GameData.Prefabs.Player.ToString()))
        {
            if (enableMove == EnableMove.Jump)
            {
                other.gameObject.GetComponent<PlayerJump>().enabled = true;
            }
            else if (enableMove == EnableMove.Dash)
            {
                other.gameObject.GetComponent<PlayerDash>().enabled = true;
            }
        }
    }
    #endregion

    #region Unity ending functions

	#endregion
}
