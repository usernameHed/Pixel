using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// AnimatorEnd Description
/// </summary>
public class AnimatorEnd : MonoBehaviour
{
    #region Attributes

    [FoldoutGroup("Object"), Tooltip("opti fps"), SerializeField]
    private Animator anim;
    [FoldoutGroup("Object"), Tooltip("opti fps"), SerializeField]
    private AnimController animController;

    #endregion

    #region Initialization

    #endregion

    #region Core
    /// <summary>
    /// change une variable de l'animator
    /// </summary>
    public void StopJump()
    {
        anim.SetBool("Jump", false);
    }

    public void StopSwitch()
    {
        anim.SetBool("switch_direc", false);
        animController.DirectionChanged();
    }
    #endregion

    #region Unity ending functions

    #endregion
}
