using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Attractor Description
/// </summary>
public class Attractor : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("position de l'attractpoint"), SerializeField]
    public float lengthPositionAttractPoint = 1f;    //position de l'attract point par rapport à la dernier position / normal

    [FoldoutGroup("GamePlay"), Tooltip("force"), SerializeField]
    public float forceAttractPoint = 10f;

    [FoldoutGroup("Debug"), Tooltip("un attract point existe ?"), SerializeField]
    private bool hasAttractPoint = false;   //a-t-on un attract point de placé ?
    [FoldoutGroup("Debug"), Tooltip("espace entre 2 sauvegarde de position ?"), SerializeField]
    private float sizeDistanceForSaveSquare = 0.5f;   //a-t-on un attract point de placé ?
    [FoldoutGroup("Debug"), Tooltip("espace entre 2 sauvegarde de position ?"), SerializeField]
    private float differenceAngleNormalForUpdatePosition = 5f;   //a-t-on un attract point de placé ?

    //selon la forcce de l'input quand on vient d'être en l'air, appliquer une force proportionnel de l'attract point
    //selon l'input, de 0.01 à 1 (plus un va vite avec l'input, plus on appliquer la force
    private float lengthInputForceAttractPoint;

    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private FrequencyCoolDown coolDownCreateAttractPoint;   //0.1, activé au démarage !

    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private PlayerController playerController;
    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private PlayerMove playerMove;
    [FoldoutGroup("Debug"), Tooltip("cooldown du déplacement horizontal"), SerializeField]
    private WorldCollision worldCollision;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private PlayerPhysics betterJump;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Rigidbody rb;

    private Vector3 [] worldLastPosition = new Vector3[3];      //save la derniere position grounded...
    private void WorldLastPositionSet(Vector3 newValue)
    {
        Vector3 next = Vector3.zero;
        for (int i = 0; i < worldLastPosition.Length - 1; i++)
        {
            if (i == 0)
            {
                next = worldLastPosition[0];
                worldLastPosition[0] = newValue;
            }                
            else
            {
                Vector3 tmpValue = worldLastPosition[i];
                worldLastPosition[i] = next;
                next = tmpValue;                
            }
        }
    }
    private Vector3 WorldLastPositionGetIndex(int index)
    {
        index = (index < 0) ? 0 : index;
        index = (index >= worldLastPosition.Length) ? worldLastPosition.Length - 1 : index;
        return (worldLastPosition[index]);
    }

    private Vector3 worldPreviousNormal;    //et sa dernière normal accepté par le changement d'angle
    private Vector3 worldLastNormal;        //derniere normal enregistré, peut import le changement position/angle
    private Vector3 positionAttractPoint;        //direction de l'attractPoint
    private Vector3 dirAttractPoint;

    private float forceAttractPointConstant = 10000f;   //why ???

    #endregion

    #region Initialization
    private void Awake()
    {
        InitValue();
    }

    private void InitValue()
    {
        hasAttractPoint = false;
        StartCoolDown();
    }
    #endregion

    #region Core
    /// <summary>
    /// 
    /// </summary>
    public void StartCoolDown()
    {
        coolDownCreateAttractPoint.StartCoolDown();
    }

    /// <summary>
    /// ici reset l'attractor anti gravité
    /// </summary>
    public void ResetAttractPoint()
    {
        if (!hasAttractPoint)
            return;
        Debug.Log("reset Attract Point");
        hasAttractPoint = false;
    }
    /// <summary>
    /// on est grounded, en profite pour sauvegarder la position !
    /// </summary>
    public void SaveLastPositionOnground()
    {
        ResetAttractPoint();    //ici se reset, on est sur le ground !

        worldLastNormal = worldCollision.GetSumNormalSafe();   //avoir toujours une normal à jour
        float distForSave = (WorldLastPositionGetIndex(0) - transform.position).sqrMagnitude;

        //si la distance entre les 2 point est trop grande, dans tout les cas, save la nouvelle position !
        if (distForSave > sizeDistanceForSaveSquare)
        {
            WorldLastPositionSet(transform.position); //save la position onGround
            DebugExtension.DebugWireSphere(WorldLastPositionGetIndex(0), Color.yellow, 0.5f, 1f);
        }
        //si la normal à changé, update la position + normal !
        else if (worldPreviousNormal != worldCollision.GetSumNormalSafe())
        {
            //ici changement de position SEULEMENT si l'angle de la normal diffère de X
            float anglePreviousNormal = QuaternionExt.GetAngleFromVector(worldPreviousNormal);
            float angleNormalPlayer = QuaternionExt.GetAngleFromVector(worldLastNormal);
            //ici gérer les normal à zero ??
            float diff;
            if (QuaternionExt.IsAngleCloseToOtherByAmount(anglePreviousNormal, angleNormalPlayer, differenceAngleNormalForUpdatePosition, out diff))
            {
                //ici l'angle est trop proche, ducoup ne pas changer de position
                
                //ni de normal ??
            }
            else
            {

                //ici change la normal, ET la position
                WorldLastPositionSet(transform.position); //save la position onGround
                worldPreviousNormal = worldLastNormal;

                DebugExtension.DebugWireSphere(WorldLastPositionGetIndex(0), Color.yellow, 0.5f, 1f);
                //Debug.DrawRay(transform.position, worldPreviousNormal, Color.yellow, 1f);
            }

            //coolDownUpdatePos.StartCoolDown();
        }
    }

    /// <summary>
    /// ici setup l'attractPoint
    /// </summary>
    public void SetUpAttractPoint()
    {
        //si on a déjà un attractPoint, ou que le cooldown entre 2 attractPoint n'est pas prêt...
        if (hasAttractPoint || !coolDownCreateAttractPoint.IsReady())
            return;

        hasAttractPoint = true;
        StartCoolDown();

        Debug.Log("ici on stup l'attract point !");

        lengthInputForceAttractPoint = playerMove.FindTheRightDir().magnitude;    //ici la force de l'attract point ! (0 - 1)
        //lengthInputForceAttractPoint = rb.velocity.magnitude;    //ici la force de l'attract point ! (0 - MAxVelocityPlayer)

        //TODOO
        //ici la pos ancien, + X dans le sens de la normal précédente ??
        positionAttractPoint = WorldLastPositionGetIndex(1) - worldLastNormal * lengthPositionAttractPoint;

        DebugExtension.DebugWireSphere(WorldLastPositionGetIndex(1), Color.red, 1f, 2f);          //ancienne pos
        DebugExtension.DebugWireSphere(positionAttractPoint, Color.blue, 1f, 2f);      //nouvel pos
        Debug.DrawRay(WorldLastPositionGetIndex(0), worldLastNormal * 4, Color.red, 2f);      //last normal

        //Debug.Break();
    }

    /// <summary>
    /// ici renvoi si on a ou non un attract point
    /// </summary>
    /// <returns></returns>
    public bool HasAttractPoint()
    {
        return (hasAttractPoint);
    }

    /// <summary>
    /// ici attire le player s'il ne faut
    /// </summary>
    public void SetNewNormalForceWhenFlying()
    {
        if (!hasAttractPoint)
            return;

        //Debug.Log("Ici attract player jusqu'a ce qu'il soit sur le sol (ou hors limite ???)");

        dirAttractPoint = (positionAttractPoint - transform.position).normalized;
        //dirAttractPoint *= lengthInputForceAttractPoint;    //applique le ration de la velocité du ribidbody
        dirAttractPoint *= forceAttractPointConstant;               //applique la force de l'attractPoint !

        //rb.velocity += dirAttractPoint * Physics.gravity.y * (betterJump.FallMultiplier - 1) * Time.fixedDeltaTime;
        //Debug.DrawRay(transform.position, dirAttractPoint, Color.magenta, 1f);

        if (dirAttractPoint == Vector3.zero)
        {
            //Debug.LogWarning("vecteur zero antigravité !");
            return;
        }

        //Debug.Log("TODO: milieu de dirAttractPoint & worldLastNormal");
        //Debug.Log();

        float signVector = QuaternionExt.DotProduct(dirAttractPoint, -worldLastNormal);
        if (signVector > 0)
        {
            worldCollision.SetSumNormal(-QuaternionExt.GetMiddleOf2Vector(dirAttractPoint, -worldLastNormal));
        }
        else
        {
            worldCollision.SetSumNormal(-QuaternionExt.GetMiddleOf2Vector(dirAttractPoint, worldLastNormal));
        }
        //playerController.NormalCollide = -dirAttractPoint;

        //Debug.DrawRay(transform.position, playerController.NormalCollide, Color.magenta, 10f);      //last normal
        //ici renvoyer vrai ou faux selon si le dir est derriere la derniere normal ?
        //pour pas appliquer la vieille force de normal après...
        
        rb.AddForce(worldCollision.GetSumNormalSafe() * -forceAttractPoint, ForceMode.VelocityChange);
    }

    
    #endregion

    #region Unity ending functions

    #endregion
}
