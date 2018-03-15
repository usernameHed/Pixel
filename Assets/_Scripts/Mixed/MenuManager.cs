using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/// <summary>
/// MenuManager Description
/// </summary>
public class MenuManager : MonoBehaviour, ILevelManager
{
    #region Attributes

    [FoldoutGroup("Objects"), Tooltip("Debug"), SerializeField]
    private List<Button> buttonsMainMenu;



    private bool enabledScript = false;
    #endregion

    #region Initialization
    /// <summary>
    /// est appelé depuis le GameManager depuis l'interface
    /// à l'initialisation...
    /// </summary>
    public void InitScene()
    {
        enabledScript = true;
    }

    #endregion

    #region Core

    /// <summary>
    /// ici lance le jeu, il est chargé !
    /// </summary>
    [FoldoutGroup("Debug"), Button("Play")]
    public void Play()
    {
        Debug.Log("play ici menu");
        GameManager.Instance.SceneManagerLocal.PlayNext();
    }

    [FoldoutGroup("Debug"), Button("Quit")]
    public void Quit()
    {
        enabledScript = false;
        buttonsMainMenu[1].Select();

        SceneManagerGlobal.Instance.QuitGame(true);
    }

    public void InputLevel()
    {
        if (PlayerConnected.Instance.getPlayer(-1).GetButtonDown("Escape")
           || PlayerConnected.Instance.getButtonDownFromAnyGamePad("Back"))
        {
            Quit();
        }
    }

    /// <summary>
    /// est appelé pour débug le clique
    /// Quand on clique avec la souris: reselect le premier bouton !
    /// </summary>
    private void DebugMouseCLick()
    {
        if (!enabledScript)
            return;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            buttonsMainMenu[0].Select();
        }
    }
    #endregion

    #region Unity ending functions
    private void Update()
    {
        if (!enabledScript)
            return;
        InputLevel();
        DebugMouseCLick();
    }
    #endregion
}
