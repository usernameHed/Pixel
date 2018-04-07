using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TriggerWin Description
/// </summary>
public class TriggerWind : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private float windForce = 3f;
    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private Transform dir;
    
    // Directional force applied to objects that enter this object's Collider 2D boundaries
    public Vector3 force;
    private bool applyForce = false;

    private PlayerController playerToPush;
    #endregion

    #region Initialization

    private void Start()
    {
        force = transform.position - dir.position;
    }
    #endregion

    #region Core
    private void FixedUpdate()
    {
        if (!applyForce)
            return;

        GameObject[] list = playerToPush.ListObjToPush;
        for (int i = 0; i < list.Length; i++)
        {
            PhysicsExt.ApplyConstForce(list[i].GetComponent<Rigidbody>(), force, windForce);
        }
    }

    /// <summary>
    /// le player entre
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(GameData.Prefabs.Player.ToString()))
        {
            applyForce = true;
            playerToPush = other.gameObject.GetComponent<PlayerController>();
        }
    }

    /// <summary>
    /// le player sort
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(GameData.Prefabs.Player.ToString()))
        {
            applyForce = false;
            playerToPush = null;
        }
    }

    #endregion

    #region Unity ending functions

	#endregion
}
