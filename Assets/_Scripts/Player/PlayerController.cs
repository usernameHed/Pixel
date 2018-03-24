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
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float speed = 10.0f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float gravity = 9.81f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float maxVelocityChange = 10.0f;

    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public bool repulseOtherWhenTouchOnAir = true;
    [FoldoutGroup("GamePlay"), Tooltip("cooldown de repulsion"), SerializeField]
    private FrequencyCoolDown coolDownSelfRepulsion; //O.2
    public FrequencyCoolDown CoolDownSelfRepulsion { get { return (coolDownSelfRepulsion); } }

    [FoldoutGroup("GamePlay"), Tooltip("défini si on est en wallJump ou pas selon la différence normal / variable wallJump 0 - 180"), SerializeField]
    public float angleDifferenceWall = 10f;
    [FoldoutGroup("GamePlay"), Tooltip("défini si on est en wallJump ou pas selon la différence normal / variable wallJump 0 - 180"), SerializeField]
    public float angleDifferenceCeilling = 30f;

    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on jump"), SerializeField]
    private Vibration onDie;

    [FoldoutGroup("Object"), Tooltip("direction du joystick"), SerializeField]
    private Transform dirArrow;
    [FoldoutGroup("Object"), Tooltip("list des layer de collisions"), SerializeField]
    private List<GameData.Layers> listLayerToCollide;

    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private SuperPower superPower;
    public SuperPower SuperPowerScript { get { return (superPower); } }
    [FoldoutGroup("Debug"), Tooltip("wallJump 180"), SerializeField]
    public float angleWallJumpRight = 0f;
    [FoldoutGroup("Debug"), Tooltip("walljump 0"), SerializeField]
    public float angleWallJumpLeft = 180f;
    [FoldoutGroup("Debug"), Tooltip("plafond"), SerializeField]
    public float angleCeiling = 270f;

    [FoldoutGroup("Debug"), Tooltip("vecteur de la normal"), SerializeField]
    private Vector3 normalCollide = Vector3.up;
    public Vector3 NormalCollide { get { return (normalCollide); } }

    [FoldoutGroup("Debug"), Tooltip("Marge des 90° du jump (0 = toujours en direction de l'arrow, 0.1 = si angle(normal, arrow) se rapproche de 90, on vise le millieu normal-arrow"), SerializeField]
    private float margeHoriz = 0.1f;
    [FoldoutGroup("Debug"), Tooltip("Marge de vitesse: quand on appuis sur aucune touche, mais qu'on jump: si notre vitesse vertical est suppérieur à cette valeur, on saute dans la direction de l'arrow (et non de la normal précédente)"), SerializeField]
    private float margeSpeedYWhenStopped = 15f;
    [FoldoutGroup("Debug"), Tooltip("list des normals qui touche le joueur"), SerializeField]
    private Vector3[] colliderNormalArray = new Vector3[4];
    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private FrequencyCoolDown coolDownGrounded;
    [FoldoutGroup("Debug"), Tooltip("cooldown du jump (influe sur le mouvements du perso)"), SerializeField]
    private FrequencyCoolDown coolDownGroundedJump; //O.2
    [FoldoutGroup("Debug"), Tooltip("Marge de vitesse: quand on appuis sur aucune touche, mais qu'on jump: si notre vitesse vertical est suppérieur à cette valeur, on saute dans la direction de l'arrow (et non de la normal précédente)"), SerializeField]
    private float debugDiferenceAngleBetweenInputAndPlayer = 30f;

    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private InputPlayer inputPlayer;
    public InputPlayer InputPlayerScript { get { return (inputPlayer); } }

    private bool grounded = false;  //est-on sur le sol ?
    public bool Grounded { get { return (grounded); } }

    private bool enabledObject = true;  //le script est-il enabled ?
    private bool stopAction = false;    //le joueur est-il stopé ?

    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Rigidbody rb;           //ref du rb
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private BetterJump betterJump;
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
        ListExt.ClearArray(colliderNormalArray);

        enabledObject = true;
        stopAction = false;
    }
    #endregion

    #region Core

    /// <summary>
    /// déplace le player
    /// </summary>
    private void TryToMove()
    {
        if (!(InputPlayerScript.Horiz == 0 && InputPlayerScript.Verti == 0))
        {
            if (grounded)
            {
                MoveOnGround();
            }
            else
            {

            }
        }
    }

    /// <summary>
    /// déplace horizontalement le player
    /// </summary>
    /// <param name="inverse"></param>
    private void MoveOnGround()
    {
        //si on a juste sauté, ne rien faire
        if (!coolDownGroundedJump.IsReady() || !coolDownGrounded.IsReady())
            return;

        // Calculate how fast we should be moving
        //Vector3 targetVelocity = new Vector3(InputPlayerScript.Horiz, InputPlayerScript.Verti, 0);
        Vector3 targetVelocity = FindTheRightDir();
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

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
            if (QuaternionExt.IsAngleCloseToOtherByAmount(angleDirInput, anglePlayer, debugDiferenceAngleBetweenInputAndPlayer, out diff) && normalCollide != Vector3.zero)   
            {
                Debug.Log("move normalement !");
            }
            else if (normalCollide == Vector3.zero)
            {
                Debug.Log("ici la normal est égal à zero...");
            }
            else
            {
                Debug.Log("ralenti progressivement ??");
            }

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        
    }

    /// <summary>
    /// set la direction des inputs selon la normal !
    /// </summary>
    /// <returns></returns>
    private Vector3 FindTheRightDir()
    {
        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(InputPlayerScript.Horiz, InputPlayerScript.Verti, 0);
        //targetVelocity = transform.TransformDirection(targetVelocity);

        //si on veut bouger...
        if (targetVelocity.x != 0 || targetVelocity.y != 0)
        {
            Vector3 right = QuaternionExt.CrossProduct(normalCollide, Vector3.forward).normalized;
            float dir = QuaternionExt.DotProduct(targetVelocity, right);

            if (dir < 0)
            {
                //Debug.Log(targetVelocity.magnitude);
                return (-right * targetVelocity.magnitude);
            }
            else
            {
                //Debug.Log(targetVelocity.magnitude);
                return (right * targetVelocity.magnitude);
            }
        }
        else
        {
            return (Vector3.zero);
        }
    }

    private void TryToJump()
    {
        if (!betterJump.CanJump())
            return;

        if (grounded)
        {
            if (InputPlayerScript.JumpInput)
            {
                Vector3 finalVelocityDir = GetDirWhenJumpAndMoving();   //get la direction du joystick / normal / les 2...

                //ici jump, selon la direction voulu, en ajoutant la force du saut
                ActualyJump(finalVelocityDir);
            }
        }
        else
        {
            //not grounded
            if (InputPlayerScript.JumpInput)
            {
                Debug.Log("ici on saute pas dans le vide...");
            }
        }
    }

    /// <summary>
    /// retourne la direction quand on saute...
    /// </summary>
    /// <returns></returns>
    private Vector3 GetDirWhenJumpAndMoving()
    {
        Vector3 finalVelocityDir = Vector3.zero;

        //get la direction du joystick de visé
        Vector3 dirArrowPlayer = getDirArrow();

        //get le dot product normal -> dir Arrow
        float dotDirPlayer = QuaternionExt.DotProduct(normalCollide, dirArrowPlayer);

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
            Vector3 rightVector = QuaternionExt.CrossProduct(normalCollide, Vector3.forward) * -1;
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
    /// get la direction de l'arrow
    /// </summary>
    /// <returns></returns>
    private Vector3 getDirArrow()
    {
        Vector3 dirArrowPlayer = QuaternionExt.QuaternionToDir(dirArrow.rotation, Vector3.up);
        //Debug.DrawRay(transform.position, dirArrowPlayer.normalized, Color.yellow, 1f);
        return (dirArrowPlayer);
    }

    /// <summary>
    /// jump à une direction donnée
    /// </summary>
    /// <param name="dir"></param>
    private void ActualyJump(Vector3 dir)
    {
        if (dir == Vector3.zero)
        {
            dir = Vector3.up;
            //ici pas de rotation ?? 
            Debug.Log("pas de rotation ! up de base !");
        }

        Debug.DrawRay(transform.position, dir, Color.red, 5f);
        GameObject particle = ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.ParticleBump, transform.position, Quaternion.identity, ObjectsPooler.Instance.transform);
        particle.transform.rotation = QuaternionExt.LookAtDir(dir * -1);

        grounded = false;
        coolDownGroundedJump.StartCoolDown();

        betterJump.Jump(dir);
    }

    /// <summary>
    /// Direction arrow
    /// </summary>
    private void ChangeDirectionArrow()
    {
        if (!(InputPlayerScript.HorizRight == 0 && InputPlayerScript.VertiRight == 0) && !isSith)
        {
            dirArrow.rotation = QuaternionExt.DirObject(dirArrow.rotation, InputPlayerScript.HorizRight, -InputPlayerScript.VertiRight, turnRateArrow, QuaternionExt.TurnType.Z);
        }
        else
        {
            dirArrow.rotation = QuaternionExt.DirObject(dirArrow.rotation, normalCollide.x, -normalCollide.y, turnRateArrow, QuaternionExt.TurnType.Z);
        }
    }

    /// <summary>
    /// set un nouveau collider dans l'array
    /// </summary>
    private void SetNewCollider(Vector3 otherNormal)
    {
        otherNormal = otherNormal.normalized;

        if (ListExt.IsInArray(colliderNormalArray, otherNormal))
            return;

        for (int i = 0; i < colliderNormalArray.Length; i++)
        {
            if (colliderNormalArray[i] == Vector3.zero)
            {
                colliderNormalArray[i] = otherNormal;
                return;
            }
        }
    }

    /// <summary>
    /// renvoi vrai si il n'y a qu'un normal dans le tableau
    /// </summary>
    /// <returns></returns>
    private bool IsOnlyOneNormal()
    {
        int number = 0;
        for (int i = 0; i < colliderNormalArray.Length; i++)
        {
            if (colliderNormalArray[i] != Vector3.zero)
            {
                number++;
            }
        }
        if (number == 1)
            return (true);
        return (false);
    }

    private void DisplayNormalArray()
    {
        for (int i = 0; i < colliderNormalArray.Length; i++)
        {
            if (colliderNormalArray[i] != Vector3.zero)
            {
                Debug.DrawRay(transform.position, colliderNormalArray[i], Color.magenta, 1f);
            }
        }
    }

    /// <summary>
    /// retourne 0 si pas en mur, 1 sur droite, -1 si gauche, 2 si plafond
    /// </summary>
    /// <param name="normal"></param>
    /// <returns></returns>
    private int WhatKindOfNormalIsIt(Vector3 normal)
    {
        float angleNormal = QuaternionExt.GetAngleFromVector(normal);    //angle normal collision
        //si la normal de contact est proche de 0 ou 180, à 20° près, alors on est en mode wallJump...
        float diffAngle = 0;

        //ici ne pas bouger si on est en mode wallJump, a droite ou a gauche
        bool left = QuaternionExt.IsAngleCloseToOtherByAmount(angleNormal, angleWallJumpLeft, angleDifferenceWall, out diffAngle);
        //Debug.Log("difLeft: " + diffAngle);
        bool right = QuaternionExt.IsAngleCloseToOtherByAmount(angleNormal, angleWallJumpRight, angleDifferenceWall, out diffAngle);
        //Debug.Log("difRight: " + diffAngle);
        bool ceiling = QuaternionExt.IsAngleCloseToOtherByAmount(angleNormal, angleCeiling, angleDifferenceCeilling, out diffAngle);

        if (right)
        {
            //Debug.Log("right");
            return (1);
        }            
        else if (left)
        {
            //Debug.Log("left");
            return (-1);
        }
            
        else if (ceiling)
        {
            //Debug.Log("ceilling");
            return (2);
        }
        else
        {
            //Debug.Log("ground ! (ou plafon descendant...)");
            return (0);
        }
    }

    /// <summary>
    /// est appelé à chaque onCollision/stay, et reset le cooldown grounded
    /// </summary>
    private void ResetCoolDownGroundedIfGrounded(Vector3 normal)
    {
        int onWall = WhatKindOfNormalIsIt(normalCollide);

        if (onWall == 0)    //on est sur le sol !
        {
            coolDownGrounded.Reset();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(GameData.Prefabs.SuperPower.ToString()) && !isSith)
        {
            superPower.SetSuperPower();
            ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.DeathPlayer, transform.position, Quaternion.identity, ObjectsPooler.Instance.transform);
            Destroy(other.gameObject);
        }
    }

    /// <summary>
    /// trigger le collider à 0.7 (se produit avant le collisionStay)
    /// sauvegarde la direction player - point de contact
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (coolDownGroundedJump.IsReady() && GameData.IsInList(listLayerToCollide, other.gameObject.layer))
        {
            

            Vector3 point = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            Vector3 tmpNormal = (point - transform.position) * -1;
            ResetCoolDownGroundedIfGrounded(tmpNormal);
            SetNewCollider(tmpNormal);

            grounded = true;
            betterJump.OnGrounded();

            CollisionAction(other.gameObject);

        }
    }

    /// <summary>
    /// save la normal de la collision
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionStay(Collision other)
    {
        if (coolDownGroundedJump.IsReady() && GameData.IsInList(listLayerToCollide, other.gameObject.layer))
        {
            Vector3 tmpNormal = other.contacts[0].normal;
            ResetCoolDownGroundedIfGrounded(tmpNormal);
            SetNewCollider(tmpNormal);
            grounded = true;
            betterJump.OnGrounded();
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
            if (IsOnlyOneNormal() && coolDownSelfRepulsion.IsReady() && repulseOtherWhenTouchOnAir)
            {
                //si l'autre n'est pas en l'air, set son coolDown pour pas qu'il saute aussi !
                if (other.GetComponent<PlayerController>().Grounded)
                {
                    other.GetComponent<PlayerController>().CoolDownSelfRepulsion.StartCoolDown();
                }

                //saute !
                coolDownSelfRepulsion.StartCoolDown();
                SetUpNormalCollide();
                ActualyJump(normalCollide);
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

    /// <summary>
    /// set la normal par rapport à la somme des 1-4
    /// </summary>
    private void SetUpNormalCollide()
    {
        Vector3 normalMedium = QuaternionExt.GetMiddleOfXVector(colliderNormalArray);
        if (normalMedium != Vector3.zero)
            normalCollide = normalMedium;
    }
    #endregion


    #region Unity ending functions
    private void Update()
    {
        if (stopAction)
            return;

        ChangeDirectionArrow();
    }

    private void FixedUpdate()
    {
        if (stopAction)
            return;

        SetUpNormalCollide();

        TryToMove();
        TryToJump();

        DisplayNormalArray();
        ListExt.ClearArray(colliderNormalArray);
        grounded = false;
    }

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