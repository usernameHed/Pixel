using UnityEngine;
using Rewired;
using Sirenix.OdinInspector;

/// <summary>
/// Gère la connexion / déconnexion des manettes
/// <summary>


public class PlayerConnected : SingletonMono<PlayerConnected>
{
    protected PlayerConnected() { } // guarantee this will be always a singleton only - can't use the constructor!

    #region variable

    [FoldoutGroup("Vibration"), Tooltip("Active les vibrations")]  public bool enabledVibration = true;
    [FoldoutGroup("Vibration"), Tooltip("the first motor")]        public int motorIndex = 0; 
    [FoldoutGroup("Vibration"), Tooltip("full motor speed")]       public float motorLevel = 1.0f;
    [FoldoutGroup("Vibration"), Tooltip("durée de la vibration")]  public float duration = 2.0f;

    public bool simulatePlayerOneifNoGamePad = false;   //Si aucune manette n'est connecté, active le player 1 avec le clavier !
    public bool[] playerArrayConnected;                      //tableau d'état des controller connecté
    private short playerNumber = 4;                     //size fixe de joueurs (0 = clavier, 1-4 = manette)



    private Player[] playersRewired;                 //tableau des class player (rewired)
    private float timeToGo;

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        playerArrayConnected = new bool[playerNumber];                           //initialise 
        playersRewired = new Player[playerNumber];
        initPlayerRewired();                                                //initialise les event rewired
        initController();                                                   //initialise les controllers rewired   
    }

    /// <summary>
    /// Initialisation à l'activation
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// initialise les players
    /// </summary>
    private void initPlayerRewired()
    {
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;

        //parcourt les X players...
        for (int i = 0; i < playerNumber; i++)
        {
            playersRewired[i] = ReInput.players.GetPlayer(i);       //get la class rewired du player X
            playerArrayConnected[i] = false;                             //set son état à false par défault
        }

        setKeyboardForPlayerOne();
    }

    /// <summary>
    /// défini le keyboard pour le joueur 1 SI il n'y a pas de manette;
    /// </summary>
    private void setKeyboardForPlayerOne()
    {
        if (simulatePlayerOneifNoGamePad && NoPlayer())
            playerArrayConnected[0] = true;
    }

    /// <summary>
    /// initialise les players (manettes)
    /// </summary>
    private void initController()
    {
        foreach (Player player in ReInput.players.GetPlayers(true))
        {
            foreach (Joystick j in player.controllers.Joysticks)
            {
                setPlayerController(player.id, true);
                break;
            }
        }
    }
    #endregion

    #region core script

    /// <summary>
    /// actualise le player ID si il est connecté ou déconnecté
    /// </summary>
    /// <param name="id">id du joueur</param>
    /// <param name="isConnected">statue de connection du joystick</param>
    private void setPlayerController(int id, bool isConnected)
    {
        playerArrayConnected[id] = isConnected;
    }

    private void updatePlayerController(int id, bool isConnected)
    {
        playerArrayConnected[id] = isConnected;
    }

    /// <summary>
    /// renvoi s'il n'y a aucun player de connecté
    /// </summary>
    /// <returns></returns>
    public bool NoPlayer()
    {
        for (int i = 0; i < playerArrayConnected.Length; i++)
        {
            if (playerArrayConnected[i])
                return (false);
        }
        return (true);
    }
    public int getNbPlayer()
    {
        int nb = 0;
        for (int i = 0; i < playerArrayConnected.Length; i++)
        {
            if (playerArrayConnected[i])
                nb++;
        }
        return (nb);
    }

    /// <summary>
    /// get id of player
    /// </summary>
    /// <param name="id"></param>
    public Player getPlayer(int id)
    {
        if (id == -1)
            return (ReInput.players.GetSystemPlayer());
        else if (id >= 0 && id < playerNumber)
            return (playersRewired[id]);
        Debug.LogError("problème d'id");
        return (null);
    }
    /// <summary>
    /// renvoi vrai si n'importe quel gamePad/joueur active
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public bool getButtonDownFromAnyGamePad(string action)
    {
        for (int i = 0; i < playersRewired.Length; i++)
        {
            if (playersRewired[i].GetButtonDown(action))
                return (true);
        }
        return (false);
    }

    /// <summary>
    /// set les vibrations du gamepad
    /// </summary>
    /// <param name="id">l'id du joueur</param>
    public void setVibrationPlayer(int id, int motorIndex = 0, float motorLevel = 1.0f, float duration = 1.0f)
    {
        if (!enabledVibration)
            return;
        getPlayer(id).SetVibration(motorIndex, motorLevel, duration);
    }


    #endregion

    #region unity fonction and ending

    /// <summary>
    /// a controller is connected
    /// </summary>
    /// <param name="args"></param>
    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        Debug.Log("A controller was connected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        updatePlayerController(args.controllerId, true);

        EventManager.TriggerEvent(GameData.Event.GamePadConnectionChange, true, args.controllerId);
    }

    /// <summary>
    /// a controller is disconnected
    /// </summary>
    void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
    {
        Debug.Log("A controller was disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        updatePlayerController(args.controllerId, false);
        setKeyboardForPlayerOne();

        EventManager.TriggerEvent(GameData.Event.GamePadConnectionChange, false, args.controllerId);
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        ReInput.ControllerConnectedEvent -= OnControllerConnected;
        ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
    }
    #endregion
}
