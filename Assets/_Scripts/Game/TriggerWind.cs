using Sirenix.OdinInspector;
using System.Collections;
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

    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private float randomMinForce = 5f;
    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private float randomMaxForce = 20f;

    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private float minWindRandom = 0.1f;
    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private float maxWindRandom = 1.5f;
    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private float minNoWindRandom = 0.1f;
    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private float maxNoWindRandom = 1.5f;

    // Directional force applied to objects that enter this object's Collider 2D boundaries
    public Vector3 force;
    private bool applyForce = false;

    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private FrequencyCoolDown first;
    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private FrequencyCoolDown second;

    private bool passSecond = false;


    private PlayerController playerToPush;
    #endregion

    #region Initialization

    private void Start()
    {
        force = transform.position - dir.position;
        first.StartCoolDown(Random.Range(minWindRandom, maxWindRandom));
        //second.StartCoolDown();
    }
    #endregion

    #region Core
    private void FixedUpdate()
    {
        if (!applyForce)
            return;

        if (!first.IsReady() && second.IsReady())
        {
            //ici le premier timer
            GameObject[] list = playerToPush.ListObjToPush;
            for (int i = 0; i < list.Length; i++)
            {
                PhysicsExt.ApplyConstForce(list[i].GetComponent<Rigidbody>(), force, Random.Range(randomMinForce, randomMaxForce));
            }
        }
        else if (first.IsReady() && second.IsReady() && !passSecond)
        {
            second.StartCoolDown(Random.Range(minNoWindRandom, maxNoWindRandom));
            passSecond = true;
        }
        else if (first.IsReady() && second.IsReady() && passSecond)
        {
            first.StartCoolDown(Random.Range(minWindRandom, maxWindRandom));
            passSecond = false;
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
