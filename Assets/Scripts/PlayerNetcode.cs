using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerNetcode : NetworkBehaviour
{
    public Camera myCamera;
    public AudioListener myAudioListener;

    
    public Vector3 spawnLocation;

    public override void OnNetworkSpawn()
    {
        myCamera.gameObject.SetActive(IsOwner);
        myAudioListener.enabled = IsOwner;

        

        if(IsOwner)
        {
            transform.position = spawnLocation;
        }
    }
}
