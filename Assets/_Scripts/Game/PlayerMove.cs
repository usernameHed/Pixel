using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("Move"), Tooltip(""), SerializeField]
    public float maxVelocityChange = 10.0f;
    [FoldoutGroup("Move"), Tooltip(""), SerializeField]
    public float propulseWhenNewGround = 1f;
    [FoldoutGroup("Move"), Tooltip(""), SerializeField]
    public float speed = 10.0f;
    [FoldoutGroup("Move"), Tooltip(""), SerializeField]
    private float debugCloseValueNormal = 0.1f;

    [FoldoutGroup("Debug"), Tooltip("valeur en x et y où une normal est considéré comme suffisament proche d'une autre"), SerializeField]
    private float debugCloseValueAngleInput = 89f;
    [FoldoutGroup("Debug"), Tooltip("Marge de vitesse: quand on appuis sur aucune touche, mais qu'on jump: si notre vitesse vertical est suppérieur à cette valeur, on saute dans la direction de l'arrow (et non de la normal précédente)"), SerializeField]
    private float debugDiferenceAngleBetweenInputAndPlayer = 30f;

    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private WorldCollision worldCollision;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private InputPlayer inputPlayer;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Rigidbody rb;           //ref du rb
    
    private bool stopAction = false;    //le joueur est-il stopé ?
    private Vector3 lastInputDir;
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
    }
    #endregion

    #region Core

    /// <summary>
    /// déplace le player
    /// </summary>
    private void TryToMove()
    {
        if (!(inputPlayer.Horiz == 0 && inputPlayer.Verti == 0))
        {
            if (worldCollision.IsGroundedSafe())
            {
                MoveOnGround();
            }
            else
            {

            }
        }
        else
        {
            //stop le déplacement
            //anim.SetBool("Run", false);
            //Debug.Log("ici stoppé");
        }
    }

    /// <summary>
    /// déplace horizontalement le player
    /// </summary>
    /// <param name="inverse"></param>
    private void MoveOnGround()
    {
        //si on a juste sauté, ne rien faire
        if (!worldCollision.CoolDownGroundedJump.IsReady() || !worldCollision.CoolDownGrounded.IsReady())
        {
            //Debug.Log("NO READY");
            return;
        }
            

        // Calculate how fast we should be moving

        Vector3 inputPlayer = FindTheRightDir();


        Vector3 targetVelocity = inputPlayer;
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;


        
        Debug.DrawRay(transform.position, inputPlayer, Color.yellow, 1f);
        Debug.DrawRay(transform.position, lastInputDir, Color.red, 1f);

        if (!(UtilityFunctions.IsClose(inputPlayer.x, lastInputDir.x, debugCloseValueNormal)
            && UtilityFunctions.IsClose(inputPlayer.y, lastInputDir.y, debugCloseValueNormal)))
        {
            //Debug.Log("ici propulse un peu !");
            //ici propulse un peu si: l'angle de la direction précédente a changé de beaucoup, mais pas de +95;
            float angleInputPlayer = QuaternionExt.GetAngleFromVector(inputPlayer);
            float angleLastInputDir = QuaternionExt.GetAngleFromVector(lastInputDir);

            float diffLastInput;
            if (QuaternionExt.IsAngleCloseToOtherByAmount(angleInputPlayer, angleLastInputDir, debugCloseValueAngleInput, out diffLastInput))
            {
                Debug.Log("ici propulse !");
                rb.AddForce(inputPlayer * propulseWhenNewGround, ForceMode.VelocityChange);
            }

        }
        lastInputDir = inputPlayer;
        

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = 0;

        //si on veut bouger...
        if (velocityChange.x != 0 || velocityChange.y != 0)
        {
            //calcule les 3 angle
            float angleDirInput = QuaternionExt.GetAngleFromVector(targetVelocity); //direction de l'input (droite ou gauche, 0 ou 180)
            float anglePlayer = QuaternionExt.GetAngleFromVector(velocityChange);   //angle de l'inertie du joueur (droite ou gauche, mais peut être l'inverse de l'input !)

            //si on est égal, alors l'intertie en x est la meme que la direction de l'input
            //important, car si l'inverse: alors on laché la touche ?
            float diff;
            if (QuaternionExt.IsAngleCloseToOtherByAmount(angleDirInput, anglePlayer, debugDiferenceAngleBetweenInputAndPlayer, out diff) && worldCollision.GetSumNormalSafe() != Vector3.zero)
            {
                //Debug.Log("move normalement !");
            }
            else if (worldCollision.GetSumNormalSafe() == Vector3.zero)
            {
                Debug.Log("ici la normal est égal à zero...");
            }
            else
            {
                //Debug.Log("ralenti progressivement ??");
            }

            rb.AddForce(velocityChange, ForceMode.VelocityChange);


             //anim.SetBool("Run", true);  //déplacement
        }

        
    }

    /// <summary>
    /// set la direction des inputs selon la normal !
    /// </summary>
    /// <returns></returns>
    public Vector3 FindTheRightDir()
    {
        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(inputPlayer.Horiz, inputPlayer.Verti, 0);
        //si on veut bouger...
        if (targetVelocity.x != 0 || targetVelocity.y != 0)
        {
            Vector3 right = QuaternionExt.CrossProduct(worldCollision.GetSumNormalSafe(), Vector3.forward).normalized;
            float dir = QuaternionExt.DotProduct(targetVelocity, right);

            return (right * dir);
        }
        else
        {
            return (Vector3.zero);
        }
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

        TryToMove();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, StopAction);
    }
    #endregion
}