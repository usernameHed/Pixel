using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class PlayerData : PersistantData
{
    #region Attributes

    [FoldoutGroup("GamePlay"), Tooltip("score des 4 joueurs")]
    public int scorePlayer = 0;


    private const int SizeArrayId = 2;  //nombre de ball du joueur
    #endregion

    #region Core
    /// <summary>
    /// reset toute les valeurs à celle d'origine pour le jeu
    /// </summary>
    public void SetDefault()
    {
        scorePlayer = 0;
    }

    public override string GetFilePath ()
	{
		return "playerData.dat";
	}

	#endregion
}