using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// AnimController Description
/// </summary>
public class AnimController : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private bool right = true;

    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private Animator anim;
    public Animator Anim { get { return (anim); } }

    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private Transform trail;
    #endregion

    #region Initialization

    private void Start()
    {
		// Start function
    }
    #endregion

    #region Core
    public void Turn(bool rightMove, float speed)
    {
        //anim["Turn"].speed = speed;
        right = rightMove;
        Debug.Log("speed: " + speed);

    }
    #endregion

    #region Unity ending functions

	#endregion
}
