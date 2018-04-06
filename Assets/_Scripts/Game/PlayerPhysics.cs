using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerPhysics : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("gravité de base"), SerializeField]
    private float gravityMultiplier = 1.0f;

    [FoldoutGroup("GamePlay"), Tooltip("gravité de base"), SerializeField]
    private float fallMultiplier = 2.5f;
    [FoldoutGroup("GamePlay"), Tooltip("gravité de base"), SerializeField]
    private float lowMultiplier = 2.5f;

    

    [Space(10)]

    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private PlayerJump playerJump;
    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private PlayerDash playerDash;
    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private WorldCollision worldCollision;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private InputPlayer inputPlayer;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private PlayerController playerController;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Attractor attractor;
    public Attractor AttractorScript { get { return (attractor); } }

    #endregion

    #region Initialize

    #endregion

    #region Core
    

    /// <summary>
    /// est appelé a chaque fois qu'il est grounded
    /// </summary>
    /// <param name="other">le type de sol</param>
    public void IsGrounded()
    {
        attractor.SaveLastPositionOnground(); //ici save la position, et se reset !
    }

    /// <summary>
    /// est appelé a chaque fois qu'il n'est pas grounded, et qu'on a pas sauté
    /// </summary>
    private void NotGroundedNorJumped()
    {
        //si on est pas en mode dash, activer l'attractor
        if (/*!playerJump.IsJumping() && */!playerDash.IsDashing())
        {

            //si on est en jump, et qu'on a pas d'attractPoint, en mettre un !
            //mais mettre un coolDown pour l'activer QUE apres un certain temps
            if (playerJump.IsJumping() && !attractor.HasAttractPoint())
            {
                attractor.CoolDownAttractorWhenJump.StartCoolDown();
            }

            //ici c'est la première fois qu'on touche plus le sol, alors que on a pas sauté ! faire quelque chose !
            attractor.SetUpAttractPoint();
        }
    }

    private void ApplyGravity()
    {
        //aucune force quand on dash !
        if (playerDash.IsDashing())
            return;

        //si le player n'est pas grounded... et qu'on a pas sauté de nous même...
        if (!worldCollision.IsGroundedSafe()/* && !playerJump.IsJumping()*/)
        {
            NotGroundedNorJumped();

            attractor.SetNewNormalForceWhenFlying();
            //Debug.Log("ici applique la gravité, on tombe !");

            //ne pas appliquer les autres force quand on a un attract point !
            if (attractor.HasAttractPoint() && attractor.CoolDownAttractorWhenJump.IsReady())
                return;

            if (!inputPlayer.JumpInput)
            {
                Debug.Log("ici gravité fall");
                //rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
                PhysicsExt.ApplyConstForce(rb, worldCollision.GetLastPersistSumNormalSafe(), fallMultiplier);
            }
            /*
            else if (rb.velocity.y > 0 && !inputPlayer.JumpInput)
            {
                PhysicsExt.ApplyConstForce(rb, worldCollision.GetLastPersistSumNormalSafe(), lowMultiplier);
                Debug.Log("ici gravité up");
            }*/

            //Debug.Log("ici gravité normal jump");
            PhysicsExt.ApplyConstForce(rb, worldCollision.GetLastPersistSumNormalSafe(), gravityMultiplier);
        }
        else if (worldCollision.IsGroundedSafe())
        {
            //applique la physique quand on est grounded !
            //Debug.Log("physics ground");
            //Debug.Log("ici ground physics");
            PhysicsExt.ApplyConstForce(rb, worldCollision.GetSumNormalSafe(), gravityMultiplier);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void FixedUpdate ()
	{
        if (worldCollision.IsGroundedSafe() && worldCollision.CoolDownGroundedJump.IsReady())    //ici, si on est sur le sol...
            IsGrounded();

        ApplyGravity();
    }
    #endregion
}
