using System;
using UnityEditor;
using UnityEngine;

public class PlayerSpawnScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GrappleGun grappleGun;
    [SerializeField] private Vector2 spawnPosition;
    [SerializeField] private Vector2 spawnVelocity;

    private PlayerInput playerInput;
    private Rigidbody2D playerRBD;

    private void Start()
    {
        playerRBD = player.GetComponent<Rigidbody2D>();

        playerInput = new();
        playerInput.Enable();
        playerInput.Player.Respawn.performed += OnRespawn;
    }

    /// <summary>
    /// Event used for resetting player on player input.
    /// </summary>
    /// <param name="context"></param>
    private void OnRespawn(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        ResetPlayer();
    }

    /// <summary>
    /// Calls the functions required for resetting the player.
    /// </summary>
    public void ResetPlayer()
    {
        ResetPosition();
        ResetVelocity();
        ResetGrapple();
    }

    /// <summary>
    /// resets the grapple gun
    /// </summary>
    private void ResetGrapple() { grappleGun.StopGrappling(); }
    /// <summary>
    /// resets the position of the player
    /// </summary>
    private void ResetPosition() { player.transform.position = spawnPosition; }
    /// <summary>
    /// resets the velocity of the player.
    /// </summary>
    private void ResetVelocity() { playerRBD.linearVelocity = spawnVelocity; }

}
