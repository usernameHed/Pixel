using Sirenix.OdinInspector;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BetterJump : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), OnValueChanged("InitValue"), Tooltip("hauteur maximal du saut"), SerializeField]
    private float jumpForce = 2.0f;
    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    private float gravity = 9.81f;
    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    private float fallMultiplier = 1.0f;
    public float FallMultiplier { get { return (fallMultiplier); } }
    [FoldoutGroup("GamePlay"), Tooltip("jumper constament en restant appuyé ?"), SerializeField]
    private bool stayHold = false;
    [Space(10)]
    [FoldoutGroup("GamePlay"), Tooltip("cooldown du jump"), SerializeField]
    private FrequencyCoolDown coolDownJump;

    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on jump"), SerializeField]
    private Vibration onJump;
    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on se pose"), SerializeField]
    private Vibration onGrounded;


    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private InputPlayer inputPlayer;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private PlayerController playerController;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Attractor attractor;
    public Attractor AttractorScript { get { return (attractor); } }

    private Vector3 initialVelocity;

    private bool jumpStop = false;      //forcer l'arrêt du jump, pour forcer le joueur a lacher la touche
    private bool hasJumpAndFlying = false;   //a-t-on juste jumpé ?

    #endregion

    #region Initialize
    private void Awake()
    {
        InitValue();
    }

    private void InitValue()
    {
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

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpForce * gravity);
    }

    

    /// <summary>
    /// est appelé a chaque fois qu'il est grounded
    /// </summary>
    /// <param name="other">le type de sol</param>
    public void OnGrounded()
    {
        //ici gère si on vient d'atterrrire...
        if (hasJumpAndFlying)
        {
            //playerController.Anim.SetBool("Jump", false);
            Debug.Log("ici just grounded");
            hasJumpAndFlying = false;
            PlayerConnected.Instance.setVibrationPlayer(playerController.IdPlayer, onGrounded);
        }
        attractor.SaveLastPositionOnground(); //ici save la position, et se reset !

        
    }

    /// <summary>
    /// est appelé a chaque fois qu'il n'est pas grounded, et qu'on a pas sauté
    /// </summary>
    private void NotGroundedNorJumped()
    {
        //ici c'est la première fois qu'on touche plus le sol, alors que on a pas sauté ! faire quelque chose !
        attractor.SetUpAttractPoint();
    }

    private void ApplyGravity()
    {
        rb.velocity += playerController.NormalCollide * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        //Debug.DrawRay(transform.position, playerController.NormalCollide, Color.magenta, 1f);
    }
    public void ApplyGravity(Vector3 dir, float force = 1)
    {
        rb.velocity += dir * force * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        //Debug.DrawRay(transform.position, playerController.NormalCollide, Color.magenta, 1f);
    }

    private void FixedUpdate ()
	{
        //si le player n'est pas grounded... et qu'on a pas sauté de nous même...
        if (!playerController.Grounded && !hasJumpAndFlying)
        {
            
            NotGroundedNorJumped();
            attractor.SetNewNormalForceWhenFlying();
            //Debug.Log("ici applique la gravité, on tombe !");
            ApplyGravity(); //ici enlever ??
        }
        else if (playerController.Grounded)
        {
            //applique la physique quand on est grounded !
            ApplyGravity();
        }
    }

    private void Update()
    {
        //on lache, on autorise le saut encore
        if (inputPlayer.JumpUpInput)
            jumpStop = false;
    }

    #endregion
}
