using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private Vector3 Velocity;
    private Vector3 PlayerMovementInput;
    private Vector2 PlayerMouseInput;
    private float xRot;

    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private CharacterController Controller;
    [Space]
    [SerializeField] private float Speed;
    [SerializeField] private float Jumpforce;
    [SerializeField] private float Sensitivity;
    private float Gravity = 9.81f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Quaternion lastRotation;

    public TMP_Text myNameTag;
    public string myName;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if (IsOwner)
        {
            PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            MovePlayer();
            MovePlayerCamera();

            SubmitMovementServerRpc(transform.position, Velocity, transform.rotation);
            myName = PlayerPrefs.GetString("clientName");
            myNameTag.text = myName;
        }
        else
        {
            InterpolatePositionAndRotation();
        }
    }

    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput);


        if (Controller.isGrounded)
        {
            Velocity.y = -1f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Velocity.y = Jumpforce;
            }

        }
        else
        {
            Velocity.y += Gravity * -2f * Time.deltaTime;
        }

        Controller.Move(MoveVector * Speed * Time.deltaTime);
        Controller.Move(Velocity * Time.deltaTime);
    }

    private void MovePlayerCamera()
    {
        xRot -= PlayerMouseInput.y * Sensitivity;
        transform.Rotate(0f, PlayerMouseInput.x * Sensitivity, 0f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }

    private void InterpolatePositionAndRotation()
    {
        // Smoothly interpolate position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
        // Smoothly interpolate rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

        myNameTag.text = myName;
    }

    [ServerRpc]
    private void SubmitMovementServerRpc(Vector3 position, Vector3 velocity, Quaternion rotation)
    {
        // Sync the player position, velocity, and rotation on the server
        transform.position = position;
        this.Velocity = velocity;
        targetPosition = position; // Update target position for interpolation
        targetRotation = rotation; // Update target rotation for interpolation
        SubmitMovementClientRpc(position, velocity, rotation);
    }

    [ClientRpc]
    private void SubmitMovementClientRpc(Vector3 position, Vector3 velocity, Quaternion rotation)
    {
        if (!IsOwner)
        {
            // Update position, velocity, and rotation for non-owning clients
            targetPosition = position;
            this.Velocity = velocity;
            targetRotation = rotation;
        }
    }
}
