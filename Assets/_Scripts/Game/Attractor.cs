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
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private BetterJump betterJump;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Rigidbody rb;

    private Vector3 worldLastPosition;      //save la derniere position grounded...
    private Vector3 worldPreviousNormal;    //et sa dernière normal accepté par le changement d'angle
    private Vector3 worldLastNormal;        //derniere normal enregistré, peut import le changement position/angle
    private Vector3 positionAttractPoint;        //direction de l'attractPoint
    private Vector3 dirAttractPoint;

    #endregion

    #region Initialization
    private void Awake()
    {
        InitValue();
    }

    private void InitValue()
    {
        hasAttractPoint = false;
        coolDownCreateAttractPoint.StartCoolDown();
    }
    #endregion

    #region Core
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

        //si la normal à changé, update la position + normal !
        if (worldPreviousNormal != playerController.NormalCollide)
        {
            worldLastNormal = playerController.NormalCollide;   //avoir toujours une normal à jour

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
                worldLastPosition = transform.position; //save la position onGround
                worldPreviousNormal = worldLastNormal;

                DebugExtension.DebugWireSphere(worldLastPosition, Color.yellow, 0.5f, 1);
                Debug.DrawRay(transform.position, worldPreviousNormal, Color.magenta, 1f);
            }

            //coolDownUpdatePos.StartCoolDown();
        }
        else
        {
            //si la normal n'a pas changé, regarder pour la position si on est plus loin
            float distForSave = (worldLastPosition - transform.position).sqrMagnitude;
            if (distForSave > sizeDistanceForSaveSquare)
            {
                worldLastPosition = transform.position; //save la position onGround
                DebugExtension.DebugWireSphere(worldLastPosition, Color.yellow, 0.5f, 1);
            }
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
        coolDownCreateAttractPoint.StartCoolDown();

        Debug.Log("ici on stup l'attract point !");

        //lengthInputForceAttractPoint = playerController.FindTheRightDir().magnitude;    //ici la force de l'attract point ! (0 - 1)
        lengthInputForceAttractPoint = rb.velocity.magnitude;    //ici la force de l'attract point ! (0 - MAxVelocityPlayer)

        //TODOO
        //ici la pos ancien, + X dans le sens de la normal précédente ??
        DebugExtension.DebugWireSphere(worldLastPosition, Color.red, 2f, 1);
        positionAttractPoint = worldLastPosition - worldLastNormal * lengthPositionAttractPoint;

        DebugExtension.DebugWireSphere(positionAttractPoint, Color.blue, 2f, 1);
        Debug.Break();
    }

    /// <summary>
    /// ici attire le player s'il ne faut
    /// </summary>
    private void AttractPlayer()
    {
        if (!hasAttractPoint)
            return;

        Debug.Log("Ici attract player jusqu'a ce qu'il soit sur le sol (ou hors limite ???)");

        dirAttractPoint = (positionAttractPoint - transform.position).normalized * lengthInputForceAttractPoint;
        dirAttractPoint *= lengthInputForceAttractPoint;    //applique le ration de la velocité du ribidbody
        dirAttractPoint *= forceAttractPoint;               //applique la force de l'attractPoint !

        //rb.velocity += dirAttractPoint * Physics.gravity.y * (betterJump.FallMultiplier - 1) * Time.fixedDeltaTime;
        //Debug.DrawRay(transform.position, dirAttractPoint, Color.magenta, 1f);


        //ici renvoyer vrai ou faux selon si le dir est derriere la derniere normal ?
        //pour pas appliquer la vieille force de normal après...
    }

    
    #endregion

    #region Unity ending functions

    #endregion
}
