using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerDash : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private float dashForce = 30.0f;
    [FoldoutGroup("Jump"), SerializeField]
    private float gravity = 9.81f;
    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private float airMoveDash = 30.0f;
    public float AirMoveDash { get { return (airMoveDash); } }

    [FoldoutGroup("Jump"), Tooltip("cooldown du jump"), SerializeField]
    private FrequencyCoolDown coolDownJumpDash;

    [FoldoutGroup("Jump"), Tooltip("vibration quand on jump"), SerializeField]
    private Vibration onDash;
    
    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private PlayerMove playerMove;
    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private WorldCollision worldCollision;
    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private PlayerController playerController;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private InputPlayer inputPlayer;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private PlayerJump playerJump;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Rigidbody rb;           //ref du rb

    private Vector3 lastDirPos;
    
    private bool stopAction = false;    //le joueur est-il stopé ?
    private bool hasDashed = false;   //a-t-on juste jumpé ?
    #endregion

    #region Initialize
    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.GameOver, StopAction);
        InitPlayer();
    }

    /// <summary>
    /// init le player
    /// </summary>
    private void InitPlayer()
    {
        stopAction = false;
        hasDashed = false;
    }
    #endregion

    #region Core
    /// <summary>
    /// renvoi vrai ou faux si on a le droit de sauter (selon le hold)
    /// </summary>
    /// <returns></returns>
    public bool CanDash()
    {
        //faux si le cooldown n'est pas fini
        if (!coolDownJumpDash.IsReady() || hasDashed)
            return (false);
        return (true);
    }

    public void SaveLastDirInput(Vector3 inputDir)
    {
        lastDirPos = inputDir;
    }

    /// <summary>
    /// ici essai de jump (est appelé a chaque frame)
    /// </summary>
    private void TryToDash()
    {
        if (!CanDash())
            return;

        if (worldCollision.IsGroundedSafe())
        {
            if (inputPlayer.DashInput > 0)
            {
                Vector3 finalVelocityDir = GetDirWhenDashing();   //get la direction du joystick / normal / les 2...

                //ici jump, selon la direction voulu, en ajoutant la force du saut
                ActuallyDash(finalVelocityDir, true);
            }
        }
        else
        {
            //not grounded
            if (inputPlayer.DashInput > 0)
            {
                //Debug.Log("ici on saute pas dans le vide...");
                Vector3 finalVelocityDir = GetDirWhenDashingInAir();   //get la direction du joystick / normal / les 2...

                //ici jump, selon la direction voulu, en ajoutant la force du saut
                ActuallyDash(finalVelocityDir, true);
            }
        }
    }

    public void Dash(Vector3 dir, bool applyThisForce = false, float force = 0)
    {
        Debug.Log("dash !");
        //s'il n'y a pas de direction, erreur ???
        if (dir == Vector3.zero)
        {
            dir = Vector3.up;
            //ici pas de rotation ?? 
            Debug.Log("pas de rotation ! up de base !");
        }
        Debug.Log("ici dash !");

        //playerController.Anim.SetBool("Jump", true);

        coolDownJumpDash.StartCoolDown();   //set le coolDown du jump
        PlayerConnected.Instance.setVibrationPlayer(playerController.IdPlayer, onDash); //set vibration de saut
        SoundManager.GetSingleton.playSound(GameData.Sounds.Swouch.ToString() + transform.GetInstanceID());
        GameManager.Instance.CameraObject.GetComponent<ScreenShake>().ShakeCamera();

        hasDashed = true; //on vient de sauter ! tant qu'on retombe pas, on est vrai

        //applique soit la force de saut, soit la force défini dans les parametres
        Vector3 jumpForce = (!applyThisForce) ? dir * CalculateJumpVerticalSpeed() : dir * force;

        rb.velocity = jumpForce;

        Debug.DrawRay(transform.position, jumpForce, Color.red, 5f);
        GameObject particle = ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.ParticleBump, transform.position, Quaternion.identity, ObjectsPooler.Instance.transform);
        particle.transform.rotation = QuaternionExt.LookAtDir(jumpForce * -1);

        return;
    }

    private float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * dashForce * gravity);
    }

    /*/// <summary>
    /// Jump (on est sur de vouloir jump)
    /// </summary>
    /// <param name="dir">direction du jump</param>
    /// <returns>retourne vrai si on a jumpé</returns>
    public bool Jump(Vector3 dir, bool applyThisForce = false, float force = 0)
    {
        //s'il n'y a pas de direction, erreur ???
        if (dir == Vector3.zero)
        {
            dir = Vector3.up;
            //ici pas de rotation ?? 
            Debug.Log("pas de rotation ! up de base !");
        }
        Debug.Log("ici jump !");

        //playerController.Anim.SetBool("Jump", true);

        coolDownJump.StartCoolDown();   //set le coolDown du jump
        PlayerConnected.Instance.setVibrationPlayer(playerController.IdPlayer, onJump); //set vibration de saut

        hasJumpAndFlying = true; //on vient de sauter ! tant qu'on retombe pas, on est vrai

        //applique soit la force de saut, soit la force défini dans les parametres
        Vector3 jumpForce = (!applyThisForce) ? dir * CalculateJumpVerticalSpeed() : dir * force;

        rb.velocity = jumpForce;

        Debug.DrawRay(transform.position, jumpForce, Color.red, 5f);
        GameObject particle = ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.ParticleBump, transform.position, Quaternion.identity, ObjectsPooler.Instance.transform);
        particle.transform.rotation = QuaternionExt.LookAtDir(jumpForce * -1);

        if (!stayHold)
            jumpStop = true;
        return (true);
    }
    */

    /// <summary>
    /// renvoi si oui ou non on est en train de jump
    /// </summary>
    /// <returns></returns>
    public bool IsDashing()
    {
        return (hasDashed);
    }

    /// <summary>
    /// est appelé a chaque fois qu'il est grounded
    /// </summary>
    /// <param name="other">le type de sol</param>
    public void IsGrounded()
    {
        //ici gère si on vient d'atterrrire...
        if (IsDashing())
        {
            Justgrounded();
        }
    }

    /// <summary>
    /// ici on vient d'atterrire
    /// </summary>
    public void Justgrounded()
    {
        //playerController.Anim.SetBool("Jump", false);
        Debug.Log("ici just grounded");
        hasDashed = false;
        PlayerConnected.Instance.setVibrationPlayer(playerController.IdPlayer, playerJump.OnGrounded);
    }

    /// <summary>
    /// retourne la direction quand on saute...
    /// </summary>
    /// <returns></returns>
    private Vector3 GetDirWhenDashing()
    {
        Vector3 finalVelocityDir = Vector3.zero;

        Vector3 dirMovement = playerMove.FindTheRightDir();

        if (dirMovement == Vector3.zero)
        {
            dirMovement = worldCollision.GetSumNormalSafe();
            Debug.Log("ici normal !");
        }

        finalVelocityDir = QuaternionExt.GetMiddleOf2Vector(worldCollision.GetSumNormalSafe(), dirMovement);

        Debug.DrawRay(transform.position, dirMovement, Color.blue, 1f);

        return (finalVelocityDir);
    }

    /// <summary>
    /// retourne la direction quand on saute...
    /// </summary>
    /// <returns></returns>
    private Vector3 GetDirWhenDashingInAir()
    {
        Vector3 finalVelocityDir = Vector3.zero;

        //Vector3 dirMovement = playerMove.FindTheRightDir();
        Vector3 dirMovement = worldCollision.GetLastPersistSumNormalSafe();
        Vector3 lastDir = lastDirPos;

        if (lastDir == Vector3.zero)
        {
            finalVelocityDir = dirMovement;
        }
        else
        {
            finalVelocityDir = QuaternionExt.GetMiddleOf2Vector(dirMovement, lastDir);
        }

        Debug.DrawRay(transform.position, dirMovement, Color.blue, 1f);

        return (finalVelocityDir);
    }

    /// <summary>
    /// jump à une direction donnée
    /// </summary>
    /// <param name="dir">direction du jump</param>
    /// <param name="automaticlyLaunchJump">si oui, appelle le jump du jumpScript, sinon, attendre...</param>
    private void ActuallyDash(Vector3 dir, bool automaticlyLaunchJump)
    {
        worldCollision.HasJustJump();   //cooldown des worldCollision
        worldCollision.CoolDownGroundedJump.StartCoolDown();

        if (automaticlyLaunchJump)
            Dash(dir);
    }

    private void StopAction()
    {
        stopAction = false;
    }

    #endregion

    #region Unity ending functions

    private void FixedUpdate()
    {
        if (stopAction)
            return;

        TryToDash();
    }

    private void Update()
    {
        if (worldCollision.IsGroundedSafe() && worldCollision.CoolDownGroundedJump.IsReady())    //ici, si on est sur le sol...
            IsGrounded();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, StopAction);
    }
    #endregion
}