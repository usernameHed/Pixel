using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IKillable
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("id unique du joueur correspondant à sa manette"), SerializeField]
    private int idPlayer = 0;
    public int IdPlayer { set { idPlayer = value; } get { return idPlayer; } }

    [FoldoutGroup("GamePlay"), Tooltip("est-on un sith ?"), SerializeField]
    private bool isSith = false;
    public bool IsSith { get { return isSith; } }

    [FoldoutGroup("GamePlay"), Tooltip("list des layer de collisions"), SerializeField]
    private float turnRateArrow = 400f;

    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on jump"), SerializeField]
    private Vibration onDie;

    [FoldoutGroup("GamePlay"), Tooltip("Animator du joueur"), SerializeField]
    private Animator anim;
    public Animator Anim { get { return (anim); } }


    [FoldoutGroup("Repulse"), Tooltip(""), SerializeField]
    public bool repulseOtherWhenTouchOnAir = true;
    [FoldoutGroup("Repulse"), Tooltip("cooldown de repulsion"), SerializeField]
    private FrequencyCoolDown coolDownSelfRepulsion; //O.2
    public FrequencyCoolDown CoolDownSelfRepulsion { get { return (coolDownSelfRepulsion); } }

    
    [FoldoutGroup("Object"), Tooltip("direction du joystick"), SerializeField]
    private Transform dirArrow;

    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private SuperPower superPower;
    public SuperPower SuperPowerScript { get { return (superPower); } }

    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private PlayerJump playerJump;
    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private WorldCollision worldCollision;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private InputPlayer inputPlayer;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Rigidbody rb;           //ref du rb
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private PlayerPhysics betterJump;

    private bool enabledObject = true;  //le script est-il enabled ?
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
        rb.freezeRotation = true;

        enabledObject = true;
        stopAction = false;
    }
    #endregion

    #region Core

    /// <summary>
    /// get la direction de l'arrow
    /// </summary>
    /// <returns></returns>
    public Vector3 GetDirArrow()
    {
        Vector3 dirArrowPlayer = QuaternionExt.QuaternionToDir(dirArrow.rotation, Vector3.up);
        //Debug.DrawRay(transform.position, dirArrowPlayer.normalized, Color.yellow, 1f);
        return (dirArrowPlayer);
    }

    /// <summary>
    /// Direction arrow
    /// </summary>
    private void ChangeDirectionArrow()
    {
        if (!(inputPlayer.HorizRight == 0 && inputPlayer.VertiRight == 0) && !isSith)
        {
            dirArrow.rotation = QuaternionExt.DirObject(dirArrow.rotation, inputPlayer.HorizRight, -inputPlayer.VertiRight, turnRateArrow, QuaternionExt.TurnType.Z);
        }
        else
        {
            dirArrow.rotation = QuaternionExt.DirObject(dirArrow.rotation, worldCollision.GetSumNormalSafe().x, -worldCollision.GetSumNormalSafe().y, turnRateArrow, QuaternionExt.TurnType.Z);
        }
        //anim.transform.rotation = QuaternionExt.DirObject(anim.transform.rotation, normalCollide.x, -normalCollide.y, turnRateArrow, QuaternionExt.TurnType.Z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(GameData.Prefabs.SuperPower.ToString()))
        {
            if (!isSith)
            {
                superPower.SetSuperPower();
                ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.DeathPlayer, transform.position, Quaternion.identity, ObjectsPooler.Instance.transform);
                Destroy(other.gameObject);

                //normalCollide = rb.velocity.normalized;
                playerJump.JumpFromCollision();
            }
            else
            {
                Kill();
            }
        }
    }

    /// <summary>
    /// action de collisions
    /// renvoi vrai si on force le jump, faux si on jump pas
    /// </summary>
    private bool CollisionAction(GameObject other)
    {
        //ici 2 BB8 se touche...
        if (!isSith && other.HasComponent<PlayerController>() && !other.GetComponent<PlayerController>().IsSith)
        {
            //ici on est en l'air (ou plutot... si on a qu'une seul normal... ça veut dire qu'il y a des chance
            //que la seul collision qu'on ai, c'est celui avec le player (donc on est en l'air)
            if (/*IsOnlyOneNormal() &&*/ coolDownSelfRepulsion.IsReady() && repulseOtherWhenTouchOnAir)
            {
                /*
                //si l'autre n'est pas en l'air, set son coolDown pour pas qu'il saute aussi !
                if (other.GetComponent<PlayerController>().Grounded)
                {
                    other.GetComponent<PlayerController>().CoolDownSelfRepulsion.StartCoolDown();
                }
                */
                coolDownSelfRepulsion.StartCoolDown();

                Vector3 jumpDir = -(other.transform.position - transform.position).normalized;
                playerJump.JumpFromCollision(jumpDir);                    //saute !
                return (true);
            }

        }
        //si je suis un sith, et que l'autre n'en ai pas un...
        else if (isSith && other.HasComponent<PlayerController>() && !other.GetComponent<PlayerController>().IsSith)
        {
            //s'il n'est pas en mode superpower, on le tue !
            if (!other.GetComponent<PlayerController>().SuperPowerScript.SuperPowerActived)
            {
                other.GetComponent<PlayerController>().Kill();
            }
            else
            {
                other.GetComponent<PlayerController>().Kill();
                Kill();
            }
            
        }
        return (false);
    }

    private void StopAction()
    {
        stopAction = false;
    }

    #endregion

    #region Unity ending functions
    private void Update()
    {
        if (stopAction)
            return;

        ChangeDirectionArrow();
    }

    /// <summary>
    /// tue le joueur
    /// </summary>
    public void Kill()
    {
        if (!enabledObject)
            return;

        StopAction();
        GameManager.Instance.CameraObject.GetComponent<ScreenShake>().ShakeCamera();
        ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.DeathPlayer, transform.position, Quaternion.identity, ObjectsPooler.Instance.transform);
        PlayerConnected.Instance.setVibrationPlayer(idPlayer, onDie);
        enabledObject = false;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, StopAction);
    }
    #endregion
}