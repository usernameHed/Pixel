using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

/// <summary>
/// PlayerController handle player movement
/// <summary>
public class PlayerController : MonoBehaviour, IKillable
{
    #region Attributes
    [FoldoutGroup("Debug"), Tooltip("id unique du joueur correspondant à sa manette"), SerializeField]
    private int idPlayer = 0;
    public int IdPlayer { set { idPlayer = value; } get { return idPlayer; } }

    private bool stopAction = false;
    private bool enabledPlayer = true;
    #endregion

    #region Initialize

    private void OnEnable()
	{
        EventManager.StartListening(GameData.Event.GameOver, StopAction);
        InitPlayer();
	}

    /// <summary>
    /// initialise les players: créé les balls et les ajoutes dans la liste si la liste est vide
    /// </summary>
    private void InitPlayer()
    {
        Debug.Log("init player: " + idPlayer);
        enabledPlayer = true;
        stopAction = false;
        Invoke("StartAction", 0.1f);
    }

    #endregion

    #region Core
    
    /// <summary>
    /// input of player for both joystick
    /// </summary>
    private void InputPlayer()
    {
        /*for (int i = 0; i < ballsList.Count; i++)
        {
            if (!ballsList[i])
                continue;
            ballsList[i].HorizMove = PlayerConnected.Instance.getPlayer(idPlayer).GetAxis("Move Horizontal" + ((i == 0) ? "" : " Right") );
            ballsList[i].VertiMove = PlayerConnected.Instance.getPlayer(idPlayer).GetAxis("Move Vertical" + ((i == 0) ? "" : " Right"));

            ballsList[i].Power1 = PlayerConnected.Instance.getPlayer(idPlayer).GetButtonDown( ((i == 0) ? "Left" : "Right") + "Trigger1");
            ballsList[i].Power2 = PlayerConnected.Instance.getPlayer(idPlayer).GetAxis( ((i == 0) ? "Left" : "Right") + "Trigger2");

            if (ballsList[i].HorizMove != 0 || ballsList[i].VertiMove != 0)
                ballsList[i].HasMoved = true;
            else
                ballsList[i].HasMoved = false;
        }*/
    }

    /// <summary>
    /// stop les action du player...
    /// </summary>
    private void StartAction()
    {
        stopAction = false;
        Debug.Log("start action du joueur");
    }

    /// <summary>
    /// stop les action du player...
    /// </summary>
    private void StopAction()
    {
        stopAction = true;
        Debug.Log("stop action du joueur");
    }

    #endregion

    #region Unity ending functions
    /// <summary>
    /// input du joueur
    /// </summary>
    private void Update()
	{
        if (!stopAction)
            InputPlayer();
    }

    
    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, StopAction);

    }

    #endregion

    [FoldoutGroup("Debug"), Button("Kill")]
    public void Kill()
	{
        if (!enabledPlayer)
            return;
        
		Debug.Log ("Player dead !");
        enabledPlayer = false;
        

        Debug.Log("ici envoi du trigger...");
        EventManager.TriggerEvent(GameData.Event.PlayerDeath, idPlayer);
        gameObject.SetActive(false);
    }
}