using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TriggerWin Description
/// </summary>
public class TriggerNoisette : MonoBehaviour
{
    #region Attributes

    [FoldoutGroup("GamePlay"), Tooltip("list des prefabs à push"), SerializeField]
    private List<GameData.Layers> listLayerToCollide;

    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on take"), SerializeField]
    private Vibration onTake;

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

        if (GameData.IsInList(listLayerToCollide, other.gameObject.layer) && other.gameObject.HasComponent<PlayerController>())
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            PlayerConnected.Instance.setVibrationPlayer(playerController.IdPlayer, onTake);
            SoundManager.GetSingleton.playSound(GameData.Sounds.Bonus.ToString() + transform.parent.GetInstanceID());

            playerController.GetNoisette();

            gameObject.transform.parent.gameObject.SetActive(false);
            //GameManager.Instance.SceneManagerLocal.PlayIndex(2, true);
            enabledObject = false;
        }
    }
    #endregion

    #region Unity ending functions

	#endregion
}
