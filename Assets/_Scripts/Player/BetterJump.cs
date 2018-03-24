﻿using Sirenix.OdinInspector;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BetterJump : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), OnValueChanged("InitValue"), Tooltip("hauteur maximal du saut"), SerializeField]
    private float jumpForce = 2.0f;
    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    private float gravity = 9.81f;
    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    private float fallMultiplier = 1.0f;
    [FoldoutGroup("GamePlay"), Tooltip("jumper constament en restant appuyé ?"), SerializeField]
    private bool stayHold = false;
    [Space(10)]
    [FoldoutGroup("GamePlay"), Tooltip("cooldown du jump"), SerializeField]
    private FrequencyCoolDown coolDownJump;

    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on jump"), SerializeField]
    private Vibration onJump;
    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on se pose"), SerializeField]
    private Vibration onGrounded;


    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private InputPlayer inputPlayer;
    [FoldoutGroup("Debug"), Tooltip("ref"), SerializeField]
    private PlayerController playerController;

    private Vector3 initialVelocity;

    private bool jumpStop = false;
    private bool hasJustJump = false;

    #endregion

    #region Initialize
    private void Awake()
    {
        InitValue();
    }

    private void InitValue()
    {
        jumpStop = false;
    }
    #endregion

    #region Core
    /// <summary>
    /// renvoi vrai ou faux si on a le droit de sauter (selon le hold)
    /// </summary>
    /// <returns></returns>
    public bool CanJump()
    {
        //faux si on hold pas et quand a pas laché
        if (jumpStop && !stayHold)
            return (false);
        //faux si le cooldown n'est pas fini
        if (!coolDownJump.IsReady())
            return (false);
        return (true);
    }

    /// <summary>
    /// jump !
    /// </summary>
    public bool Jump(Vector3 dir)
    {
        playerController.Anim.SetBool("Jump", true);

        coolDownJump.StartCoolDown();
        PlayerConnected.Instance.setVibrationPlayer(playerController.IdPlayer, onJump);

        hasJustJump = true;

        Vector3 jumpForce = dir * CalculateJumpVerticalSpeed();
        rb.velocity = jumpForce;

        if (!stayHold)
            jumpStop = true;
        return (true);
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpForce * gravity);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnGrounded()
    {
        if (hasJustJump)
        {
            playerController.Anim.SetBool("Jump", false);
            Debug.Log("ici grounded");
            hasJustJump = false;
            PlayerConnected.Instance.setVibrationPlayer(playerController.IdPlayer, onGrounded);
        }
    }

    private void FixedUpdate ()
	{
        if (!playerController.Grounded && !hasJustJump)
        {
            rb.velocity += playerController.NormalCollide * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            Debug.DrawRay(transform.position, playerController.NormalCollide, Color.magenta, 1f);
        }
    }

    private void Update()
    {
        //on lache, on autorise le saut encore
        if (inputPlayer.JumpUpInput)
            jumpStop = false;
    }

    #endregion
}
