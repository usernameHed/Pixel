using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class PlayerData : PersistantData
{
    #region Attributes

    [FoldoutGroup("GamePlay"), Tooltip("score des 4 joueurs"), SerializeField]
    private int[] scorePlayer = new int[SizeArrayId];
    public int[] ScorePlayer { get { return scorePlayer; } }


    private const int SizeArrayId = 2;  //nombre de ball du joueur
    #endregion

    #region Core
    /// <summary>
    /// reset toute les valeurs à celle d'origine pour le jeu
    /// </summary>
    public void SetDefault()
    {
        for (int i = 0; i < SizeArrayId; i++)
        {
            scorePlayer[i] = 0;
        }        
    }

    public override string GetFilePath ()
	{
		return "playerData.dat";
	}

	#endregion
}