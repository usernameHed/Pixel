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
    private float speedTurn = 3f;

    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private Animator anim;
    public Animator Anim { get { return (anim); } }

    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private Transform trail;
    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private Transform ear;
    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private Transform dirArrow;

    private float speedInput = 1;
    private Vector3 refMove;
    #endregion

    #region Initialization

    private void Start()
    {
		// Start function
    }
    #endregion

    #region Core
    public void Turn(Vector3 moveDir, bool rightMove, float speed)
    {
        //anim["Turn"].speed = speed;
        right = rightMove;
        speedInput = Mathf.Abs(speed);
        refMove = moveDir;
        Debug.Log("speed: " + speedInput);

    }
    #endregion

    #region Unity ending functions
    private void Update()
    {
        if (speedInput > 0.1f)
        {
            Vector3 dir = QuaternionExt.CrossProduct(refMove, Vector3.forward);
            dir = (right) ? dir : -dir;

            trail.rotation = QuaternionExt.DirObject(trail.rotation, dir.x, -dir.y, speedTurn * speedInput, QuaternionExt.TurnType.Z);
        }
        ear.rotation = dirArrow.rotation;
    }
    #endregion
}
