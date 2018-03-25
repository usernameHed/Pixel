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
    private float horizRight;    //input horiz
    public float HorizRight { get { return (horizRight); } }
    private float vertiRight;    //input verti
    public float VertiRight { get { return (vertiRight); } }
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
        horizRight = PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetAxis("Move Horizontal Right");
        vertiRight = PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetAxis("Move Vertical Right");

        jumpInput = PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetButton("FireA") || PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetButton("RightTrigger2") || PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetButton("LeftTrigger2");
        jumpUpInput = PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetButtonUp("FireA") || PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetButtonUp("RightTrigger2") || PlayerConnected.Instance.getPlayer(playerController.IdPlayer).GetButton("LeftTrigger2");
    }
    #endregion

    #region Unity ending functions

    private void Update()
    {
        GetInput();
    }

	#endregion
}
