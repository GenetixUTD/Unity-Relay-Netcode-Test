using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameLookAt : MonoBehaviour
{
    public Camera playerCamera;  // Manually assign the local player's camera

    void Start()
    {
        if (playerCamera == null)
        {
            // If no camera is assigned, try to find the main camera
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        if (playerCamera == null) return;

        Vector3 directionToCamera = playerCamera.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(-directionToCamera);
        transform.rotation = lookRotation;
    }
}
