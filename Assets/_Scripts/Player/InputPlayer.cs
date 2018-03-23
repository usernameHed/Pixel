using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// InputPlayer Description
/// </summary>
public class InputPlayer : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("Object"), Tooltip("id unique du joueur correspondant à sa manette"), SerializeField]
    private PlayerController playerController;
    public PlayerController PlayerController { get { return (playerController); } }

    private float horiz;    //input horiz
    public float Horiz { get { return (horiz); } }
    private float verti;    //input verti
    public float Verti { get { return (verti); } }
    private bool jumpInput; //jump input
    public bool JumpInput { get { return (jumpInput); } }
    private bool jumpUpInput; //jump input
    public bool JumpUpInput { get { return (jumpUpInput); } }
    private bool gripInput; //grip input hold
    public bool GripInput { get { return (gripInput); } }
    private bool gripDownInput; //grgip input down
    public bool GripDownInput { get { return (gripDownInput); } }
    private bool gripUpInput; //grip input up
    public bool GripUpInput { get { return (gripUpInput); } }

    #endregion

    #region Initialization

    #endregion

    #region Core
    private void GetInput()
    {
        horiz = PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetAxis("Move Horizontal");
        verti = PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetAxis("Move Vertical");
        jumpInput = PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetButton("FireA");
        jumpUpInput = PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetButtonUp("FireA");
        gripInput = PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetButton("FireX");
        gripUpInput = PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetButtonUp("FireX");
        gripDownInput = PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetButtonDown("FireX");
    }
    #endregion

    #region Unity ending functions

    private void Update()
    {
        GetInput();
    }

	#endregion
}
