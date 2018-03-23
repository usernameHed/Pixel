using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BetterJump : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), OnValueChanged("InitValue"), Tooltip("hauteur maximal du saut"), SerializeField]
    private float jumpHeight = 2.0f;
    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    private float gravity = 9.81f;
    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    private float fallMultiplier = 1.0f;
    [FoldoutGroup("GamePlay"), Tooltip("jumper constament en restant appuyé ?"), SerializeField]
    private bool stayHold = false;
    [Space(10)]
    [FoldoutGroup("GamePlay"), Tooltip("cooldown du jump"), SerializeField]
    private FrequencyCoolDown coolDownJump;


    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private InputPlayer inputPlayer;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private PlayerController playerController;

    private Vector3 initialVelocity;

    private bool jumpStop = false;
    //public bool JumpStop { get { return (jumpStop); } }

    #endregion

    #region Initialize
    private void Awake()
    {
        InitValue();
    }

    private void InitValue()
    {
        jumpStop = false;
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
    /// jump !
    /// </summary>
    public bool Jump(Vector3 dir)
    {
        coolDownJump.StartCoolDown();

        Vector3 jumpForce = dir * CalculateJumpVerticalSpeed();
        rb.velocity = jumpForce;

        if (!stayHold)
            jumpStop = true;
        return (true);
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    private void FixedUpdate ()
	{
        if (!playerController.Grounded)
        {
            Debug.Log("retourne sur terre !");
            rb.velocity += playerController.NormalCollide * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            Debug.DrawRay(transform.position, playerController.NormalCollide, Color.magenta, 1f);
        }
        //rb.velocity
        /*
        if (rb.velocity.y < 0)
		{
            
		}
        else if (rb.velocity.y > 0 && !inputPlayer.JumpInput)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowMultiplier - 1) * Time.fixedDeltaTime;
        }
        */
    }

    private void Update()
    {
        //on lache, on autorise le saut encore
        if (inputPlayer.JumpUpInput)
            jumpStop = false;
    }

    #endregion
}
