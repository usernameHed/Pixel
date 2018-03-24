using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// SuperPower Description
/// </summary>
public class SuperPower : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("Object"), Tooltip("ref"), SerializeField]
    private GameObject displaySuperPower;

    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private PlayerController playerController;

    private bool superPowerActived = false;
    public bool SuperPowerActived { get { return (superPowerActived); } }

    #endregion

    #region Initialization
    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.SuperPower, SuperPowerActive);
    }

    private void Start()
    {
        superPowerActived = false;
        if (displaySuperPower)
            displaySuperPower.SetActive(false);
    }
    #endregion

    #region Core
    public void SetSuperPower()
    {
        EventManager.TriggerEvent(GameData.Event.SuperPower);
    }

    private void SuperPowerActive()
    {
        if (displaySuperPower)
            displaySuperPower.SetActive(true);
        superPowerActived = true;
        ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.DeathPlayer, transform.position, Quaternion.identity, ObjectsPooler.Instance.transform);
    }
    #endregion

    #region Unity ending functions

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.SuperPower, SuperPowerActive);
    }
    #endregion
}
