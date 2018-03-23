using UnityEngine;
using Sirenix.OdinInspector;
using Obi;

/// <summary>
/// RopeHandler Description
/// </summary>
public class RopeHandler : MonoBehaviour
{
    #region Attributes

    
    [FoldoutGroup("GamePlay"), Tooltip("vitesse d'ajout/suppression"), SerializeField]
    private float hookExtendRetractSpeed = 2;
    [FoldoutGroup("GamePlay"), Tooltip("valeur de stretch avant d'ajouter"), SerializeField]
    private float stretchAdd = 1.5f;
    [FoldoutGroup("GamePlay"), Tooltip("valeur de stretch avant de supprimer"), SerializeField]
    private float stretchRemove = 0.5f;

    [FoldoutGroup("GamePlay"), Tooltip("ajout max de particule"), SerializeField]
    private int minParticle = 80;
    [FoldoutGroup("GamePlay"), Tooltip("suppression min de aprticule"), SerializeField]
    private int maxParticle = 105;

    [FoldoutGroup("Object"), Tooltip("liens de la rope"), SerializeField]
    private ObiRope rope;
    [FoldoutGroup("Object"), Tooltip("liens du curseur (pour ajouter/enleverl des particule)"), SerializeField]
    private ObiRopeCursor cursor;
    #endregion

    #region Initialization

    private void Start()
    {

    }
    #endregion

    #region Core
    /// <summary>
    /// change la rope
    /// </summary>
    private void ModifyRope()
    {
        float strain = rope.CalculateLength() / rope.RestLength;

        if (strain > stretchAdd && rope.PooledParticles > minParticle) //ici la rope est tendu !
        {
            Debug.Log("add");
            cursor.ChangeLength(rope.RestLength + hookExtendRetractSpeed * Time.deltaTime);
            //cursor.normalizedCoord = 0.5f;
        }
        else if (strain < stretchRemove && rope.PooledParticles < maxParticle)
        {
            Debug.Log("less");
            cursor.ChangeLength(rope.RestLength - hookExtendRetractSpeed * Time.deltaTime);
            //cursor.normalizedCoord = 0.5f;
        }
    }
    #endregion

    #region Unity ending functions

    private void Update()
    {
        ModifyRope();
    }

	#endregion
}
