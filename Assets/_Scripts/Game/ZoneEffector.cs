using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CircleCollider2D))]
public class ZoneEffector : MonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    public List<TimeWithEffectOnCamera> listEffector;
    public bool onEnter = true;
    public bool onExit = true;
    public bool triggerPlayer = true;

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>

    /// <summary>
    /// variable privé serealized
    /// </summary>
    //[SerializeField] private bool tmp;
    private bool isEnter = false;

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {

    }

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Start()                                                    //initialsiation
    {
        //listEffector.Clear();
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
                listEffector.Add(child.gameObject.GetComponent<TimeWithEffectOnCamera>());
        }
    }
    #endregion

    #region core script
    /// <summary>
    /// Fonction qui parcourt les objets à mettre in/out de la caméra (selon active)
    /// </summary>
    private void ActionOnCamera(bool active)                                             //test
    {
        isEnter = active;
        for (int i = 0; i < listEffector.Count; i++)
        {
            if (!listEffector[i])
                continue;
            if (active)
            {
                listEffector[i].alwaysOnCamera = true;                                  //set que l'objet est TOUJOURS dans la caméra

                if (listEffector[i].isOk)                                               //si isOk: c'est la première fois, on le met false (dans la caméra !)
                    listEffector[i].isOk = false;
                else
                    listEffector[i].restart = true;                                     //sinon, il est déjà, faire un restart
                    
            }
            else
            {
                listEffector[i].alwaysOnCamera = false;                                 //set que l'objet est PAS TOUJOURS dans la caméra
                listEffector[i].stopNow();
            }
            
        }
    }
    #endregion

    #region unity fonction and ending

    /// <summary>
    /// action lorsque le joueur entre dans une zone
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if (isEnter)
            return;
        //si c'est un collider 2D, et que son objet de reference est un joueur
        if (onEnter && collision.CompareTag(GameData.Prefabs.Player.ToString()) && triggerPlayer)
        {
            //collision.gameObject.GetComponent<PlayerController>().addZone(this);
            ActionOnCamera(true);
        }
    }

    /// <summary>
    /// active ou désactive la zone via un script
    /// </summary>
    /// <param name="active"></param>
    public void activeZoneByScript(bool active)
    {
        ActionOnCamera(active);
    }

    /// <summary>
    /// action lorsque le joueur sort d'une zone
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit(Collider collision)
    {
        //si c'est un collider 2D, et que son objet de reference est un joueur
        if (onExit && collision.CompareTag(GameData.Prefabs.Player.ToString()))
        {
            //collision.gameObject.GetComponent<PlayerController>().deleteZone(this);
            ActionOnCamera(false);
        }
    }
    #endregion
}
