using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// WrapMode Description
/// </summary>
public class Grip : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("jumper constament en restant appuyé ?"), SerializeField]
    public bool stayHold = false;
    [FoldoutGroup("GamePlay"), Tooltip("cooldown du jump (influe sur le mouvements du perso)"), SerializeField]
    private FrequencyCoolDown coolDownGrip;

    [FoldoutGroup("Object"), Tooltip("est-on grippé ?"), SerializeField]
    public GameObject display;

    [FoldoutGroup("Debug"), Tooltip("est-on grippé ?"), SerializeField]
    public bool gripped = false;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private PlayerController playerController;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private InputPlayer inputPlayer;


    #endregion

    #region Initialization
    private void Awake()
    {
        InitValue();
    }

    private void InitValue()
    {
        gripped = false;
        display.SetActive(false);
    }
    #endregion

    #region Core
    /// <summary>
    /// renvoi vrai ou faux si on a le droit de sauter (selon le hold)
    /// </summary>
    /// <returns></returns>
    public bool CanGrip()
    {
        //faux si le cooldown n'est pas fini
        if (!coolDownGrip.IsReady() || !playerController.Grounded)
            return (false);
        return (true);
    }

    /// <summary>
    /// aggriper !
    /// </summary>
    /// <returns></returns>
    public bool Gripping(bool grip)
    {
        if (grip && !CanGrip())
            return (false);

        gripped = grip;
        if (gripped)
        {
            Debug.Log("ici grip");
            display.SetActive(true);
            rb.isKinematic = gripped;
        }
        else
        {
            Debug.Log("ici ungrip");
            display.SetActive(false);
            rb.isKinematic = gripped;

            coolDownGrip.StartCoolDown();
        }
        return (true);
    }
    #endregion

    #region Unity ending functions
    private void Update()
    {
        //si on reste appuyé
        if (inputPlayer.GripInput && !gripped)
        {
            Gripping(true);
        }
        //si on lache, stopper le gripped
        //(et démarrer le coolDown pour pas réactiver tout de suite après)
        if (inputPlayer.GripUpInput && gripped)
        {
            Gripping(false);
        }
    }
    #endregion
}
