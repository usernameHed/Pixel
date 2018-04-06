using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerPhysics : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    private float fallMultiplier = 1.0f;

    [Space(10)]

    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private PlayerJump playerJump;
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
        //ici c'est la première fois qu'on touche plus le sol, alors que on a pas sauté ! faire quelque chose !
        attractor.SetUpAttractPoint();
    }

    private void ApplyGravity()
    {
        //si le player n'est pas grounded... et qu'on a pas sauté de nous même...
        if (!worldCollision.IsGroundedSafe() && !playerJump.IsJumping())
        {
            NotGroundedNorJumped();
            attractor.SetNewNormalForceWhenFlying();
            //Debug.Log("ici applique la gravité, on tombe !");
            PhysicsExt.ApplyConstForce(rb, worldCollision.GetSumNormalSafe(), fallMultiplier);
        }
        else if (worldCollision.IsGroundedSafe())
        {
            //applique la physique quand on est grounded !
            PhysicsExt.ApplyConstForce(rb, worldCollision.GetSumNormalSafe(), fallMultiplier);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void FixedUpdate ()
	{
        if (worldCollision.IsGroundedSafe())    //ici, si on est sur le sol...
            IsGrounded();

        ApplyGravity();
    }
    #endregion
}
