using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using System;
using System.Collections.Generic;

/// <summary>
/// LevelManager Description
/// </summary>
public class LevelManager : MonoBehaviour, ILevelManager
{
    #region Attributes
    public GameObject player;
    public PlayerBag bag;

    [FoldoutGroup("Debug"), Tooltip("gere le temps avant de pouvoir faire Restart"), SerializeField]
    private FrequencyTimer coolDownRestart;

    public GameObject[] checkpoint;
    public Transform[] pointRespawn;
    public int checkpointNumber = 0;

    private bool enabledScript = true;
    #endregion

    #region Initialization

    private void OnEnable()
    {
        
    }

    private void Awake()
    {
        coolDownRestart.Ready();
    }

    /// <summary>
    /// est appelé depuis le GameManager depuis l'interface
    /// à l'initialisation... 
    /// </summary>
    public void InitScene()
    {
        enabledScript = true;
        LevelInit();
    }
    #endregion

    #region Core
    
    public void LevelInit()
    {
        Debug.Log("level init");
        int scorePlayer = ScoreManager.Instance.Data.scorePlayer;
        for (int i = 0; i < scorePlayer; i++)
        {
            checkpoint[i].SetActive(false);
            //bag.noisertteArray[i].SetActive(true);
            //bag.numberNoisette++;
        }
        bag.SetNoisetteOnCheckpoint(scorePlayer);

        if (scorePlayer > 0)
        {
            player.transform.position = pointRespawn[scorePlayer - 1].position;
            //player.transform.rotation = pointRespawn[scorePlayer - 1].rotation;
        }

        SoundManager.GetSingleton.playSound("event:/Music/BackgroundMusic");
    }

    public void InputLevel()
    {
        if (PlayerConnected.Instance.getPlayer(-1).GetButtonDown("Escape")
            || PlayerConnected.Instance.getButtonDownFromAnyGamePad("Back"))
        {
            Quit();
        }
        if (PlayerConnected.Instance.getPlayer(-1).GetButtonDown("Restart")
            || PlayerConnected.Instance.getButtonDownFromAnyGamePad("Restart"))
        {
            Restart();
        }
    }

    public void Play()
    {

    }

    /// <summary>
    /// restart le jeu
    /// </summary>
    [Button("Restart")]
    public void Restart()
    {
        if (!coolDownRestart.Ready())
            return;

        ObjectsPooler.Instance.desactiveEveryOneForTransition();
        ObjectsPoolerLocal.Instance.desactiveEveryOneForTransition();
        //LevelInit();

        //GameManager.GetSingleton.RestartGame(true);
        GameManager.Instance.SceneManagerLocal.PlayNext();
    }

    [Button("Quit")]
    public void Quit()
    {
        if (!enabledScript)
            return;
        if (!coolDownRestart.Ready())
            return;

        enabledScript = false;

        ObjectsPooler.Instance.desactiveEveryOneForTransition();
        ObjectsPoolerLocal.Instance.desactiveEveryOneForTransition();

        GameManager.Instance.SceneManagerLocal.PlayPrevious(false);


    }

    #endregion

    #region Unity ending functions
    private void Update()
    {
        InputLevel();
    }

    private void OnDisable()
    {
        
    }
    #endregion
}
