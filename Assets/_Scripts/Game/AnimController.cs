using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections;

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
    private Transform parentAnim;
    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private WorldCollision worldCollision;

    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private FrequencyCoolDown coolDownGoToJumpWitoutJumping;

    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private PlayerJump playerJump;
    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private PlayerDash playerDash;

    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private Transform trail;
    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private Transform ear;
    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private Transform dirArrow;

    private float speedInput = 0;
    private Vector3 refMove;

    private bool hasChanged = false;

    private bool waitingForJumpBool = false;
    #endregion

    #region Initialization

    private void Start()
    {
		// Start function
    }
    #endregion

    #region Core
    /// <summary>
    /// ici move (turn ?) ou stop
    /// </summary>
    /// <param name="moveDir"></param>
    /// <param name="rightMove"></param>
    /// <param name="speed"></param>
    public void Turn(Vector3 moveDir, bool rightMove, float speed)
    {
        if (speed == 0)
        {
            speedInput = 0;
            return;
        }

        if (right != rightMove)
            hasChanged = true;
        //anim["Turn"].speed = speed;
        right = rightMove;
        speedInput = Mathf.Abs(speed);
        refMove = moveDir;
        //Debug.Log("speed: " + speedInput);

    }
    #endregion

    #region Unity ending functions

    /// <summary>
    /// ici gère les anim grounded
    /// </summary>
    private void GroundedAnim()
    {
        if (speedInput > 0f)
        {
            Vector3 dir = QuaternionExt.CrossProduct(refMove, Vector3.forward);
            dir = (right) ? dir : -dir;

            if (hasChanged)
            {
                hasChanged = false;
                anim.SetBool("switch_direc", true);
                anim.Play("switch_direc");
                //parentAnim.transform.localScale = new Vector3((right) ? 1 : -1, 1, 1);
            }
            else if (speedInput < 0.5f)
            {
                //ici anim de marche
                Debug.Log("ici walk ?");
                anim.SetBool("walk", true);
            }
            else if (speedInput >= 0.5f)
            {
                //ici anim de course
                Debug.Log("ici run ?");
                anim.SetBool("run", true);
            }

            /*if (anim.GetBool("switch_direc"))
            {
                //AnimationClip animation = anim.GetAnimationClipFromAnimatorByName("switch_direc");

                //animTurn.averageDuration = speedInput;
                anim.speed = speedInput;
                //animTurn["switch_direc"].speed = 2.0f;
            }
            else
            {
                anim.speed = 1f;
            }*/



            //trail.rotation = QuaternionExt.DirObject(trail.rotation, dir.x, -dir.y, speedTurn * speedInput, QuaternionExt.TurnType.Z);
        }
        else
        {
            if (anim.GetBool("run"))
            {
                anim.SetBool("run", false);
            }
            if (anim.GetBool("walk"))
            {
                anim.SetBool("walk", false);
            }
        }
    }

    private void HandleAnim()
    {
        if (worldCollision.IsGroundedSafe() && worldCollision.CoolDownGroundedJump.IsReady())
        {
            GroundedAnim();
            coolDownGoToJumpWitoutJumping.Reset();
            waitingForJumpBool = false;
            anim.SetBool("jump", false);
            anim.SetBool("dash", false);
        }
        else
        {
            if (!playerJump.IsJumping() && !playerDash.IsDashing() && worldCollision.CoolDownGroundedJump.IsReady() && !waitingForJumpBool)
            {
                coolDownGoToJumpWitoutJumping.StartCoolDown();
                waitingForJumpBool = true;
            }
            if (!playerJump.IsJumping() && !playerDash.IsDashing() && waitingForJumpBool && coolDownGoToJumpWitoutJumping.IsReady()
                && !anim.GetCurrentAnimatorStateInfo(0).IsName("jump"))
            {
                waitingForJumpBool = false;
                anim.speed = 1;
                anim.Play("jump");
            }
        }
    }

    /// <summary>
    /// appelé quand on vient d'atterir
    /// </summary>
    public void JustGroundedJump()
    {
        anim.SetBool("jump", false);
        anim.Play("jump");
    }

    /// <summary>
    /// appelé quand on vient d'atterir
    /// </summary>
    public void JustGroundedDash()
    {
        anim.SetBool("dash", false);
        anim.Play("jump");
    }

    /// <summary>
    /// ici on jump !
    /// </summary>
    public void JustJump()
    {
        coolDownGoToJumpWitoutJumping.Reset();
        anim.SetBool("jump", true);
    }
    /// <summary>
    /// ici on jump !
    /// </summary>
    public void JustDash()
    {
        coolDownGoToJumpWitoutJumping.Reset();
        anim.SetBool("dash", true);
        anim.Play("dash");
    }

    private void Update()
    {
        HandleAnim();

        parentAnim.rotation = dirArrow.rotation;
        //anim.transform.rotation = Quaternion.AngleAxis(90, dirArrow.eulerAngles);
        ear.rotation = dirArrow.rotation;
    }

    public void DirectionChanged()
    {
        anim.Play("idle");
        
        StartCoroutine(ChangeDirectionAnim());
    }
    private IEnumerator ChangeDirectionAnim()
    {
        yield return new WaitForEndOfFrame();
        parentAnim.transform.localScale = new Vector3((right) ? 1 : -1, 1, 1);
    }
    #endregion
}
