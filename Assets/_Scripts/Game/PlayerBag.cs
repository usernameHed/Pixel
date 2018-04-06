using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// PlayerBag Description
/// </summary>
public class PlayerBag : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("list des layer de collisions"), SerializeField]
    private int numberNoisette = 0;

    [FoldoutGroup("GamePlay"), Tooltip("list des layer de collisions"), SerializeField]
    private GameObject[] noisertteArray = new GameObject[6];
    #endregion

    #region Initialization

    #endregion

    #region Core
    public void GetNoisette()
    {
        if (numberNoisette >= noisertteArray.Length)
            return;
        noisertteArray[numberNoisette].SetActive(true);
        numberNoisette++;
    }
    #endregion

    #region Unity ending functions

    #endregion
}
