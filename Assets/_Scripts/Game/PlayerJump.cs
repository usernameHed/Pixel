using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerJump : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("Jump"), Tooltip("hauteur maximal du saut"), SerializeField]
    private float jumpForce = 2.0f;
    [FoldoutGroup("Jump"), SerializeField]
    private float gravity = 9.81f;
    [FoldoutGroup("GamePlay"), Tooltip("jumper constament en restant appuyé ?"), SerializeField]
    private bool stayHold = true;

    [FoldoutGroup("Jump"), Tooltip("cooldown du jump"), SerializeField]
    private FrequencyCoolDown coolDownJump;
    [FoldoutGroup("Jump"), Tooltip("vibration quand on jump"), SerializeField]
    private Vibration onJump;
    [FoldoutGroup("Jump"), Tooltip("vibration quand on se pose"), SerializeField]
    private Vibration onGrounded;


    [FoldoutGroup("Jump"), Tooltip(""), SerializeField]
    private float margeHoriz = 0.1f;

    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private WorldCollision worldCollision;
    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private PlayerController playerController;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private InputPlayer inputPlayer;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Rigidbody rb;           //ref du rb

    private bool jumpStop = false;      //forcer l'arrêt du jump, pour forcer le joueur a lacher la touche
    private bool hasJumpAndFlying = false;   //a-t-on juste jumpé ?

    private bool stopAction = false;    //le joueur est-il stopé ?
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
        jumpStop = false;
        hasJumpAndFlying = false;
    }
    #endregion

    #region Core
    /// <summary>
    /// renvoi vrai ou faux si on a le droit de sauter (selon le hold)
    /// </summary>
    /// <returns></returns>
    public bool CanJump()
    {
        //faux si on hold pas et quand a pas laché
        if (jumpStop && !stayHold)
            return (false);
        //faux si le cooldown n'est pas fini
        if (!coolDownJump.IsReady())
            return (false);
        return (true);
    }

    /// <summary>
    /// ici essai de jump (est appelé a chaque frame)
    /// </summary>
    private void TryToJump()
    {
        if (!CanJump())
            return;

        if (worldCollision.IsGroundedSafe())
        {
            if (inputPlayer.JumpInput)
            {
                Vector3 finalVelocityDir = GetDirWhenJumpAndMoving();   //get la direction du joystick / normal / les 2...

                //ici jump, selon la direction voulu, en ajoutant la force du saut
                SetupJumpFromPlayerController(finalVelocityDir, true);
            }
        }
        else
        {
            //not grounded
            if (inputPlayer.JumpInput)
            {
                Debug.Log("ici on saute pas dans le vide...");
            }
        }
    }

    /// <summary>
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

    /// <summary>
    /// renvoi si oui ou non on est en train de jump
    /// </summary>
    /// <returns></returns>
    public bool IsJumping()
    {
        return (hasJumpAndFlying);
    }

    /// <summary>
    /// est appelé a chaque fois qu'il est grounded
    /// </summary>
    /// <param name="other">le type de sol</param>
    public void IsGrounded()
    {
        //ici gère si on vient d'atterrrire...
        if (IsJumping())
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
        hasJumpAndFlying = false;
        PlayerConnected.Instance.setVibrationPlayer(playerController.IdPlayer, onGrounded);
    }

    private float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpForce * gravity);
    }


    /// <summary>
    /// retourne la direction quand on saute...
    /// </summary>
    /// <returns></returns>
    private Vector3 GetDirWhenJumpAndMoving()
    {
        Vector3 finalVelocityDir = Vector3.zero;

        //get la direction du joystick de visé
        Vector3 dirArrowPlayer = playerController.GetDirArrow();

        //get le dot product normal -> dir Arrow
        float dotDirPlayer = QuaternionExt.DotProduct(worldCollision.GetSumNormalSafe(), dirArrowPlayer);

        //si positif, alors on n'a pas à faire de mirroir
        if (dotDirPlayer > margeHoriz)
        {
            //direction visé par le joueur
            Debug.Log("Direction de l'arrow !" + dotDirPlayer);
            finalVelocityDir = dirArrowPlayer.normalized;
        }
        else if (dotDirPlayer < -margeHoriz)
        {
            //ici on vise dans le négatif, faire le mirroir du vector par rapport à...
            Debug.Log("ici mirroir de l'arrow !" + dotDirPlayer);

            //récupéré le vecteur de DROITE de la normal
            Vector3 rightVector = QuaternionExt.CrossProduct(worldCollision.GetSumNormalSafe(), Vector3.forward) * -1;
            //Debug.DrawRay(transform.position, rightVector.normalized, Color.blue, 1f);

            //faire le mirroir entre la normal et le vecteur de droite
            Vector3 mirror = QuaternionExt.ReflectionOverPlane(dirArrowPlayer, rightVector * -1) * -1;
            //Debug.DrawRay(transform.position, mirror.normalized, Color.yellow, 1f);

            //direction inverse visé par le joueur
            finalVelocityDir = mirror.normalized;
        }
        else
        {
            /*
            Debug.Log("ici on est proche du 90°, faire la bisection !");
            //ici l'angle normal - direction est proche de 90°, ducoup on fait le milieu des 2 axe
            //ici faire la moyenne des 2 vecteur normal, et direction arrow
            finalVelocityDir = QuaternionExt.GetMiddleOf2Vector(normalCollide, dirArrowPlayer);
            */

            //direction visé par le joueur
            Debug.Log("Direction de l'arrow !" + dotDirPlayer);
            finalVelocityDir = dirArrowPlayer.normalized;
        }
        return (finalVelocityDir);
    }

    /// <summary>
    /// jump à une direction donnée
    /// </summary>
    /// <param name="dir">direction du jump</param>
    /// <param name="automaticlyLaunchJump">si oui, appelle le jump du jumpScript, sinon, attendre...</param>
    private void SetupJumpFromPlayerController(Vector3 dir, bool automaticlyLaunchJump)
    {
        worldCollision.HasJustJump();   //cooldown des worldCollision
        worldCollision.CoolDownGroundedJump.StartCoolDown();

        if (automaticlyLaunchJump)
            Jump(dir);
    }

    /// <summary>
    /// on provoque un jump depusi le code
    /// </summary>
    /// <param name="applyThisForce">si vrai: on change manuellement la velocité du jump</param>
    /// <param name="force">si applyThisForce vrai: on change manuellement force du jump à force</param>
    public void JumpFromCollision(Vector3 dir)
    {
        SetupJumpFromPlayerController(dir, true);
    }
    public void JumpFromCollision(bool applyThisForce = false, float force = 0)
    {
        SetupJumpFromPlayerController(Vector3.zero, false);
        Jump(worldCollision.GetSumNormalSafe(), applyThisForce, force);
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

        TryToJump();
    }

    private void Update()
    {
        //on lache, on autorise le saut encore
        if (inputPlayer.JumpUpInput)
            jumpStop = false;

        if (worldCollision.IsGroundedSafe() && worldCollision.CoolDownGroundedJump.IsReady())    //ici, si on est sur le sol...
            IsGrounded();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, StopAction);
    }
    #endregion
}