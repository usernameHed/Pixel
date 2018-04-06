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

    [FoldoutGroup("GamePlay"), Tooltip("list des layer de collisions"), SerializeField]
    private float turnRateArrow = 400f;

    [FoldoutGroup("GamePlay"), Tooltip("temps d'attend avan tle restart"), SerializeField]
    private float timeWaitBeforeDie = 2f;

    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on jump"), SerializeField]
    private Vibration onDie;
    
    [FoldoutGroup("Repulse"), Tooltip("cooldown de repulsion"), SerializeField]
    private FrequencyCoolDown coolDownParent; //O.2

    [FoldoutGroup("Object"), Tooltip("direction du joystick"), SerializeField]
    private Transform dirArrow;
    [FoldoutGroup("Object"), Tooltip("bag du squirell"), SerializeField]
    private Transform bag;


    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private WorldCollision worldCollision;

    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Rigidbody rb;           //ref du rb

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

    public void SetPlayerParent(Transform other)
    {
        if (!coolDownParent.IsReady())
            return;
        coolDownParent.StartCoolDown();

        return;


        if (other == null && transform.parent != null)
        {
            transform.SetParent(other);
            transform.rotation = Quaternion.identity;
            //transform.localScale = new Vector3(1, 1, 1);
        }
        else if (other != null && transform.parent != other.transform)
        {
            transform.SetParent(null);
            transform.rotation = Quaternion.identity;

            transform.SetParent(other);
            transform.rotation = Quaternion.identity;
            //transform.localScale = new Vector3(1, 1, 1);

        }

        
            
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
        dirArrow.rotation = QuaternionExt.DirObject(dirArrow.rotation, worldCollision.GetLastPersistSumNormalSafe().x, -worldCollision.GetLastPersistSumNormalSafe().y, turnRateArrow, QuaternionExt.TurnType.Z);
        //anim.transform.rotation = QuaternionExt.DirObject(anim.transform.rotation, normalCollide.x, -normalCollide.y, turnRateArrow, QuaternionExt.TurnType.Z);
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
        SoundManager.GetSingleton.playSound(GameData.Sounds.Explode.ToString() + transform.GetInstanceID());

        enabledObject = false;

        GameManager.Instance.SceneManagerLocal.PlayNext(timeWaitBeforeDie);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, StopAction);
    }
    #endregion
}