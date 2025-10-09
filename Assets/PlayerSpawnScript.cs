using System;
using UnityEditor;
using UnityEngine;

public class PlayerSpawnScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D playerRBD;
    [SerializeField] private Vector2 spawnPosition;
    [SerializeField] private Vector2 spawnVelocity;

    private PlayerInput playerInput;

    private void Start()
    {
        playerRBD = player.GetComponent<Rigidbody2D>();

        playerInput = new();
        playerInput.Enable();
        playerInput.Player.Respawn.performed += OnRespawn;
    }

    private void OnRespawn(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        ResetPosition();
        ResetVelocity();
    }

    private void ResetPosition() { player.transform.position = spawnPosition; }
    private void ResetVelocity() { playerRBD.linearVelocity = spawnVelocity; }

}
